using BackspaceClient.Game.Configuration;
using BackspaceClient.Game.Graphics.UserInterface.Containers;
using BackspaceClient.Game.Online.API;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using osuTK;

namespace BackspaceClient.Game
{
    public partial class MainScreen : Screen
    {
        private readonly Vector2 text_box_size = new(.5f, .05f);

        private FillFlowContainer loginContainer, signUpContainer;
        private ShakerContainer shakeContainer;

        [Resolved(canBeNull: true)]
        private ScreenStack stack { get; set; }

        private InstanceObserver instanceObserver;

        [BackgroundDependencyLoader]
        private void load(InstanceConfiguration instanceConfig)
        {
            var instance = instanceConfig.Get<string>(InstanceSettings.Home);
            instanceObserver = new();
            instanceObserver.Current.Value = instance;


            loginContainer = new SignUpContainer(instanceObserver);

            InternalChildren =
            [
                new TrianglesV2
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.3f,
                },
                shakeContainer = new()
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Width = 600,
                    AutoSizeAxes = Axes.Y,
                    AutoSizeDuration = 300f,
                    AutoSizeEasing = Easing.OutCubic,
                    Masking = true,
                    CornerRadius = 12f,
                    BorderThickness = 2.5f,
                    BorderColour = Colour4.FromHex("#3E3E4E"),
                    Children =
                    [
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colour4.FromHex("#181820"),
                        },
                        loginContainer,
                    ]
                }
            ];
        }
    }
}
