using BackspaceClient.Game.Configuration;
using BackspaceClient.Game.Graphics.UserInterface.Containers;
using BackspaceClient.Game.Online.API;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;

namespace BackspaceClient.Game
{
    public partial class MainScreen : Screen
    {
        private AuthorizationContainer loginContainer, signUpContainer;
        private ShakerContainer shakeContainer;

        private InstanceObserver instanceObserver;

        [BackgroundDependencyLoader]
        private void load(InstanceConfiguration instanceConfig)
        {
            var instance = instanceConfig.Get<string>(InstanceSettings.Home);
            instanceObserver = new();
            instanceObserver.Current.Value = instance;


            loginContainer = new LoginContainer(instanceObserver);

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

            loginContainer.ErrorOccurred += _ => { shakeContainer.Shake(); };
        }
    }
}
