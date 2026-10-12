using osu.Framework.Graphics.Containers.Markdown;
using osu.Framework.Graphics.Sprites;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class BackspaceMarkdownContainer : MarkdownContainer
{
    public FontUsage Font = new(family: BackspaceFonts.Inter);

    public override SpriteText CreateSpriteText() => new() { Font = Font };
}
