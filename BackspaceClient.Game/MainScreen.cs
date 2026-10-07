using System;
using BackspaceClient.Game.Graphics.UserInterface;
using BackspaceClient.Game.Online.API;
using BackspaceClient.Game.Screens;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input;
using osu.Framework.Screens;
using osuTK;

namespace BackspaceClient.Game
{
    public partial class MainScreen : Screen
    {
        private readonly Vector2 text_box_size = new(.5f, .05f);
        private BackspaceClientTextBox loginTextBox, passwordTextBox;
        private FillFlowContainer loginContainer;

        [Resolved]
        private ScreenStack stack { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
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
                        new BasicButton
                        {
                            RelativeSizeAxes = Axes.Both,
                            Text = "Apply",
                            Size = text_box_size,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Action = () => {
                                var login = loginTextBox.Text;
                                var passwd = passwordTextBox.Text;

                                authorize(login, passwd);
                            }
                        },
                    ]
                }
            ];

            passwordTextBox.OnCommit += (s, n) => {
                if (n) authorize(loginTextBox.Text, s.Text);
            };
        }

        private void authorize(string username, string password)
        {
            var provider = new APIProvider(username, password);

            if (!provider.HasKey)
                throw new Exception("Provider don't contain api key(Might be server is unavailable)");

            stack.Push(new MessengerScreen(provider));
        }
    }
}
