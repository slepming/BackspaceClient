using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;
using static osu.Framework.Graphics.UserInterface.Menu;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class StyledContextMenuContainer : ContextMenuContainer
{
    private static readonly Colour4 menuBackgroundColour = Colour4.FromHex("#18181B");
    private static readonly Colour4 menuBorderColour = Colour4.FromHex("#27272A");
    private static readonly Colour4 menuShadowColour = Colour4.Black.Opacity(0.5f);

    private static readonly Colour4 itemBackgroundNormalColour = Colour4.Transparent;
    private static readonly Colour4 itemBackgroundHoverColour = Colour4.FromHex("#27272A");
    private static readonly Colour4 itemBackgroundPressColour = Colour4.FromHex("#3F3F46");

    private static readonly Colour4 textNormalColour = Colour4.FromHex("#E4E4E7");
    private static readonly Colour4 textHoverColour = Colour4.FromHex("#FFFFFF");
    private static readonly Colour4 textDisabledColour = Colour4.FromHex("#52525B");

    private static readonly Colour4 scrollbarColour = Colour4.FromHex("#3F3F46");

    protected override Menu CreateMenu() => new StyledContextMenu();

    public partial class StyledContextMenu : BasicMenu
    {
        private const float corner_radius = 8f;
        private const float border_thickness = 1.2f;
        private const float menu_padding = 4f;
        private const float min_menu_width = 130f;

        public StyledContextMenu()
            : base(Direction.Vertical)
        {
            BackgroundColour = menuBackgroundColour;

            MaskingContainer.CornerRadius = corner_radius;
            MaskingContainer.BorderThickness = border_thickness;
            MaskingContainer.BorderColour = menuBorderColour;

            ItemsContainer.Padding = new MarginPadding(4);
        }

        protected override void AnimateOpen()
        {
            this.FadeIn(180, Easing.OutQuint);
        }

        protected override void AnimateClose()
        {
            this.FadeOut(140, Easing.OutQuint);
        }

        protected override void UpdateSize(Vector2 newSize)
        {
            if (newSize.Y > 0)
            {
                Vector2 paddedSize = new Vector2(
                    Math.Max(min_menu_width, newSize.X + menu_padding * 2),
                    newSize.Y
                );

                this.ResizeTo(paddedSize, 220, Easing.OutQuint);
            }
            else
            {
                this.ResizeTo(new Vector2(DrawWidth, 0), 140, Easing.OutQuint);
            }
        }

        protected override Menu CreateSubMenu() => new StyledContextMenu();

        protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item) => new StyledDrawableMenuItem(item);

        protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new StyledScrollContainer(direction);
    }

    private partial class StyledDrawableMenuItem : DrawableMenuItem
    {
        private const float item_corner_radius = 5f;
        private const float text_size = 15f;

        private TextContainer text;

        public StyledDrawableMenuItem(MenuItem item)
            : base(item)
        {
            BackgroundColour = itemBackgroundNormalColour;
            BackgroundColourHover = itemBackgroundHoverColour;
            Masking = true;
            CornerRadius = item_corner_radius;

            Foreground.AutoSizeAxes = Axes.Both;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Item.Action.BindDisabledChanged(_ => updateState(), true);
        }

        protected override bool OnHover(HoverEvent e)
        {
            updateState();
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            updateState();
            base.OnHoverLost(e);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            if (!Item.Action.Disabled)
            {
                Background.FadeColour(itemBackgroundPressColour, 50, Easing.OutQuint);
                text.ScaleTo(0.98f, 50, Easing.OutQuint);
            }

            return base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            updateState();
            text.ScaleTo(1f, 150, Easing.OutQuint);
            base.OnMouseUp(e);
        }

        private void updateState()
        {
            if (Item.Action.Disabled)
            {
                Background.FadeColour(itemBackgroundNormalColour, 150, Easing.OutQuint);
                text.NormalText.FadeColour(textDisabledColour, 150, Easing.OutQuint);
                text.BoldText.FadeOut(150, Easing.OutQuint);
                return;
            }

            if (IsHovered)
            {
                Background.FadeColour(itemBackgroundHoverColour, 120, Easing.OutQuint);
                text.NormalText.FadeOut(120, Easing.OutQuint);
                text.BoldText.FadeIn(120, Easing.OutQuint);
            }
            else
            {
                Background.FadeColour(itemBackgroundNormalColour, 150, Easing.OutQuint);
                text.NormalText.FadeIn(150, Easing.OutQuint);
                text.BoldText.FadeOut(150, Easing.OutQuint);
            }
        }

        protected override Drawable CreateContent() => text = new TextContainer(text_size);

        private partial class TextContainer : Container, IHasText
        {
            public LocalisableString Text
            {
                get => NormalText.Text;
                set
                {
                    NormalText.Text = value;
                    BoldText.Text = value;
                }
            }

            public readonly SpriteText NormalText;
            public readonly SpriteText BoldText;

            public TextContainer(float fontSize)
            {
                AutoSizeAxes = Axes.Both;
                Origin = Anchor.CentreLeft;
                Anchor = Anchor.CentreLeft;
                Padding = new MarginPadding { Left = 12, Right = 24, Vertical = 7 };

                Children =
                [
                    NormalText = new SpriteText
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Font = FrameworkFont.Regular.With(size: fontSize),
                        Colour = textNormalColour,
                    },
                    BoldText = new SpriteText
                    {
                        AlwaysPresent = true,
                        Alpha = 0,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Font = FrameworkFont.Regular.With(size: fontSize, weight: "Bold"),
                        Colour = textHoverColour,
                    }
                ];
            }
        }
    }

    private partial class StyledScrollContainer : BasicScrollContainer
    {
        public StyledScrollContainer(Direction direction)
            : base(direction)
        {
            ScrollbarVisible = false;
        }
    }
}
