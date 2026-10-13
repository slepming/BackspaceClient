using BackspaceClient.Game.Online.API.Models;
using BackspaceClient.Game.Online.API.Response;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class MessageView : Container
{
    private static readonly Colour4 surface_message = Colour4.FromHex("23232a");
    private static readonly Colour4 text_normal = Colour4.FromHex("E0E0E8");
    private static readonly Colour4 text_deleted = Colour4.White.Opacity(0.4f);

    public MessageModel Model { get; }
    public bool IsConsecutive { get; }

    private SpriteText usernameText;
    private BackspaceMarkdownContainer contentText;
    private SpriteText editedMarker;
    private SpriteText replyUsernameText;
    private SpriteText replyContentText;

    private FillFlowContainer replyContainer;

    private Box background;

    public MessageView(DmChatMessageResponse data, bool isConsecutive = false)
    {
        Model = new MessageModel(data);
        IsConsecutive = isConsecutive;

        Masking = true;
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        AutoSizeDuration = 600f;
        AutoSizeEasing = Easing.OutElasticHalf;
        CornerRadius = 8f;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        var innerFlow = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Padding = new MarginPadding { Horizontal = 16, Top = IsConsecutive ? 8 : 12, Bottom = 10 },
            Spacing = new Vector2(0, 4)
        };

        innerFlow.Add(replyContainer = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(6, 0),
            Margin = new MarginPadding { Bottom = 2 },
            Alpha = 0,
            Children =
            [
                new SpriteIcon
                {
                    Icon = FontAwesome.Solid.ArrowCircleRight,
                    Size = new Vector2(12),
                    Colour = Colour4.White.Opacity(0.5f),
                    Margin = new MarginPadding { Top = 2 }
                },
                replyUsernameText = new SpriteText
                {
                    Font = new FontUsage(family: BackspaceFonts.Inter, size: 13, weight: "Bold"),
                    Colour = Colour4.White.Opacity(0.7f),
                },
                replyContentText = new SpriteText
                {
                    Font = new FontUsage(family: BackspaceFonts.Inter, size: 13),
                    Colour = Colour4.White.Opacity(0.5f),
                    Truncate = true,
                    MaxWidth = 250
                }
            ]
        });

        if (!IsConsecutive)
        {
            innerFlow.Add(new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(8, 0),
                Margin = new MarginPadding { Bottom = 2 },
                Children =
                [
                    usernameText = new SpriteText
                    {
                        Font = new FontUsage(family: BackspaceFonts.Inter, size: 15, weight: "Bold"),
                        Colour = Colour4.White,
                    },
                    editedMarker = new SpriteText
                    {
                        Font = new FontUsage(size: 12),
                        Colour = Colour4.White.Opacity(0.3f),
                        Text = "(edited)",
                        Alpha = 0,
                        Margin = new MarginPadding { Top = 2 }
                    },
                ]
            });
        }

        innerFlow.Add(contentText = new BackspaceMarkdownContainer()
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Colour = text_normal,
        });

        InternalChildren =
        [
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = surface_message
            },
            innerFlow
        ];
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Model.Content.BindValueChanged(c => updateContent(c.NewValue), true);
        Model.IsDeleted.BindValueChanged(d => updateDeleted(d.NewValue), true);
        Model.ReplyToId.BindValueChanged(rid => SetReply(rid.NewValue, ""), true);
        Model.ReplyTo.BindValueChanged(rid => SetReply(rid.NewValue), true);

        if (!IsConsecutive)
        {
            Model.EditedAt.BindValueChanged(e =>
            {
                if (!Model.IsDeleted.Value)
                    editedMarker.Alpha = e.NewValue.HasValue ? 1 : 0;
            }, true);

            usernameText.Text = Model.User.Username;
        }
    }

    public void SetReply(string username, string messageSnippet)
    {
        if (username == null || messageSnippet == null) return;
        replyUsernameText.Text = username;
        replyContentText.Text = messageSnippet;
        replyContainer.Show();
    }

    public void SetReply(DmChatMessageResponse replyData)
    {
        if (replyData == null)
            return;

        replyUsernameText.Text = replyData.User?.Username ?? "Unknown";
        replyContentText.Text = replyData.Content ?? string.Empty;
        replyContainer.Show();
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
            contentText.Text = "**This message was deleted**";
            contentText.Colour = text_deleted;

            if (!IsConsecutive)
                editedMarker.Alpha = 0;
        }
        else
        {
            contentText.Text = Model.Content.Value ?? string.Empty;
            contentText.Colour = text_normal;

            if (!IsConsecutive)
                editedMarker.Alpha = Model.EditedAt.Value.HasValue ? 1 : 0;
        }
    }
}
