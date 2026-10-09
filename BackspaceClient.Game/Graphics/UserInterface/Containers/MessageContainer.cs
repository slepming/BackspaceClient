using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;
using BackspaceClient.Game.Online.API.Models;
using BackspaceClient.Game.Online.API.Response;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Containers.Markdown;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class MessageView : Container
{
    private static readonly Colour4 surface_message = Colour4.FromHex("23232a");

    public MessageModel Model { get; }

    private SpriteText usernameText;
    private MarkdownContainer contentText;
    private SpriteText editedMarker;

    private Box background;

    public MessageView(DmChatMessageResponse data)
    {
        Model = new MessageModel(data);

        Masking = true;
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Padding = new MarginPadding { Horizontal = 12, Vertical = 3 };
        CornerRadius = 5f;
        BorderThickness = 2f;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            background = new()
            {
                RelativeSizeAxes = Axes.Both,
                Colour = surface_message
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Margin = new(5f),
                Children =
                [
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(8, 0),
                        Children =
                        [
                            usernameText = new SpriteText
                            {
                                Font = new FontUsage(size: 15, weight: "Bold"),
                                Colour = Colour4.FromHex("e6e6eb"),
                            },
                            editedMarker = new SpriteText
                            {
                                Font = new FontUsage(size: 11),
                                Colour = Colour4.White.Opacity(0.4f),
                                Text = "(edited)",
                                Alpha = 0,
                            },
                        ],
                    },
                    contentText = new()
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Colour = Colour4.FromHex("d0d0d8"),
                        //Padding = new MarginPadding { Top = 1 },
                    },
                ],
            },
        ];
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Model.Content.BindValueChanged(c => updateContent(c.NewValue), true);
        Model.IsDeleted.BindValueChanged(d => updateDeleted(d.NewValue), true);
        Model.EditedAt.BindValueChanged(e => editedMarker.Alpha = e.NewValue.HasValue ? 1 : 0, true);

        usernameText.Text = Model.User.Username;
    }

    private void updateContent(string content)
    {
        if (Model.IsDeleted.Value)
            return;

        contentText.Text = content ?? string.Empty;
    }

    private void updateDeleted(bool deleted)
    {
        if (deleted)
        {
            contentText.Text = "[message deleted]";
            contentText.Colour = Colour4.White.Opacity(0.3f);
        }
        else
        {
            contentText.Text = Model.Content.Value ?? string.Empty;
            contentText.Colour = Colour4.FromHex("d0d0d8");
        }
    }
}
