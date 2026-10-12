using System;
using System.Net;
using BackspaceClient.Game.Configuration;
using BackspaceClient.Game.Graphics.UserInterface;
using BackspaceClient.Game.Graphics.UserInterface.Buttons;
using BackspaceClient.Game.Graphics.UserInterface.Containers;
using BackspaceClient.Game.Online.API;
using BackspaceClient.Game.Screens;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace BackspaceClient.Game
{
    public partial class MainScreen : Screen
    {
        private readonly Vector2 text_box_size = new(.5f, .05f);

        private BackspaceClientTextBox loginTextBox, passwordTextBox, defaultInstanceTextBox;

        private FillFlowContainer loginContainer;
        private ShakerContainer shakeContainer;
        private SpriteText errorText;

        [Resolved(canBeNull: true)]
        private ScreenStack stack { get; set; }

        private InstanceObserver instanceObserver;

        [BackgroundDependencyLoader]
        private void load(InstanceConfiguration instanceConfig)
        {
            var instance = instanceConfig.Get<string>(InstanceSettings.Home);
            instanceObserver = new();
            instanceObserver.Current.Value = instance;

            InternalChildren =
            [
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
                    BorderThickness = 2f,
                    BorderColour = Colour4.FromHex("#2E2E3E"),
                    Children =
                    [
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colour4.FromHex("#181820"),
                        },
                        loginContainer = new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Padding = new MarginPadding(32),
                            Spacing = new Vector2(0, 18),
                            Children =
                            [
                                new SpriteText
                                {
                                    Text = "Login authorization",
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,
                                    Font = new FontUsage(family: BackspaceFonts.Inter, size: 30, weight: "Bold"),
                                    Margin = new MarginPadding { Bottom = 6 },
                                },
                                createLabeledInput("Login", loginTextBox = new()
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = 42,
                                    PlaceholderText = "Click to enter",
                                    TabbableContentContainer = this,
                                }),
                                createLabeledInput("Password", passwordTextBox = new()
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = 42,
                                    PlaceholderText = "Password",
                                    InputProperties = new TextInputProperties(TextInputType.Password),
                                    TabbableContentContainer = this,
                                }),
                                createLabeledInput("Instance URL", defaultInstanceTextBox = new()
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = 42,
                                    PlaceholderText = "Default instance url(with protocol)",
                                    TabbableContentContainer = this,
                                    Text = instanceConfig.Get<string>(InstanceSettings.Home)
                                }),
                                errorText = new SpriteText
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Truncate = true,
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,
                                    Font = new FontUsage(family: BackspaceFonts.Inter, size: 15),
                                    Colour = Colour4.FromHex("#FF5555"),
                                    Alpha = 0,
                                },
                                new LoginButton
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = 45,
                                    Text = "Apply",
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,
                                    Margin = new MarginPadding { Top = 6 },
                                    Action = () =>
                                    {
                                        var login = loginTextBox.Text;
                                        var passwd = passwordTextBox.Text;

                                        try
                                        {
                                            errorText.FadeOut(150);
                                            authorize(login, passwd);
                                        }
                                        catch (Exception e)
                                        {
                                            shakeContainer.Shake();
                                            errorText.Text = e.Message;
                                            errorText.ClearTransforms();
                                            errorText.FadeIn(200, Easing.OutQuint).Then().Delay(3500).FadeOut(400, Easing.InQuint);
                                            Logger.Error(e, "Login runtime error");
                                        }
                                    }
                                },
                            ]
                        }
                    ]
                }
            ];

            passwordTextBox.OnCommit += (s, n) =>
            {
                if (n) authorize(loginTextBox.Text, s.Text);
            };

            defaultInstanceTextBox.OnCommit += (instance, n) =>
            {
                if (n)
                {
                    instanceConfig.SetValue(InstanceSettings.Home, instance.Current.Value);
                    instanceObserver.Current.Value = instance.Current.Value;
                }
            };
        }

        private static FillFlowContainer createLabeledInput(string label, Drawable input) => new()
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 6),
            Children =
            [
                new SpriteText
                {
                    Text = label,
                    Font = new FontUsage(family: BackspaceFonts.Inter, size: 15, weight: "Bold"),
                    Colour = Colour4.FromHex("#A0A0B0"),
                },
                input,
            ]
        };

        private void authorize(string username, string password)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username)) throw new ArgumentNullException("Password or username empty");

            var provider = new APIProvider(username, password, instanceObserver.Current.Value);
            if (!provider.HasKey)
                throw new ArgumentNullException("Provider don't contain api key(Might be server is unavailable)");

            stack?.Push(new MessengerScreen(provider, instanceObserver));
        }
    }
}
