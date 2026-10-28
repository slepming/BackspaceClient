using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osuTK.Graphics;
using osuTK;
using osu.Framework.Input.Events;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class BackspaceMenu : BasicMenu
{
    public bool PressBlocked { get; set; }

    private static readonly Color4 bg_dark_blue = Color4Extensions.FromHex("#0B1021");
    private static readonly Color4 border_blue = Color4Extensions.FromHex("#1F305E");
    private static readonly Color4 item_normal = Color4Extensions.FromHex("#111A36");
    private static readonly Color4 item_hover = Color4Extensions.FromHex("#243B7A");
    private static readonly Color4 text_color = Color4Extensions.FromHex("#E2EAFC");
    private static readonly Color4 accent_cyan = Color4Extensions.FromHex("#4CC9F0");

    public BackspaceMenu(Direction direction, bool topLevelMenu = false)
        : base(direction, topLevelMenu)
    {
        BackgroundColour = bg_dark_blue.Opacity(0.95f);
        ScrollbarVisible = false;

        MaskingContainer.CornerRadius = 8f;
        MaskingContainer.BorderThickness = 1.5f;
        MaskingContainer.BorderColour = border_blue;

        ItemsContainer.Padding = new MarginPadding(4);
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        return PressBlocked = base.OnKeyDown(e);
    }

    protected override void UpdateSize(Vector2 newSize)
    {
        if (Direction == Direction.Vertical)
        {
            Width = newSize.X;

            if (newSize.Y > 0)
                this.ResizeHeightTo(newSize.Y, 300, Easing.OutQuint);
        }
        else
        {
            Height = newSize.Y;

            if (newSize.X > 0)
                this.ResizeWidthTo(newSize.X, 300, Easing.OutQuint);
        }
    }

    protected override void AnimateOpen()
    {
        this.FadeIn(300, Easing.OutQuint);
    }

    protected override void AnimateClose()
    {
        this.FadeOut(250);
    }

    protected override Menu CreateSubMenu() => new BackspaceMenu(Direction.Vertical)
    {
        Anchor = Direction == Direction.Horizontal ? Anchor.BottomLeft : Anchor.TopRight
    };

    protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item) =>
        new BackspaceDrawableMenuItem(item);

    private partial class BackspaceDrawableMenuItem : BasicDrawableMenuItem
    {
        private Box accentBar = null;

        public BackspaceDrawableMenuItem(MenuItem item)
            : base(item)
        {
            BackgroundColour = item_normal;
            BackgroundColourHover = item_hover;

            Masking = true;
            CornerRadius = 5f;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            AddInternal(accentBar = new Box
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                RelativeSizeAxes = Axes.Y,
                Height = 0.6f,
                Width = 3f,
                Colour = accent_cyan,
                Alpha = 0f
            });
        }

        protected override bool OnHover(HoverEvent e)
        {
            accentBar.FadeIn(200, Easing.OutQuint);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            accentBar.FadeOut(200, Easing.OutQuint);
            base.OnHoverLost(e);
        }

        protected override Drawable CreateContent() => new SpriteText
        {
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
            Padding = new MarginPadding { Horizontal = 14, Vertical = 7 },
            Colour = text_color,
            Font = FrameworkFont.Regular.With(size: 16)
        };
    }
}
