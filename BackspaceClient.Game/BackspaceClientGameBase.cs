using System.Linq;
using BackspaceClient.Game.Configuration;
using BackspaceClient.Game.Graphics.UserInterface.Containers.Cursor;
using BackspaceClient.Game.Input.Bindings;
using BackspaceClient.Resources;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Input;
using osu.Framework.Input.Handlers.Mouse;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK;

namespace BackspaceClient.Game
{
    public partial class BackspaceClientGameBase : osu.Framework.Game
    {
        // Anything in this class is shared between the test browser and the game implementation.
        // It allows for caching global dependencies that should be accessible to tests, or changing
        // the screen scaling for all components including the test browser and framework overlays.

        protected Storage Storage { get; private set; }
        protected InstanceConfiguration InstanceConfiguration;

        protected override Container<Drawable> Content { get; }

        private DependencyContainer dependencies;

        protected GlobalActionContainer GlobalBindings;

        [Cached]
        protected readonly ScreenStack ScreenStack = new() { RelativeSizeAxes = Axes.Both };


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
        private void load(FrameworkConfigManager configManager)
        {
            Resources.AddStore(new DllResourceStore(typeof(BackspaceClientResources).Assembly));

            InstanceConfiguration = new(Storage);
            dependencies.CacheAs(InstanceConfiguration);

            base.Content.Add(new SafeAreaContainer()
            {
                RelativeSizeAxes = Axes.Both,
                SafeAreaOverrideEdges = Edges.None,
                Child = new DrawSizePreservingFillContainer()
                {
                    Children =
                    [
                        GlobalBindings = new(this)
                        {
                            Children =
                            [
#if !DEBUG
                                new TooltipContainer()
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Child = ScreenStack
                                },
#else
                                ScreenStack,
#endif
                                new MenuCursorContainer()
                            ]
                        },
                    ]
                }
            });

            dependencies.Cache(GlobalBindings);

            InitialiseFonts();
            InitialiseFrameworkConfig(configManager);
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            Storage = host.Storage;
            host.Window?.CursorState |= CursorState.Hidden;

            host.AvailableInputHandlers.OfType<MouseHandler>().First().UseRelativeMode.Value = false;
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        protected void InitialiseFrameworkConfig(FrameworkConfigManager configManager)
        {
            configManager.SetValue(FrameworkSetting.ExecutionMode, ExecutionMode.SingleThread);
            configManager.SetValue(FrameworkSetting.FrameSync, FrameSync.VSync);
            configManager.SetValue(FrameworkSetting.ConfineMouseMode, ConfineMouseMode.Never);
        }

        protected void InitialiseFonts()
        {
            AddFont(Resources, @"Fonts/adwaita/adwaita");
            AddFont(Resources, @"Fonts/agave/agave_bold");
            AddFont(Resources, @"Fonts/bigblueplus/bigblueplus");

            AddFont(Resources, @"Fonts/Inter/Inter-Regular");
            AddFont(Resources, @"Fonts/Inter/Inter-RegularItalic");
            AddFont(Resources, @"Fonts/Inter/Inter-Light");
            AddFont(Resources, @"Fonts/Inter/Inter-LightItalic");
            AddFont(Resources, @"Fonts/Inter/Inter-SemiBold");
            AddFont(Resources, @"Fonts/Inter/Inter-SemiBoldItalic");
            AddFont(Resources, @"Fonts/Inter/Inter-Bold");
            AddFont(Resources, @"Fonts/Inter/Inter-BoldItalic");
        }
    }
}
