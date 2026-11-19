using System;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class AttachmentPreviewCard : CompositeDrawable
{
    private static readonly Colour4 card_background_colour = Colour4.FromHex("#18181B");
    private static readonly Colour4 card_border_colour = Colour4.FromHex("#27272A");
    private static readonly Colour4 card_border_hover_colour = Colour4.FromHex("#3F3F46");
    private static readonly Colour4 card_shadow_colour = Colour4.Black.Opacity(0.35f);

    private static readonly Colour4 close_bg_idle_colour = Colour4.FromHex("#09090B").Opacity(0.82f);
    private static readonly Colour4 close_bg_hover_colour = Colour4.FromHex("#EF4444");
    private static readonly Colour4 close_bg_press_colour = Colour4.FromHex("#DC2626");
    private static readonly Colour4 close_border_colour = Colour4.FromHex("#3F3F46");
    private static readonly Colour4 close_icon_colour = Colour4.FromHex("#F4F4F5");

    private const float card_size = 84f;
    private const float corner_radius = 10f;
    private const float border_thickness = 1.2f;

    [Resolved]
    private IRenderer renderer { get; set; }

    public Action OnRemove { get; init; }

    private readonly Image<Rgba32> sourceImage;
    private Container cardContainer;
    private Sprite previewSprite;

    public AttachmentPreviewCard(Image<Rgba32> image)
    {
        sourceImage = image;

        Size = new Vector2(card_size);
        Alpha = 0f;
        Scale = new Vector2(0.88f);
    }

    public static Texture CreateTextureFromImage(IRenderer renderer, Image<Rgba32> image)
    {
        var texture = renderer.CreateTexture(image.Width, image.Height);
        texture.SetData(new TextureUpload(image));
        return texture;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Texture previewTexture = sourceImage != null ? CreateTextureFromImage(renderer, sourceImage) : null;

        InternalChild = cardContainer = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Masking = true,
            CornerRadius = corner_radius,
            BorderThickness = border_thickness,
            BorderColour = card_border_colour,
            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Shadow,
                Colour = card_shadow_colour,
                Radius = 10,
                Offset = new Vector2(0, 3),
            },
            Children =
            [
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = card_background_colour,
                },
                previewSprite = new Sprite
                {
                    RelativeSizeAxes = Axes.Both,
                    FillMode = FillMode.Fill,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Texture = previewTexture,
                },
                new RemoveAttachmentButton
                {
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                    Margin = new MarginPadding(6),
                    Action = () => OnRemove?.Invoke(),
                }
            ]
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        this.FadeIn(200, Easing.OutQuint);
        this.ScaleTo(1f, 260, Easing.OutBack);
    }

    protected override bool OnHover(HoverEvent e)
    {
        cardContainer.TransformTo(nameof(BorderColour), (ColourInfo)card_border_hover_colour, 150, Easing.OutQuint);
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        cardContainer.TransformTo(nameof(BorderColour), (ColourInfo)card_border_colour, 150, Easing.OutQuint);
        base.OnHoverLost(e);
    }

    private partial class RemoveAttachmentButton : ClickableContainer
    {
        private Box backgroundBox;
        private SpriteIcon closeIcon;

        public RemoveAttachmentButton()
        {
            Size = new Vector2(22f);
            Masking = true;
            CornerRadius = 11f;
            BorderThickness = 1f;
            BorderColour = close_border_colour;

            Children =
            [
                backgroundBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = close_bg_idle_colour,
                },
                closeIcon = new SpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome.Solid.Times,
                    Size = new Vector2(10f),
                    Colour = close_icon_colour,
                }
            ];
        }

        protected override bool OnHover(HoverEvent e)
        {
            backgroundBox.FadeColour(close_bg_hover_colour, 120, Easing.OutQuint);
            this.TransformTo(nameof(BorderColour), (ColourInfo)close_bg_hover_colour, 120, Easing.OutQuint);
            closeIcon.ScaleTo(1.1f, 150, Easing.OutBack);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            backgroundBox.FadeColour(close_bg_idle_colour, 150, Easing.OutQuint);
            this.TransformTo(nameof(BorderColour), (ColourInfo)close_border_colour, 150, Easing.OutQuint);
            closeIcon.ScaleTo(1f, 150, Easing.OutQuint);
            base.OnHoverLost(e);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            backgroundBox.FadeColour(close_bg_press_colour, 50, Easing.OutQuint);
            this.ScaleTo(0.9f, 50, Easing.OutQuint);
            return base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            this.ScaleTo(1f, 160, Easing.OutBack);
            backgroundBox.FadeColour(IsHovered ? close_bg_hover_colour : close_bg_idle_colour, 120, Easing.OutQuint);
            base.OnMouseUp(e);
        }
    }
}
