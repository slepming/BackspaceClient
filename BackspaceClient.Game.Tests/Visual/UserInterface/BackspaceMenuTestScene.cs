using BackspaceClient.Game.Graphics.UserInterface.Containers;
using NUnit.Framework;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Graphics;

namespace BackspaceClient.Game.Tests.Visual.UserInterface;

public partial class BackspaceMenuTestScene : BackspaceClientTestScene
{
    private Menu menu;

    [SetUp]
    public void Setup()
    {
        Schedule(() =>
        {
            Clear();
            menu = new BackspaceMenu(Direction.Vertical, true)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                State = MenuState.Open,
                Items =
                [
                    new MenuItem("Item #1")
                    {
                        Items =
                        [
                            new MenuItem("Sub-item #1"),
                            new MenuItem("Sub-item #2", () => { }),
                        ]
                    },
                    new MenuItem("Item #2"),
                ]
            };
            Add(menu);
        });
    }

    [Test]
    public void InMenuManipulations()
    {
    }
}
