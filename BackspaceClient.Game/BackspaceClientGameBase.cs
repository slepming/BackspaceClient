using BackspaceClient.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osuTK;

namespace BackspaceClient.Game
{
    public partial class BackspaceClientGameBase : osu.Framework.Game
    {
        // Anything in this class is shared between the test browser and the game implementation.
        // It allows for caching global dependencies that should be accessible to tests, or changing
        // the screen scaling for all components including the test browser and framework overlays.

        protected override Container<Drawable> Content { get; }

        protected BackspaceClientGameBase()
        {
            // Ensure game and tests scale with window size and screen DPI.
            base.Content.Add(Content = new DrawSizePreservingFillContainer
            {
                // You may want to change TargetDrawSize to your "default" resolution, which will decide how things scale and position when using absolute coordinates.
                TargetDrawSize = new Vector2(1366, 768)
            });
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Resources.AddStore(new DllResourceStore(typeof(BackspaceClientResources).Assembly));
            InitializeFonts();
        }

        protected void InitializeFonts()
        {
            AddFont(Resources, @"Fonts/adwaita/adwaita");
            AddFont(Resources, @"Fonts/agave/agave_bold");
            AddFont(Resources, @"Fonts/bigblueplus/bigblueplus");
            AddFont(Resources, @"Fonts/inter/interfont");
        }
    }
}
