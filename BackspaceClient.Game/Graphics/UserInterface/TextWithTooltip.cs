using BackspaceClient.Game.Graphics.UserInterface.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;

namespace BackspaceClient.Game.Graphics.UserInterface;

public partial class TextWithTooltip : SpriteText, IHasCustomTooltip
{
    public object TooltipContent => Text;

    public ITooltip GetCustomTooltip() => new AnimatedTooltip();
}
