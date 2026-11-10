using BackspaceClient.Game.Screens;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace BackspaceClient.Game.Tests.Visual;

[TestFixture]
public partial class TestSceneLoginScreen : BackspaceClientTestScene
{
    // Add visual tests to ensure correct behaviour of your game: https://github.com/ppy/osu-framework/wiki/Development-and-Testing
    // You can make changes to classes associated with the tests and they will recompile and update immediately.

    public TestSceneLoginScreen()
    {
        Add(new ScreenStack(new LoginScreen()) { RelativeSizeAxes = Axes.Both });
    }
}
