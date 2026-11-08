using System;
using System.Threading.Tasks;
using BackspaceClient.Game.Configuration;
using BackspaceClient.Game.Graphics.UserInterface.Buttons;
using BackspaceClient.Game.Online.API;
using BackspaceClient.Game.Screens;
using BackspaceClient.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

/// <summary>
/// Shared base for the login and registration forms.
/// </summary>
public abstract partial class AuthorizationContainer : FillFlowContainer
{
    private static readonly Colour4 title_colour = Colour4.FromHex("#F4F4F5");
    private static readonly Colour4 subtitle_colour = Colour4.FromHex("#71717A");
    private static readonly Colour4 label_colour = Colour4.FromHex("#A1A1AA");
    private static readonly Colour4 divider_colour = Colour4.FromHex("#27272A");

    private static readonly Colour4 error_banner_background_colour = Colour4.FromHex("#EF4444").Opacity(0.12f);
    private static readonly Colour4 error_banner_border_colour = Colour4.FromHex("#EF4444").Opacity(0.35f);
    private static readonly Colour4 error_text_colour = Colour4.FromHex("#F87171");

    protected BackspaceClientTextBox LoginTextBox { get; private set; }
    protected BackspaceClientTextBox PasswordTextBox { get; private set; }
    protected BackspaceClientTextBox InstanceTextBox { get; private set; }

    protected Button FinalButton { get; private set; }

    protected SpriteText ErrorText { get; private set; }

    private Container errorBannerContainer;

    [Resolved]
    protected InstanceConfiguration InstanceConfig { get; private set; }

    protected InstanceObserver InstanceObserver { get; private set; }

    [Resolved(canBeNull: true)]
    protected ScreenStack Stack { get; private set; }

    /// <summary>
    /// Raised on the update thread whenever an error occurs.
    /// </summary>
    public event Action<Exception> ErrorOccurred;

    protected abstract string TitleText { get; }

    protected virtual string SubtitleText => "Enter your credentials to connect to the server";

    public AuthorizationContainer(InstanceObserver observer) => InstanceObserver = observer;
    public AuthorizationContainer() { }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Padding = new MarginPadding { Horizontal = 36, Vertical = 36 };
        Spacing = new Vector2(0, 16);

        FinalButton = new LoginButton
        {
            RelativeSizeAxes = Axes.X,
            Height = 42,
            Text = "Sign In",
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre,
            Margin = new MarginPadding { Top = 4 },
        };

        var instanceField = CreateInstanceField();
        var loginField = CreateLoginField();
        var passwordField = CreatePasswordField();

        errorBannerContainer = new Container
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Masking = true,
            CornerRadius = 8f,
            BorderThickness = 1f,
            BorderColour = error_banner_border_colour,
            Alpha = 0,
            Children =
            [
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = error_banner_background_colour,
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new MarginPadding { Horizontal = 12, Vertical = 9 },
                    Child = ErrorText = new TextWithTooltip
                    {
                        RelativeSizeAxes = Axes.X,
                        Truncate = true,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Font = new FontUsage(family: BackspaceFonts.Inter, size: 14),
                        Colour = error_text_colour,
                    }
                }
            ]
        };

        Children =
        [
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 6),
                Margin = new MarginPadding { Bottom = 6 },
                Children =
                [
                    new SpriteText
                    {
                        Text = TitleText,
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft,
                        Font = new FontUsage(family: BackspaceFonts.Inter, size: 26, weight: "Bold"),
                        Colour = title_colour,
                    },
                    new SpriteText
                    {
                        Text = SubtitleText,
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft,
                        Font = new FontUsage(family: BackspaceFonts.Inter, size: 16),
                        Colour = subtitle_colour,
                    }
                ]
            },
            loginField,
            passwordField,
            new Box // DIVIDER
            {
                RelativeSizeAxes = Axes.X,
                Height = 1,
                Colour = divider_colour,
                Margin = new MarginPadding { Vertical = 2 },
            },
            instanceField,
            errorBannerContainer,
            FinalButton,
        ];
    }

    protected void HideErrorContainer() => errorBannerContainer.FadeOut(150, Easing.OutQuint);
    protected void ShowErrorContainer() => errorBannerContainer.FadeIn(150, Easing.OutQuint);

    protected virtual CompositeDrawable CreateLoginField() => CreateLabeledInput("USERNAME", LoginTextBox = new()
    {
        RelativeSizeAxes = Axes.X,
        Height = 40,
        FontSize = 24,
        PlaceholderText = "Enter your username",
        TabbableContentContainer = this,
    });

    protected virtual CompositeDrawable CreatePasswordField() => CreateLabeledInput("PASSWORD", PasswordTextBox = new()
    {
        RelativeSizeAxes = Axes.X,
        Height = 40,
        FontSize = 24,
        PlaceholderText = "********",
        InputProperties = new TextInputProperties(TextInputType.Password),
        TabbableContentContainer = this,
    });

    protected virtual CompositeDrawable CreateInstanceField() => CreateLabeledInput("INSTANCE SERVER", InstanceTextBox = new()
    {
        RelativeSizeAxes = Axes.X,
        Height = 38,
        FontSize = 24,
        PlaceholderText = "https://backspaced.org",
        TabbableContentContainer = this,
        CommitOnFocusLost = true,
        Text = InstanceConfig.Get<string>(InstanceSettings.Home),
    });

    protected virtual void Authorize(string username, string password)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
            throw new ArgumentNullException("Username or password cannot be empty");

        if (InstanceObserver == null) Logger.Log("Instance observer is empty", level: LogLevel.Important);

        Task.Run(() =>
        {
            lock (InstanceObserver)
            {
                try
                {
                    var provider = new APIProvider(username, password, InstanceObserver.Current.Value);
                    if (!provider.HasKey)
                        throw new ArgumentNullException("Server didn't return an API key (check instance URL or credentials)");

                    Schedule(() =>
                    {
                        if (Stack is null)
                        {
                            Logger.Log("Screen stack is null", level: LogLevel.Important);
                            return;
                        }

                        Stack.Push(new MessengerScreen(provider, InstanceObserver));
                    });
                }
                catch (Exception e)
                {
                    ShowError(e);
                }
            }
        });
    }

    protected void ShowError(Exception e)
    {
        Schedule(() =>
        {
            ErrorOccurred?.Invoke(e);
            ErrorText.Text = e.Message;
            errorBannerContainer.ClearTransforms();
            errorBannerContainer
                .FadeIn(200, Easing.OutQuint)
                .Then()
                .Delay(4000)
                .FadeOut(300, Easing.InQuint);

            Logger.Error(e, "Authorization runtime error");
        });
    }

    protected static FillFlowContainer CreateLabeledInput(string label, Drawable input) => new()
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
                Font = new FontUsage(family: BackspaceFonts.Inter, size: 12, weight: "Bold"),
                Colour = label_colour,
                Spacing = new Vector2(0.8f, 0),
            },
            input,
        ]
    };
}
