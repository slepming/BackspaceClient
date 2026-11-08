using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace BackspaceClient.Game.Graphics.UserInterface;

public partial class BackspaceClientTextBox : BasicTextBox
{
    private static readonly Colour4 surface_idle = Colour4.FromHex("#121215");
    private static readonly Colour4 surface_focused = Colour4.FromHex("#18181B");
    private static readonly Colour4 surface_commit = Colour4.FromHex("#1F1F23");

    private static readonly Colour4 border_idle = Colour4.FromHex("#27272A");
    private static readonly Colour4 border_hover = Colour4.FromHex("#3F3F46");
    private static readonly Colour4 border_focused = Colour4.FromHex("#71717A");

    private static readonly Colour4 text_colour = Colour4.FromHex("#F4F4F5");
    private static readonly Colour4 placeholder_text_colour = Colour4.FromHex("#71717A");
    private static readonly Colour4 selection_colour = Colour4.FromHex("#3F3F46").Opacity(0.65f);
    private static readonly Colour4 shadow_colour = Colour4.Black.Opacity(0.25f);

    protected override Color4 SelectionColour => selection_colour;

    protected override float CaretWidth => 2f;

    private const float corner_radius = 8f;
    private const float border_thickness = 1.2f;
    private const float horizontal_padding = 14f;

    public BackspaceClientTextBox()
    {
        FontSize = 16f;

        Masking = true;
        CornerRadius = corner_radius;
        BorderThickness = border_thickness;

        BackgroundUnfocused = surface_idle;
        BackgroundFocused = surface_focused;
        BackgroundCommit = surface_commit;
        BorderColour = border_idle;

        EdgeEffect = new EdgeEffectParameters
        {
            Type = EdgeEffectType.Shadow,
            Colour = shadow_colour,
            Radius = 8,
            Offset = new Vector2(0, 2),
        };
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        TextContainer.Padding = new MarginPadding { Horizontal = horizontal_padding };
    }

    protected override Drawable GetDrawableCharacter(char c) => new FallingDownContainer
    {
        AutoSizeAxes = Axes.Both,
        Child = new SpriteText
        {
            Text = c.ToString(),
            Font = new FontUsage(family: BackspaceFonts.Inter, size: FontSize),
            Colour = text_colour,
        }
    };

    protected override SpriteText CreatePlaceholder() => new FadingPlaceholderText
    {
        Colour = placeholder_text_colour,
        Font = new FontUsage(family: BackspaceFonts.Inter),
        Anchor = Anchor.CentreLeft,
        Origin = Anchor.CentreLeft,
        X = CaretWidth,
    };

    protected override bool OnHover(HoverEvent e)
    {
        if (!HasFocus)
            this.TransformTo(nameof(BorderColour), (ColourInfo)border_hover, 150, Easing.OutQuint);

        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        if (!HasFocus)
            this.TransformTo(nameof(BorderColour), (ColourInfo)border_idle, 150, Easing.OutQuint);

        base.OnHoverLost(e);
    }

    protected override void OnFocus(FocusEvent e)
    {
        base.OnFocus(e);
        this.TransformTo(nameof(BorderColour), (ColourInfo)border_focused, 150, Easing.OutQuint);
    }

    protected override void OnFocusLost(FocusLostEvent e)
    {
        base.OnFocusLost(e);
        var targetBorder = IsHovered ? border_hover : border_idle;
        this.TransformTo(nameof(BorderColour), (ColourInfo)targetBorder, 150, Easing.OutQuint);
    }
}
