using BackspaceClient.Game.Graphics.UserInterface.Buttons;
using NUnit.Framework;

namespace BackspaceClient.Game.Tests.Visual.UserInterface;

public partial class LoginButtonTestScene : BackspaceClientTestScene
{
    private LoginButton button;

    [SetUp]
    public void Setup()
    {
        Clear();
        Schedule(() =>
        {
            Add(button = new LoginButton()
            {
                Width = 300f,
                Height = 50f,
                Text = "Hello",
            });
        });
    }
}

