using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class ImageViewerOverlay : OverlayContainer
{
    private static readonly Colour4 dim_background_colour = Colour4.Black.Opacity(0.75f);
    private static readonly Colour4 window_background_colour = Colour4.FromHex("#121215");
    private static readonly Colour4 window_border_colour = Colour4.FromHex("#27272A");
    private static readonly Colour4 window_shadow_colour = Colour4.Black.Opacity(0.65f);

    private const float window_corner_radius = 12f;
    private const float window_border_thickness = 1.2f;

    private WindowContainer windowContainer;

    public ImageViewerOverlay()
    {
        RelativeSizeAxes = Axes.Both;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = dim_background_colour,
            },
            windowContainer = new WindowContainer
            {
                OnClose = Hide,
            }
        ];
    }

    public void ShowImage(Texture texture)
    {
        if (texture == null)
            return;

        windowContainer.SetImage(texture);
        windowContainer.Position = Vector2.Zero;
        Show();
    }

    protected override void PopIn()
    {
        this.FadeIn(220, Easing.OutQuint);
        windowContainer.ScaleTo(1f, 280, Easing.OutBack);
    }

    protected override void PopOut()
    {
        this.FadeOut(180, Easing.InQuint);
        windowContainer.ScaleTo(0.95f, 200, Easing.InQuint);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (!windowContainer.ReceivePositionalInputAt(e.ScreenSpaceMousePosition))
            Hide();

        return true;
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Key == Key.Escape)
        {
            Hide();
            return true;
        }

        return base.OnKeyDown(e);
    }

    private partial class WindowContainer : Container
    {
        private Sprite imageSprite;
        private ImageViewerTitleBar titleBar;

        public Action OnClose { get; init; }

        public WindowContainer()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
            Size = new Vector2(0.72f, 0.78f);
            Masking = true;
            CornerRadius = window_corner_radius;
            BorderThickness = window_border_thickness;
            BorderColour = window_border_colour;
            Scale = new Vector2(0.95f);

            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Shadow,
                Colour = window_shadow_colour,
                Radius = 32,
                Offset = new Vector2(0, 12),
            };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren =
            [
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = window_background_colour,
                },
                titleBar = new ImageViewerTitleBar("Image Preview", "", this)
                {
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    OnClose = () => OnClose?.Invoke(),
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding
                    {
                        Top = ImageViewerTitleBar.HEIGHT + 12,
                        Bottom = 12,
                        Horizontal = 12,
                    },
                    Child = imageSprite = new Sprite
                    {
                        RelativeSizeAxes = Axes.Both,
                        FillMode = FillMode.Fit,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    }
                }
            ];
        }

        public void SetImage(Texture texture) => imageSprite.Texture = texture;
    }
}
