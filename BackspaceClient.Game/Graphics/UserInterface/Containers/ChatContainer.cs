using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackspaceClient.Game.Extensions;
using BackspaceClient.Game.Online.API;
using BackspaceClient.Game.Online.API.Request;
using BackspaceClient.Game.Online.API.Response;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Testing;
using osuTK;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class ChatContainer : Container
{
    private static readonly Colour4 surface_chat = Colour4.FromHex("#09090B");
    private static readonly Colour4 input_divider_colour = Colour4.FromHex("#18181B");

    private readonly Dictionary<string, MessageView> messagesById = new();

    private bool scrollToEndPending = false;
    public string DmChannelId
    {
        get => dmChannelId.Value;
        set => dmChannelId.Value = value;
    }

    private Bindable<string> dmChannelId { get; } = new();

    public APIProvider Provider;

    public readonly InstanceObserver InstanceObserver;

    private Box backgroundBox;

    private TextBox messageTextBox;
    private BasicButton fileSelectorButton;

    private BasicScrollContainer scrollContainer;
    private FillFlowContainer messagesFlow;
    private Container textBoxContainer;

    public ChatContainer(InstanceObserver observer)
    {
        RelativeSizeAxes = Axes.Both;
        InstanceObserver = observer;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            backgroundBox = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = surface_chat,
            },
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                RowDimensions =
                [
                    new Dimension(GridSizeMode.Distributed),
                    new Dimension(GridSizeMode.AutoSize)
                ],
                Content = new[]
                {
                    new Drawable[]
                    {
                        scrollContainer = new BasicScrollContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            ScrollbarOverlapsContent = false,
                            Padding = new MarginPadding { Horizontal = 24 },
                            Child = messagesFlow = new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                Width = 0.8f,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Padding = new MarginPadding { Vertical = 20 },
                                Spacing = new Vector2(0, 10),
                            },
                        }
                    },
                    [
                        textBoxContainer = new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            Width = 0.8f,
                            AutoSizeAxes = Axes.Y,
                            Padding = new MarginPadding { Horizontal = 20, Bottom = 20, Top = 5 },
                            Alpha = 0f,
                            Children =
                            [
                                new Box
                                {
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,
                                    RelativeSizeAxes = Axes.X,
                                    Height = 1f,
                                    Colour = input_divider_colour,
                                    Margin = new MarginPadding { Bottom = 10 },
                                },
                                messageTextBox = new BackspaceClientTextBox
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = 44,
                                    PlaceholderText = "Write a message...",
                                    ReleaseFocusOnCommit = false,
                                    FontSize = 32
                                }
                            ]
                        }
                    ]
                }
            }
        ];

        dmChannelId.BindValueChanged(v =>
        {
            textBoxContainer.FadeTo(v.NewValue != null ? 1f : 0f, 200, Easing.OutQuint);
        });

        messageTextBox.OnCommit += (textBox, newValue) =>
        {
            Task.Run(() =>
            {
                lock (InstanceObserver)
                {
                    var request = new SendMessageReqeust(DmChannelId)
                    {
                        Content = textBox.Current.Value
                    };

                    var channelAck = new { type = "channel_ack", channelId = DmChannelId, messageId = messagesById.Last().Key };

                    request.AttachApi(Provider);

                    InstanceObserver.PerformAPIRequest(request);
                    InstanceObserver.WebSocketClient.SendAsync(JsonConvert.SerializeObject(channelAck)).ConfigureAwait(false);

                    Schedule(() =>
                    {
                        textBox.Current.Value = "";
                    });
                }
            });
        };
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        // If I will use scrollBottom from Schedule it will not scroll to end
        // Or when is near bottom then Schedule will be executed before scroll container will be updated
        if (scrollToEndPending)
        {
            scrollToEndPending = false;
            scrollToBottom();
        }
    }

    /// <summary>
    /// Adds message. If chat contains message then message will be edit
    /// </summary>
    public void AddOrUpdateMessage(DmChatMessageResponse data, bool slideToEnd = false)
    {
        if (messagesById.TryGetValue(data.Id, out var existing))
        {
            existing.Model.UpdateFromResponse(data);
            return;
        }

        if (messagesFlow.Count > 0)
        {
            var message = messagesFlow.ChildrenOfType<MessageContentContainer>().Last();

            if (message != null
                && string.Equals(data.User.CanonicalKey(), message.WhoseMessage?.CanonicalKey())
                && message.IsCanBeConsecutive)
            {
                message.Add(new MessageView(data, InstanceObserver?.Current.Value));
                goto scroll;
            }
        }

        var view = new MessageView(data, InstanceObserver?.Current.Value);
        var container = new MessageContentContainer(view);
        messagesById[data.Id] = view;
        Schedule(() => messagesFlow.Add(container));

    scroll:
        if (isNearBottom())
            scrollToEndPending = true;
        else if (slideToEnd)
            scrollToEndPending = true;
    }

    /// <summary>
    /// Delete or mark message as deleted
    /// </summary>
    public void RemoveMessage(string messageId, bool silent = false)
    {
        if (!silent)
        {
            if (messagesById.TryGetValue(messageId, out var markMessage))
                markMessage.Model.MarkDeleted();
            return;
        }

        if (!messagesById.Remove(messageId, out var deletedMessage))
            return;

        if (deletedMessage.Parent is FillFlowContainer parentContainer)
            parentContainer.Remove(deletedMessage, false);
    }

    public void ClearMessages()
    {
        messagesFlow.Clear();
        messagesById.Clear();
    }

    public bool Contains(string messageId) => messagesById.ContainsKey(messageId);

    private bool isNearBottom()
    {
        var distance = scrollContainer.ScrollableExtent - scrollContainer.Current;
        return distance < 100;
    }

    private void scrollToBottom()
    {
        scrollContainer.ScrollToEnd();
    }
}
