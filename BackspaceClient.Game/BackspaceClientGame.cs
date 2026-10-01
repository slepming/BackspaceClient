using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace BackspaceClient.Game
{
    public partial class BackspaceClientGame : BackspaceClientGameBase
    {
        [Cached]
        protected readonly ScreenStack ScreenStack = new() { RelativeSizeAxes = Axes.Both };

        [BackgroundDependencyLoader]
        private void load()
        {
            // Add your top-level game components here.
            // A screen stack and sample screen has been provided for convenience, but you can replace it if you don't want to use screens.
            Child = ScreenStack;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            ScreenStack.Push(new MainScreen());
        }
    }
}
