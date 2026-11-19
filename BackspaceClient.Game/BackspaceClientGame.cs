using BackspaceClient.Game.Screens;
using osu.Framework.Allocation;
using osu.Framework.Screens;

namespace BackspaceClient.Game
{
    public partial class BackspaceClientGame : BackspaceClientGameBase
    {
        protected override void LoadComplete()
        {
            base.LoadComplete();

            ScreenStack.Push(new LoginScreen());
        }
    }
}
