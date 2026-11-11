using BackspaceClient.Game.Utils;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;

namespace BackspaceClient.Game.Graphics.UserInterface;

public partial class SearchTextBox : BasicTextBox
{
    public SearchTextBox()
    {
        BackgroundUnfocused = Colour4.Transparent;
        BackgroundFocused = Colour4.Transparent;
        BackgroundCommit = Colour4.White.Opacity(.1f);
        FontSize = 20f;
        Masking = true;
        CornerRadius = 6f;
    }

    protected override SpriteText CreatePlaceholder()
    {
        return new FadingPlaceholderText
        {
            Colour = Colour4.WhiteSmoke.Darken(.3f),
            Font = new FontUsage(family: BackspaceFonts.Inter),
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
            X = CaretWidth
        };
    }
}
