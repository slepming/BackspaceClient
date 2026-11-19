using BackspaceClient.Game.Graphics.UserInterface.Containers;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Textures;

namespace BackspaceClient.Game.Tests.Visual.UserInterface;

public partial class ImageViewerTestScene : BackspaceClientTestScene
{
    private ImageViewerOverlay overlay;

    private Texture texture;

    [BackgroundDependencyLoader]
    private void load(TextureStore textureStore)
    {
        texture = textureStore.Get("logo.png");
    }

    [SetUp]
    public void Setup()
    {
        Schedule(() =>
        {
            Clear();
            overlay = new();
            Add(overlay);
        });
    }

    [Test]
    public void Manipulations()
    {
        AddStep("Toggle overlay", () => overlay.ShowImage(texture));
    }
}
