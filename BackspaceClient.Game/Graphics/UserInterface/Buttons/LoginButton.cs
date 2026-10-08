using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;

namespace BackspaceClient.Game.Graphics.UserInterface.Buttons;

public partial class LoginButton : BasicButton
{
    private static readonly Colour4 background_colour = Colour4.FromHex("3a3a3a");
    private static readonly Colour4 background_hover = background_colour.Darken(.5f);
    private static readonly Colour4 background_flash = background_colour.Lighten(.5f);
    private static readonly Colour4 background_disabled = Colour4.FromHex("0a0a0a");

    public LoginButton()
    {
        Masking = true;
        Enabled.Value = true;
        BorderColour = Colour4.White;
        BorderThickness = 2f;
        BackgroundColour = background_colour;
        HoverColour = background_hover;
        FlashColour = background_flash;
        DisabledColour = background_disabled;
        CornerRadius = 5f;
    }

    protected override SpriteText CreateText()
    {
        return new SpriteText
        {
            Depth = -1f,
            Origin = Anchor.Centre,
            Anchor = Anchor.Centre,
            Font = new FontUsage(family: BackspaceFonts.BigBluePlusNerdFont),
            Colour = Colour4.White,
        };
    }
}
