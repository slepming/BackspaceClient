using System;
using System.Collections.Generic;
using BackspaceClient.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Framework.Logging;
using osuTK;

public partial class UserCardContainer : Container, IFilterable
{
    public string Id { get; set; } = string.Empty;

    public bool MatchingFilter
    {
        set
        {
            if (value)
                Show();
            else
                Hide();
        }
    }

    public bool FilteringActive { set; get; }

    public IEnumerable<LocalisableString> FilterTerms => [Username.Value, DisplayName.Value, LastMessage.Value];

    public Bindable<string> Username = new();
    public Bindable<string> DisplayName = new();
    public Bindable<string> AvatarUrl = new();
    public Bindable<string> Status = new();
    public Bindable<string> AvatarColor = new();
    public Bindable<string> LastMessage = new();
    public Bindable<string> LastMessageTime = new();
    public BindableBool Selected = new();
    public readonly BindableBool Enabled = new();

    public Action<string, BindableBool> Action
    {
        get;
        set
        {
            field = value;
            Enabled.Value = field != null;
        }
    }

    private Box backgroundBox;
    private Box accentBar;
    private SpriteText usernameText;
    private SpriteText timestampText;
    private SpriteText previewText;

    private readonly string instanceUrl;

    public UserCardContainer(string currentInstance)
    {
        RelativeSizeAxes = Axes.Both;
        Size = new Vector2(1f, .08f);
        Padding = new MarginPadding { Vertical = 1, Horizontal = 2 };
        instanceUrl = currentInstance;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                CornerRadius = 6,
                Masking = true,
                Children =
                [
                    backgroundBox = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.White,
                        Alpha = 0,
                    },
                    accentBar = new Box
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Size = new Vector2(3, 24),
                        Colour = Colour4.White,
                        Alpha = 0,
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Horizontal = 8 },
                        Children =
                        [
                            new Container
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Size = new Vector2(32),
                                Children =
                                [
                                    new CircularContainer
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Masking = true,
                                        Child = new BindableAvatar(AvatarUrl, AvatarColor, instanceUrl) {
                                            RelativeSizeAxes = Axes.Both
                                        },
                                    },
                                    new Circle
                                    {
                                        Anchor = Anchor.BottomRight,
                                        Origin = Anchor.Centre,
                                        Size = new Vector2(10),
                                        Colour = Colour4.LimeGreen,
                                        BorderColour = Colour4.Black,
                                        BorderThickness = 1.5f,
                                        Position = new Vector2(-3, -3),
                                    },
                                ],
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Padding = new MarginPadding { Left = 44 },
                                Child = new Container
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Children =
                                    [
                                        new Container
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Height = 18,
                                            Children =
                                            [
                                                usernameText = new SpriteText
                                                {
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft,
                                                    RelativeSizeAxes = Axes.X,
                                                    Margin = new MarginPadding { Right = 60 },
                                                    Font = new FontUsage(size: 15, family: BackspaceFonts.Inter),
                                                    Colour = Colour4.White,
                                                    Truncate = true,
                                                },
                                                timestampText = new SpriteText
                                                {
                                                    Anchor = Anchor.CentreRight,
                                                    Origin = Anchor.CentreRight,
                                                    Font = new FontUsage(size: 11),
                                                    Colour = Colour4.White.Opacity(0.45f),
                                                },
                                            ],
                                        },
                                        previewText = new SpriteText
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Padding = new MarginPadding { Top = 2 },
                                            Font = new FontUsage(size: 12),
                                            Colour = Colour4.White.Opacity(0.45f),
                                            Truncate = true,
                                        },
                                    ],
                                },
                            },
                        ],
                    },
                ],
            },
        ];
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Username.BindValueChanged(u => usernameText.Text = u.NewValue, true);
        DisplayName.BindValueChanged(d => usernameText.Text = $"{Username.Value}({d.NewValue})", true);
        LastMessage.BindValueChanged(m => previewText.Text = m.NewValue, true);
        LastMessageTime.BindValueChanged(t => timestampText.Text = t.NewValue, true);
        Selected.BindValueChanged(_ => updateVisualState(), true);

        updateVisualState();
    }

    protected override bool OnHover(HoverEvent e)
    {
        updateVisualState();
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        updateVisualState();
        base.OnHoverLost(e);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (Enabled.Value)
            Action?.Invoke(Id, Selected);

        return true;
    }

    private void updateVisualState()
    {
        if (Selected.Value)
        {
            backgroundBox.FadeTo(0.10f, 100);
            accentBar.FadeTo(0.70f, 100);
        }
        else if (IsHovered)
        {
            backgroundBox.FadeTo(0.05f, 100);
            accentBar.FadeTo(0f, 100);
        }
        else
        {
            backgroundBox.FadeTo(0f, 100);
            accentBar.FadeTo(0f, 100);
        }
    }
}
