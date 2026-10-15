using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace BackspaceClient.Game.Graphics.UserInterface;

[LongRunningLoad]
public partial class ImageMimetypeSprite : Container
{
    private string url;
    public ImageMimetypeSprite(string url)
    {
        AutoSizeAxes = Axes.Both;
        this.url = url;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore store)
    {
        var texture = store.Get(url);

        Add(new Sprite { Texture = texture });
    }
}
