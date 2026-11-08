using System.Collections;
using System.Linq;
using BackspaceClient.Game.Online.API.Models;
using BackspaceClient.Game.Online.API.Response;
using BackspaceClient.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osuTK;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class MessageView : Container, IHasContextMenu
{
    private static readonly string[] supported_image_mimetypes = ["image/png", "image/webp", "image/jpeg"];

    private static readonly Colour4 username_colour = Colour4.FromHex("#F4F4F5");
    private static readonly Colour4 text_normal = Colour4.FromHex("#D4D4D8");
    private static readonly Colour4 text_hover = Colour4.FromHex("#FFFFFF");
    private static readonly Colour4 text_deleted = Colour4.FromHex("#52525B");
    private static readonly Colour4 edited_marker_colour = Colour4.FromHex("#71717A");

    private static readonly Colour4 reply_bar_colour = Colour4.FromHex("#3F3F46");
    private static readonly Colour4 reply_icon_colour = Colour4.FromHex("#71717A");
    private static readonly Colour4 reply_username_colour = Colour4.FromHex("#A1A1AA");
    private static readonly Colour4 reply_content_colour = Colour4.FromHex("#71717A");

    public MessageModel Model { get; }
    public BindableBool IsConsecutive { get; } = new();

    public string MessageId { get; set; }

    public bool IsCanBeConsecutive => Model.Attachments.Count == 0 || Model.ReplyTo.Value == null;

    [Resolved]
    private Clipboard clipboard { get; set; }

    public MenuItem[] ContextMenuItems =>
    [
        new("Copy", copyContentToClipboard),
    ];

    private SpriteText usernameText;
    private SpriteText editedMarker;
    private SpriteText replyUsernameText;
    private SpriteText replyContentText;

    private BackspaceMarkdownContainer contentText;

    private FillFlowContainer replyContainer;
    private FillFlowContainer attachmentsContainer;

    private readonly string instanceUrl;

    public MessageView(DmChatMessageResponse data, string instanceUrl)
    {
        Model = new MessageModel(data);
        this.instanceUrl = instanceUrl;

        Masking = true;
        CornerRadius = 10f;
        BorderThickness = 1f;

        Masking = true;
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        var innerFlow = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Padding = new MarginPadding { Horizontal = 16, Top = 8, Bottom = 8 },
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
                new Container
                {
                    Width = 2f,
                    Height = 14f,
                    Masking = true,
                    CornerRadius = 1f,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = reply_bar_colour,
                    }
                },
                new SpriteIcon
                {
                    Icon = FontAwesome.Solid.Reply,
                    Size = new Vector2(10),
                    Colour = reply_icon_colour,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                },
                replyUsernameText = new SpriteText
                {
                    Font = new FontUsage(family: BackspaceFonts.Inter, size: 13, weight: "Bold"),
                    Colour = reply_username_colour,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                },
                replyContentText = new SpriteText
                {
                    Font = new FontUsage(family: BackspaceFonts.Inter, size: 13),
                    Colour = reply_content_colour,
                    Truncate = true,
                    MaxWidth = 320,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                }
            ]
        });

        innerFlow.Add(new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(6, 0),
            Margin = new MarginPadding { Bottom = 1 },
            Children =
            [
                usernameText = new SpriteText
                {
                    Font = new FontUsage(family: BackspaceFonts.Inter, size: 15, weight: "Bold"),
                    Colour = username_colour,
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                },
                editedMarker = new SpriteText
                {
                    Font = new FontUsage(family: BackspaceFonts.Inter, size: 12),
                    Colour = edited_marker_colour,
                    Text = "(edited)",
                    Alpha = 0,
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    Margin = new MarginPadding { Bottom = 1 }
                },
            ]
        });

        innerFlow.Add(contentText = new BackspaceMarkdownContainer()
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Colour = text_normal,
        });

        innerFlow.Add(attachmentsContainer = new()
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Horizontal,
            Spacing = new(6f),
            Margin = new() { Top = 4, Bottom = 2 },
        });

        InternalChildren =
        [
            innerFlow
        ];
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Model.Content.BindValueChanged(c =>
            updateContent(c.NewValue), true);
        Model.IsDeleted.BindValueChanged(d =>
            updateDeleted(d.NewValue), true);
        Model.ReplyToId.BindValueChanged(rid =>
            SetReply(rid.NewValue, ""), true);
        Model.ReplyTo.BindValueChanged(rid =>
            SetReply(rid.NewValue), true);
        Model.Attachments.BindCollectionChanged((attachments, eventArgs) =>
            setAttachments(eventArgs.NewItems), true);
        Model.EditedAt.BindValueChanged(e =>
        {
            if (!Model.IsDeleted.Value)
                editedMarker.Alpha = e.NewValue.HasValue ? 1 : 0;
        }, true);

        usernameText.Text = Model.User.Username;
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

    private void setAttachment(Attachment attachment)
    {
        switch (attachment.Mimetype)
        {
            case var _ when supported_image_mimetypes.Contains(attachment.Mimetype):
            {
                if (instanceUrl != null) attachmentsContainer.Add(new DelayedLoadUnloadWrapper(() => new ImageMimetypeSprite($"{instanceUrl}/api/uploads/{attachment.Filename}")) { });

                break;
            }
        }
    }

    private void setAttachments(IList attachments)
    {
        if (attachments.Count <= 0)
            return;

        foreach (Attachment attachment in attachments)
            setAttachment(attachment);
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

            editedMarker.Alpha = 0;
        }
        else
        {
            contentText.Text = Model.Content.Value ?? string.Empty;
            contentText.Colour = text_normal;

            editedMarker.Alpha = Model.EditedAt.Value.HasValue ? 1 : 0;
        }
    }

    protected override bool OnHover(HoverEvent e)
    {
        if (!Model.IsDeleted.Value)
            contentText.FadeColour(text_hover, 150, Easing.OutQuint);

        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        if (!Model.IsDeleted.Value)
            contentText.FadeColour(text_normal, 200, Easing.OutQuint);

        base.OnHoverLost(e);
    }

    private void copyContentToClipboard()
    {
        clipboard.SetText(Model.Content?.Value);
    }
}
