using System.Collections.Generic;
using System.Linq;
using BackspaceClient.Game.Online.API.Response;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class MessageContentContainer : Container
{
    private static readonly Colour4 surface_message = Colour4.FromHex("23232a");

    private readonly List<MessageView> messages = [];

    public bool IsCanBeConsecutive => messages.Last().Model.Attachments.Count == 0 || messages.Last().Model.ReplyTo.Value == null;

    public User WhoseMessage
    {
        get
        {
            if (field != null) return field;
            if (messages.Count > 0)
                field = messages.First().Model.User;

            return field;
        }
        init;
    } = null;

    private Box background;

    private FillFlowContainer messagesContainer = new()
    {
        Direction = FillDirection.Vertical,
        RelativeSizeAxes = Axes.X,
        AutoSizeAxes = Axes.Y,
    };

    public MessageContentContainer(MessageView ms) => Add(ms);

    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = true;
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        CornerRadius = 8f;

        InternalChildren =
        [
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = surface_message
            },
            messagesContainer
        ];
    }

    public void Add(MessageView message)
    {
        messages.Add(message);

        messagesContainer.Add(message);
    }
}
