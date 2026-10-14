using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK.Graphics;

namespace BackspaceClient.Game.Graphics.UserInterface;

public partial class BackspaceClientTextBox : BasicTextBox
{
    private static readonly Colour4 surface_idle = Colour4.FromHex("13131a");
    private static readonly Colour4 surface_focused = Colour4.FromHex("181822");
    private static readonly Colour4 border_idle = Colour4.White;
    private static readonly Colour4 border_focused = Colour4.FromHex("7c6cf6");
    private static readonly Colour4 text_colour = Colour4.FromHex("e6e6eb");
    private static readonly Colour4 placeholder_text_colour = Colour4.FromHex("a6a6ab");

    protected override Color4 SelectionColour => border_idle.Darken(.3f);

    protected override float CaretWidth => 4f;

    private const float corner_radius = 6f;
    private const float border_thickness = 2f;
    private const float horizontal_padding = 14f;

    public BackspaceClientTextBox()
    {
        FontSize = 35f;

        Masking = true;
        CornerRadius = corner_radius;
        BorderThickness = border_thickness;

        BackgroundUnfocused = surface_idle;
        BackgroundFocused = surface_focused;
        BackgroundCommit = surface_focused;
        BorderColour = border_idle;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        TextContainer.Padding = new MarginPadding { Horizontal = horizontal_padding };
    }

    protected override Drawable GetDrawableCharacter(char c) => new FallingDownContainer
    {
        AutoSizeAxes = Axes.Both,
        Child = new SpriteText { Text = c.ToString(), Font = new FontUsage(family: BackspaceFonts.Inter, size: FontSize), Colour = text_colour, }
    };

    protected override SpriteText CreatePlaceholder() => new FadingPlaceholderText
    {
        Colour = placeholder_text_colour,
        Font = new FontUsage(family: BackspaceFonts.Inter, italics: true),
        Anchor = Anchor.CentreLeft,
        Origin = Anchor.CentreLeft,
        X = CaretWidth,
    };

    protected override void OnFocus(FocusEvent e)
    {
        base.OnFocus(e);
        this.TransformTo(nameof(BorderColour), (ColourInfo)border_focused, 150, Easing.OutQuint);
    }

    protected override void OnFocusLost(FocusLostEvent e)
    {
        base.OnFocusLost(e);
        this.TransformTo(nameof(BorderColour), (ColourInfo)border_idle, 150, Easing.OutQuint);
    }
}
