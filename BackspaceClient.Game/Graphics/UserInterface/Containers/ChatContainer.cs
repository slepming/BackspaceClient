using System.Collections.Generic;
using BackspaceClient.Game.Online.API;
using BackspaceClient.Game.Online.API.Request;
using BackspaceClient.Game.Online.API.Response;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class ChatContainer : Container
{
    private static readonly Colour4 surface_chat = Colour4.FromHex("13131a");

    private readonly Dictionary<string, MessageView> messagesById = new();

    private Box backgroundBox;
    private BasicScrollContainer scrollContainer;
    private FillFlowContainer messagesFlow;
    private Drawable bottomSpacer;
    private TextBox messageTextBox;

    private Bindable<string> dmChannelId { get; } = new();

    public string DmChannelId
    {
        get => dmChannelId.Value;
        set => dmChannelId.Value = value;
    }

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
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Children =
                [
                    scrollContainer = new()
                    {
                        RelativeSizeAxes = Axes.Both,
                        Size = new(1.0f, .93f),
                        ScrollbarOverlapsContent = false,
                        Child = messagesFlow = new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Padding = new MarginPadding { Vertical = 12 },
                        },
                    },
                    messageTextBox = new BackspaceClientTextBox
                    {
                        RelativeSizeAxes = Axes.Both,
                        Size = new(1.0f, .07f),
                    }
                ]
            }
        ];

        messageTextBox.OnCommit += (textBox, newValue) =>
        {
            var request = new SendMessageReqeust(DmChannelId)
            {
                Content = textBox.Current.Value
            };
            request.AttachApi(Provider);
            InstanceObserver.PerformAPIRequest(request);
            textBox.Current.Value = "";
        };
    }

    /// <summary>
    /// Adds message. If chat contains message then message will be edit
    /// </summary>
    public void AddOrUpdateMessage(DmChatMessageResponse data, bool slideToEnd = false)
    {
        if (messagesById.TryGetValue(data.Id, out var existing))
        {
            existing.Model.UpdateFrom(data);
            return;
        }

        var view = new MessageView(data);
        messagesById[data.Id] = view;
        messagesFlow.Add(view);

        if (isNearBottom())
            Schedule(scrollToBottom);
        if (slideToEnd)
            Schedule(scrollToBottom);
    }


    /// <summary>
    /// Delete or mark message as deleted
    /// </summary>
    public void RemoveMessage(string messageId, bool mark = false)
    {
        if (mark)
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
