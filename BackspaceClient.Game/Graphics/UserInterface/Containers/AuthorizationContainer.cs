using System;
using System.Threading.Tasks;
using BackspaceClient.Game.Configuration;
using BackspaceClient.Game.Graphics.UserInterface.Buttons;
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

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

/// <summary>
/// Shared base for the login and registration forms.
/// </summary>
public abstract partial class AuthorizationContainer : FillFlowContainer
{
    protected BackspaceClientTextBox LoginTextBox { get; private set; }
    protected BackspaceClientTextBox PasswordTextBox { get; private set; }
    protected BackspaceClientTextBox InstanceTextBox { get; private set; }
    protected SpriteText ErrorText { get; private set; }

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

    public AuthorizationContainer(InstanceObserver observer) => InstanceObserver = observer;
    public AuthorizationContainer() {}

    [BackgroundDependencyLoader]
    private void load(InstanceConfiguration instanceConfig)
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Padding = new MarginPadding(32);
        Spacing = new Vector2(0, 18);

        Children =
        [
            new SpriteText
            {
                Text = TitleText,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Font = new FontUsage(family: BackspaceFonts.Inter, size: 30, weight: "Bold"),
                Margin = new MarginPadding { Bottom = 6 },
            },
            CreateLabeledInput("Login", LoginTextBox = new()
            {
                RelativeSizeAxes = Axes.X,
                Height = 42,
                PlaceholderText = "Login",
                TabbableContentContainer = this,
            }),
            CreateLabeledInput("Password", PasswordTextBox = new()
            {
                RelativeSizeAxes = Axes.X,
                Height = 42,
                PlaceholderText = "Password",
                InputProperties = new TextInputProperties(TextInputType.Password),
                TabbableContentContainer = this,
            }),
            CreateLabeledInput("Instance URL", InstanceTextBox = new()
            {
                RelativeSizeAxes = Axes.X,
                Height = 42,
                FontSize = 25,
                PlaceholderText = "Example https://backspaced.org",
                TabbableContentContainer = this,
                CommitOnFocusLost = true,
                Text = instanceConfig.Get<string>(InstanceSettings.Home),
            }),
            ErrorText = new TextWithTooltip
            {
                RelativeSizeAxes = Axes.X,
                Truncate = true,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Font = new FontUsage(family: BackspaceFonts.Inter, size: 25, italics: true),
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
                Action = submit,
            },
        ];

        PasswordTextBox.OnCommit += (_, newText) =>
        {
            if (newText) submit();
        };

        InstanceTextBox.OnCommit += (instance, newText) =>
        {
            if (InstanceObserver == null) ShowError(new Exception("Instance Observer is null"));
            if (newText)
            {
                InstanceConfig.SetValue(InstanceSettings.Home, instance.Current.Value);
                InstanceObserver.Current.Value = instance.Current.Value;
            }
        };
    }

    private void submit()
    {
        if (InstanceObserver == null) ShowError(new Exception("Instance Observer is null"));

        try
        {
            ErrorText.FadeOut(150);
            Authorize(LoginTextBox.Text, PasswordTextBox.Text);
        }
        catch (Exception e)
        {
            ShowError(e);
        }
    }

    protected virtual void Authorize(string username, string password)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
            throw new ArgumentNullException("Password or username empty");

        if (InstanceObserver == null) Logger.Log("Instance observer is empty", level: LogLevel.Important);


        Task.Run(() =>
        {
            lock (InstanceObserver)
            {
                try
                {
                    var provider = new APIProvider(username, password, InstanceObserver.Current.Value);
                    if (!provider.HasKey)
                        throw new ArgumentNullException("Provider don't contain api key(Might be server is unavailable)");

                    Schedule(() => Stack?.Push(new MessengerScreen(provider, InstanceObserver)));
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
            ErrorText.ClearTransforms();
            ErrorText.FadeIn(200, Easing.OutQuint).Then().Delay(3500).FadeOut(400, Easing.InQuint);
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
                Font = new FontUsage(family: BackspaceFonts.Inter, size: 15, weight: "Bold"),
                Colour = Colour4.FromHex("#A0A0B0"),
            },
            input,
        ]
    };
}
