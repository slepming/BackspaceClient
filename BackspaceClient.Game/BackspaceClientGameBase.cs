using BackspaceClient.Game.Configuration;
using BackspaceClient.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osuTK;

namespace BackspaceClient.Game
{
    public partial class BackspaceClientGameBase : osu.Framework.Game
    {
        // Anything in this class is shared between the test browser and the game implementation.
        // It allows for caching global dependencies that should be accessible to tests, or changing
        // the screen scaling for all components including the test browser and framework overlays.

        protected Storage Storage { get; private set; }
        protected override Container<Drawable> Content { get; }
        private DependencyContainer dependencies;

        protected InstanceConfiguration InstanceConfiguration;

        protected BackspaceClientGameBase()
        {
            // Ensure game and tests scale with window size and screen DPI.
            base.Content.Add(Content = new DrawSizePreservingFillContainer
            {
                // You may want to change TargetDrawSize to your "default" resolution, which will decide how things scale and position when using absolute coordinates.
                TargetDrawSize = new Vector2(1920, 1080)
            });
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Resources.AddStore(new DllResourceStore(typeof(BackspaceClientResources).Assembly));
            InstanceConfiguration = new(Storage);
            dependencies.CacheAs(InstanceConfiguration);

            InitializeFonts();
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            Storage = host.Storage;
            host.Window?.CursorState |= CursorState.Hidden;
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        protected void InitializeFonts()
        {
            AddFont(Resources, @"Fonts/adwaita/adwaita");
            AddFont(Resources, @"Fonts/agave/agave_bold");
            AddFont(Resources, @"Fonts/bigblueplus/bigblueplus");
            AddFont(Resources, @"Fonts/inter/interfont");
        }
    }
}
