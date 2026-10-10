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

        private BackspaceClientTextBox loginTextBox, passwordTextBox;

        private FillFlowContainer loginContainer;
        private ShakerContainer shakeContainer;


        [Resolved]
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
                loginContainer = new FillFlowContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Direction = FillDirection.Vertical,
                    RelativeSizeAxes = Axes.Both,
                    Spacing = new(30f),
                    Children =
                    [
                        new SpriteText
                        {
                            Text = "Login authorization",
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Font = new(family: BackspaceFonts.Inter, size: 64),
                        },
                        loginTextBox = new()
                        {
                            RelativeSizeAxes = Axes.Both,
                            Size = text_box_size,
                            PlaceholderText = "Login",
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            TabbableContentContainer = this,
                        },
                        passwordTextBox = new()
                        {
                            RelativeSizeAxes = Axes.Both,
                            Size = text_box_size,
                            PlaceholderText = "Password",
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            InputProperties = new TextInputProperties(TextInputType.Password),
                            TabbableContentContainer = this,
                        },
                        shakeContainer = new()
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Size = text_box_size,
                            Child = new LoginButton
                            {
                                RelativeSizeAxes = Axes.Both,
                                Text = "Apply",
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Action = () => {
                                    var login = loginTextBox.Text;
                                    var passwd = passwordTextBox.Text;

                                    try
                                    {
                                        authorize(login, passwd);
                                    }
                                    catch(Exception e)
                                    {
                                        shakeContainer.Shake();
                                        Logger.Error(e, "Login runtime error");
                                    }
                                }
                            },
                        }
                    ]
                }
            ];

            passwordTextBox.OnCommit += (s, n) =>
            {
                if (n) authorize(loginTextBox.Text, s.Text);
            };
        }

        private void authorize(string username, string password)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username)) throw new ArgumentNullException("Password or username empty");

            try
            {
                var provider = new APIProvider(username, password, instanceObserver.Current.Value);
                if (!provider.HasKey)
                    throw new ArgumentNullException("Provider don't contain api key(Might be server is unavailable)");

                stack.Push(new MessengerScreen(provider, instanceObserver));
            }
            catch(WebException webExc)
            {
                if (webExc.HResult == 401) shakeContainer.Shake(300);
            }
            catch
            {
                throw;
            }

            }
    }
}
