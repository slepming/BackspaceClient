using BackspaceClient.Game.Graphics.UserInterface.Containers;
using BackspaceClient.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;

namespace BackspaceClient.Game.Graphics.UserInterface.Buttons;

public partial class LoginButton : BasicButton
{
    private static readonly Colour4 background_colour = Colour4.FromHex("#27272A");
    private static readonly Colour4 background_hover = Colour4.FromHex("#3F3F46");
    private static readonly Colour4 background_press = Colour4.FromHex("#18181B");
    private static readonly Colour4 background_flash = Colour4.FromHex("#52525B");
    private static readonly Colour4 background_disabled = Colour4.FromHex("#18181B").Opacity(0.5f);

    private static readonly Colour4 text_colour = Colour4.FromHex("#F4F4F5");
    private static readonly Colour4 text_disabled_colour = Colour4.FromHex("#52525B");

    private readonly TrianglesV2 triangles = new()
    {
        RelativeSizeAxes = Axes.Both,
        Colour = background_colour.Darken(0.3f),
        Velocity = 0.45f,
    };

    public LoginButton()
    {
        Masking = true;
        CornerRadius = 8f;
        BorderThickness = 2.2f;
        BorderColour = Colour4.Transparent;

        BackgroundColour = background_colour;
        HoverColour = Colour4.Transparent;
        FlashColour = background_flash;
        DisabledColour = background_disabled;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Add(triangles);
        Enabled.BindValueChanged(_ => updateState(), true);
    }

    protected override SpriteText CreateText() => new()
    {
        Depth = -1f,
        Origin = Anchor.Centre,
        Anchor = Anchor.Centre,
        Font = new FontUsage(family: BackspaceFonts.Inter, size: 16, weight: "Bold"),
        Colour = text_colour,
    };

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
        if (Enabled.Value)
        {
            Background.FadeColour(background_press, 60, Easing.OutQuint);
            SpriteText.ScaleTo(0.96f, 60, Easing.OutQuint);
        }

        return base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        updateState();
        SpriteText.ScaleTo(1f, 180, Easing.OutBack);
        base.OnMouseUp(e);
    }

    private void updateState()
    {
        if (!Enabled.Value)
        {
            Background.FadeColour(background_disabled, 150, Easing.OutQuint);
            SpriteText.FadeColour(text_disabled_colour, 150, Easing.OutQuint);
            return;
        }

        SpriteText.FadeColour(text_colour, 150, Easing.OutQuint);

        if (IsHovered)
        {
            Background.FadeColour(background_hover, 200, Easing.OutQuad);
            triangles.FadeColour(background_hover.Lighten(0.5f), 300, Easing.OutQuad);
        }
        else
        {
            Background.FadeColour(background_colour, 120, Easing.OutQuad);
            triangles.FadeColour(background_colour.Darken(0.3f), 180, Easing.OutQuad);
        }
    }
}
