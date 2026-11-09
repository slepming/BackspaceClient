using BackspaceClient.Game.Configuration;
using BackspaceClient.Game.Graphics.UserInterface.Containers;
using BackspaceClient.Game.Online.API;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using osuTK;

namespace BackspaceClient.Game;

public partial class MainScreen : Screen
{
    private static readonly Colour4 screen_background_colour = Colour4.FromHex("#09090B");
    private static readonly Colour4 card_background_colour = Colour4.FromHex("#121215").Opacity(0.92f);
    private static readonly Colour4 card_border_colour = Colour4.FromHex("#27272A");
    private static readonly Colour4 card_shadow_colour = Colour4.Black.Opacity(0.6f);

    private static readonly Colour4 triangles_colour = Colour4.FromHex("#27272A");
    private static readonly Colour4 triangles_error = Colour4.FromHex("#77272A");

    private AuthorizationContainer loginContainer, signUpContainer;
    private ShakerContainer shakeContainer;

    private TrianglesV2 backgroundTriangles;

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
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = screen_background_colour,
            },
            backgroundTriangles = new TrianglesV2(928375)
            {
                RelativeSizeAxes = Axes.Both,
                Colour = ColourInfo.GradientVertical(
                    triangles_colour.Opacity(0.15f),
                    triangles_colour.Opacity(0.55f)
                ),
                Velocity = 0.25f,
                SpawnRatio = 2.25f,
            },
            shakeContainer = new()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Width = 420,
                AutoSizeAxes = Axes.Y,
                AutoSizeDuration = 250f,
                AutoSizeEasing = Easing.OutQuint,
                Masking = true,
                CornerRadius = 16f,
                BorderThickness = 1.2f,
                BorderColour = card_border_colour,
                Alpha = 0,
                Scale = new Vector2(0.95f),
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Colour = card_shadow_colour,
                    Radius = 32,
                    Offset = new Vector2(0, 12),
                },
                Children =
                [
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = card_background_colour,
                    },
                    loginContainer,
                ]
            }
        ];

        loginContainer.ErrorOccurred += _ =>
        {
            shakeContainer.Shake();
            backgroundTriangles
                .FadeColour(ColourInfo.GradientVertical(
                    triangles_error.Opacity(0.15f),
                    triangles_error.Opacity(0.55f)
                ), 200, Easing.OutQuint)
                .Delay(4000)
                .FadeColour(ColourInfo.GradientVertical(
                    triangles_colour.Opacity(0.15f),
                    triangles_colour.Opacity(0.55f)
                ), 300, Easing.OutQuint);
        };
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);

        shakeContainer
            .FadeIn(350, Easing.OutQuint)
            .ScaleTo(1f, 400, Easing.OutBack);
    }
}
