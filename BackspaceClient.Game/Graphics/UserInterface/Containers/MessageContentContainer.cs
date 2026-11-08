using System.Collections.Generic;
using System.Linq;
using BackspaceClient.Game.Online.API.Response;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class MessageContentContainer : Container
{
    private static readonly Colour4 surface_message = Colour4.FromHex("#121215");
    private static readonly Colour4 border_message = Colour4.FromHex("#1F1F23");
    private static readonly Colour4 shadow_message = Colour4.Black.Opacity(0.3f);

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
        Padding = new MarginPadding { Vertical = 4 },
    };

    public MessageContentContainer(MessageView ms) => Add(ms);

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Masking = true;
        CornerRadius = 10f;
        BorderThickness = 1f;
        BorderColour = border_message;

        EdgeEffect = new EdgeEffectParameters
        {
            Type = EdgeEffectType.Shadow,
            Colour = shadow_message,
            Radius = 12,
            Offset = new Vector2(0, 3),
        };

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
