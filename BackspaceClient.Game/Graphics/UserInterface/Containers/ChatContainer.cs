using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackspaceClient.Game.Extensions;
using BackspaceClient.Game.Online.API;
using BackspaceClient.Game.Online.API.Request;
using BackspaceClient.Game.Online.API.Response;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Logging;
using osu.Framework.Testing;
using osuTK;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class ChatContainer : Container
{
    private static readonly Colour4 surface_chat = Colour4.FromHex("13131a");

    private readonly Dictionary<string, MessageView> messagesById = new();

    private Box backgroundBox;
    private BasicScrollContainer scrollContainer;
    private FillFlowContainer messagesFlow;
    private TextBox messageTextBox;
    private Container textBoxContainer;

    private bool scrollToEndPending = false;
    private string lastWriter;
    public string DmChannelId
    {
        get => dmChannelId.Value;
        set => dmChannelId.Value = value;
    }

    private Bindable<string> dmChannelId { get; } = new();

    public APIProvider Provider;

    public readonly InstanceObserver InstanceObserver;

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
                            Padding = new MarginPadding { Horizontal = 20 },
                            Child = messagesFlow = new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Padding = new MarginPadding { Vertical = 15 },
                                Spacing = new Vector2(0, 10),
                            },
                        }
                    },
                    [
                        textBoxContainer = new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Padding = new MarginPadding { Horizontal = 20, Bottom = 20, Top = 5 },
                            Alpha = 0f,
                            Child = messageTextBox = new BackspaceClientTextBox
                            {
                                RelativeSizeAxes = Axes.X,
                                Height = 45,
                                ReleaseFocusOnCommit = false
                            }
                        }
                    ]
                }
            }
        ];

        dmChannelId.BindValueChanged(v =>
        {
            textBoxContainer.FadeTo(v.NewValue != null ? 1f : 0f, 200, Easing.OutQuint);
            lastWriter = null;
        });

        messageTextBox.OnCommit += (textBox, newValue) =>
        {
            Task.Run(() =>
            {
                lock(InstanceObserver)
                {
                    var request = new SendMessageReqeust(DmChannelId)
                    {
                        Content = textBox.Current.Value
                    };
                    request.AttachApi(Provider);
                    InstanceObserver.PerformAPIRequest(request);
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

        if (lastWriter != null && string.Equals(data.User.CanonicalKey(), lastWriter))
        {
            messagesFlow.ChildrenOfType<MessageView>().Last().Model.Content.Value += "\n" + data.Content;
            goto scroll;
        }

        var view = new MessageView(data);
        messagesById[data.Id] = view;
        messagesFlow.Add(view);
        lastWriter = data.User.CanonicalKey();

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

        messagesFlow.Remove(deletedMessage, true);
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
