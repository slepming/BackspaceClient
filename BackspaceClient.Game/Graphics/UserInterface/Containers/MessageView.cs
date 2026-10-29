using System.Collections;
using System.Linq;
using BackspaceClient.Game.Online.API.Models;
using BackspaceClient.Game.Online.API.Response;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class MessageView : Container
{
    private static readonly string[] supported_image_mimetypes = ["image/png", "image/webp", "image/jpeg"];

    private static readonly Colour4 text_normal = Colour4.FromHex("E0E0E8");
    private static readonly Colour4 text_deleted = Colour4.White.Opacity(0.4f);
    private static readonly Colour4 text_hover = Colour4.FromHex("A0A0A8");

    public MessageModel Model { get; }
    public BindableBool IsConsecutive { get; } = new();

    public string MessageId { get; set; }

    public bool IsCanBeConsecutive => Model.Attachments.Count == 0 || Model.ReplyTo.Value == null;

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
            Padding = new MarginPadding { Horizontal = 16, Top = 12, Bottom = 10 },
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
            Spacing = new(5f),
            Margin = new() { Bottom = 2 },
        });

        InternalChildren =
        [
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
        Model.Attachments.BindCollectionChanged((attachments, eventArgs) => setAttachments(eventArgs.NewItems), true);
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
            // In the future I have to handle webp manually
            case var _ when supported_image_mimetypes.Contains(attachment.Mimetype):
            {
                if (instanceUrl != null) attachmentsContainer.Add(new DelayedLoadUnloadWrapper(() => new ImageMimetypeSprite($"{instanceUrl}/api/uploads/{attachment.Filename}")) {  });

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
        contentText.FadeColour(text_hover, 300, Easing.OutCubic);
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        contentText.FadeColour(text_normal, 300, Easing.OutCubic);
        base.OnHoverLost(e);
    }
}
