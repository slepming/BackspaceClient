using System;
using BackspaceClient.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class ImageViewerTitleBar : CompositeDrawable
{
    private static readonly Colour4 header_background_colour = Colour4.FromHex("#18181B");
    private static readonly Colour4 header_border_colour = Colour4.FromHex("#27272A");
    private static readonly Colour4 icon_colour = Colour4.FromHex("#71717A");
    private static readonly Colour4 title_colour = Colour4.FromHex("#F4F4F5");
    private static readonly Colour4 subtitle_colour = Colour4.FromHex("#71717A");

    private static readonly Colour4 triangles_colour = header_background_colour.Darken(0.3f);

    private static readonly Colour4 close_hover_bg_colour = Colour4.FromHex("#EF4444").Opacity(0.18f);
    private static readonly Colour4 close_icon_idle_colour = Colour4.FromHex("#A1A1AA");
    private static readonly Colour4 close_icon_hover_colour = Colour4.FromHex("#F87171");

    public const float HEIGHT = 42f;

    private readonly Drawable movableTarget;

    private LocalisableString title, subtitle;

    public Action OnClose { get; init; }

    public ImageViewerTitleBar(LocalisableString title, LocalisableString subtitle, Drawable movableTarget)
    {
        this.movableTarget = movableTarget;

        this.title = title;
        this.subtitle = subtitle;

        RelativeSizeAxes = Axes.X;
        Height = HEIGHT;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = header_background_colour,
            },
            new TrianglesV2
            {
                RelativeSizeAxes = Axes.Both,
                Colour = triangles_colour,
                Velocity = 0.25f,
                SpawnRatio = 0.3f,
            },
            new Box
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                RelativeSizeAxes = Axes.X,
                Height = 1f,
                Colour = header_border_colour,
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Y,
                AutoSizeAxes = Axes.X,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(8, 0),
                Padding = new MarginPadding { Left = 14 },
                Children =
                [
                    new SpriteIcon
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Size = new Vector2(14),
                        Icon = FontAwesome.Solid.Image,
                        Colour = icon_colour,
                    },
                    new SpriteText
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Text = title,
                        Font = new FontUsage(family: BackspaceFonts.Inter, size: 14, weight: "Bold"),
                        Colour = title_colour,
                    },
                    new SpriteText
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Text = subtitle,
                        Font = new FontUsage(family: BackspaceFonts.Inter, size: 12),
                        Colour = subtitle_colour,
                    },
                ]
            },
            new HeaderCloseButton
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Margin = new MarginPadding { Right = 8 },
                Action = () => OnClose?.Invoke(),
            }
        ];
    }

    protected override bool OnDragStart(DragStartEvent e) => true;

    protected override void OnDrag(DragEvent e)
    {
        movableTarget.Position += e.Delta;
        base.OnDrag(e);
    }

    protected override bool OnMouseDown(MouseDownEvent e) => true;

    private partial class HeaderCloseButton : ClickableContainer
    {
        private Box hoverBox;
        private SpriteIcon icon;

        public HeaderCloseButton()
        {
            Size = new Vector2(28f);
            Masking = true;
            CornerRadius = 6f;

            Children =
            [
                hoverBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = close_hover_bg_colour,
                    Alpha = 0f,
                },
                icon = new SpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome.Solid.Times,
                    Size = new Vector2(12f),
                    Colour = close_icon_idle_colour,
                }
            ];
        }

        protected override bool OnHover(HoverEvent e)
        {
            hoverBox.FadeIn(120, Easing.OutQuint);
            icon.FadeColour(close_icon_hover_colour, 120, Easing.OutQuint);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hoverBox.FadeOut(150, Easing.OutQuint);
            icon.FadeColour(close_icon_idle_colour, 150, Easing.OutQuint);
            base.OnHoverLost(e);
        }
    }
}
