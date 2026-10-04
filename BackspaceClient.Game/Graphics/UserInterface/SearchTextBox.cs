using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;

namespace BackspaceClient.Game.Graphics.UserInterface;

public partial class SearchTextBox : BasicTextBox
{
    public SearchTextBox()
    {
        BackgroundUnfocused = Colour4.Transparent;
        BackgroundFocused = Colour4.Transparent;
        BackgroundCommit = Colour4.Transparent;
    }
}
