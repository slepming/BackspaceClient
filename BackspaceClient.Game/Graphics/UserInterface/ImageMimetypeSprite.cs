using BackspaceClient.Game.Graphics.UserInterface.Containers;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;

namespace BackspaceClient.Game.Graphics.UserInterface;

[LongRunningLoad]
public partial class ImageMimetypeSprite : Container
{
    private string url;

    [Resolved(canBeNull: true)]
    private ImageViewerOverlay imageViewerOverlay { get; set; }

    private Sprite sprite;

    public ImageMimetypeSprite(string url)
    {
        AutoSizeAxes = Axes.Both;
        this.url = url;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore store)
    {
        var texture = store.Get(url);

        Add(sprite = new Sprite { Texture = texture });
    }

    protected override bool OnClick(ClickEvent e)
    {
        imageViewerOverlay.ShowImage(sprite.Texture);

        return base.OnClick(e);
    }
}
