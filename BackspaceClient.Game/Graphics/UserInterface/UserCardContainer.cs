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
using osuTK;

public partial class UserCardContainer : Container, IFilterable
{
    private static readonly Colour4 card_bg_colour = Colour4.White;
    private static readonly Colour4 unread_card_bg_colour = Colour4.FromHex("#2AECF5");
    private static readonly Colour4 text_colour = Colour4.White;
    private static readonly Colour4 status_indicator_base_colour = Colour4.Black;
    private static readonly Colour4 unread_indicator_bg_colour = new(0.9f, 0.2f, 0.2f, 1.0f);

    private static readonly Colour4 status_offline_colour = Colour4.DarkGray;
    private static readonly Colour4 status_online_colour = Colour4.LimeGreen;
    private static readonly Colour4 status_dnd_colour = Colour4.DarkRed;
    private static readonly Colour4 status_default_colour = Colour4.Purple;

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

    public Bindable<string> Username { get; } = new();
    public Bindable<string> DisplayName { get; } = new();
    public Bindable<string> AvatarUrl { get; } = new();
    public Bindable<string> Status { get; } = new();
    public Bindable<string> AvatarColor { get; } = new();
    public Bindable<string> LastMessage { get; } = new();
    public Bindable<string> LastMessageTime { get; } = new();
    public BindableBool Selected { get; } = new();
    public BindableBool Enabled { get; } = new();
    public BindableInt UnreadMessagesCount { get; } = new(0);

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
    private Box unreadIndicatorBackground;

    private Circle statusIndicator;

    private SpriteText usernameText;
    private SpriteText timestampText;
    private SpriteText previewText;
    private SpriteText unreadCountText;

    private CircularContainer unreadIndicatorContainer;

    private readonly string instanceUrl;

    public UserCardContainer(string currentInstance)
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        AutoSizeDuration = 300f;
        AutoSizeEasing = Easing.OutCubic;
        //Size = new Vector2(1f, .08f);
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
                RelativeSizeAxes = Axes.X,
                Height = 50,
                CornerRadius = 6,
                Masking = true,
                Children =
                [
                    backgroundBox = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = card_bg_colour,
                        Alpha = 0,
                    },
                    accentBar = new Box
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Size = new Vector2(3, 24),
                        Colour = card_bg_colour,
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
                                    statusIndicator = new Circle
                                    {
                                        Anchor = Anchor.BottomRight,
                                        Origin = Anchor.Centre,
                                        Size = new Vector2(10),
                                        Colour = status_indicator_base_colour,
                                        BorderColour = status_indicator_base_colour,
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
                                                    Colour = text_colour,
                                                    Truncate = true,
                                                },
                                                timestampText = new SpriteText
                                                {
                                                    Anchor = Anchor.CentreRight,
                                                    Origin = Anchor.CentreRight,
                                                    Font = new FontUsage(size: 11),
                                                    Colour = text_colour.Opacity(0.45f),
                                                },
                                                unreadIndicatorContainer = new CircularContainer
                                                {
                                                    Margin = new() { Right = 10, Left = 60 },
                                                    Size = new(30f),
                                                    Anchor = Anchor.CentreRight,
                                                    Origin = Anchor.CentreRight,
                                                    Masking = true,
                                                    Alpha = 0f,
                                                    Children =
                                                    [
                                                        unreadIndicatorBackground = new()
                                                        {
                                                            RelativeSizeAxes = Axes.Both,
                                                            Colour = unread_indicator_bg_colour.Opacity(.5f),
                                                        },
                                                        unreadCountText = new()
                                                        {
                                                            Font = new(size: 15, family: BackspaceFonts.BigBluePlusNerdFont),
                                                            Colour = unread_indicator_bg_colour,
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                        },
                                                    ]
                                                }
                                            ],
                                        },
                                        previewText = new SpriteText
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Padding = new MarginPadding { Top = 2 },
                                            Font = new FontUsage(size: 12),
                                            Colour = text_colour.Opacity(0.45f),
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

        Status.BindValueChanged(v =>
        {
            statusIndicator.Colour = v.NewValue switch
            {
                "offline" => status_offline_colour,
                "online" => status_online_colour,
                "dnd" => status_dnd_colour,
                _ => status_default_colour
            };
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Username.BindValueChanged(u => usernameText.Text = u.NewValue, true);
        DisplayName.BindValueChanged(d => usernameText.Text = $"{Username.Value}({d.NewValue})", true);
        LastMessage.BindValueChanged(m => previewText.Text = m.NewValue, true); // Now is useless
        LastMessageTime.BindValueChanged(t => timestampText.Text = t.NewValue, true); // Now is useless
        Selected.BindValueChanged(_ => updateVisualState(), true);
        UnreadMessagesCount.BindValueChanged(c => {
                var unreadCount = c.NewValue;
                if (unreadCount == 0) unreadIndicatorContainer.FadeTo(.0f, 200, Easing.OutQuad);
                else unreadIndicatorContainer.FadeTo(1f, 200, Easing.OutQuad);
                unreadCountText.Text = c.NewValue.ToString();
                updateVisualState();
        }, true);

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
        bool hasUnread = UnreadMessagesCount.Value > 0;
        Colour4 targetColour = hasUnread ? unread_card_bg_colour : card_bg_colour;

        float targetBgAlpha = 0f;
        float targetAccentAlpha = 0f;

        if (Selected.Value)
        {
            targetBgAlpha = hasUnread ? 0.15f : 0.10f;
            targetAccentAlpha = 0.70f;
        }
        else if (IsHovered)
        {
            targetBgAlpha = hasUnread ? 0.08f : 0.05f;
            targetAccentAlpha = 0f;
        }
        else
        {
            targetBgAlpha = hasUnread ? 0.06f : 0f;
            targetAccentAlpha = 0f;
        }

        var easing = Easing.OutCubic;
        var duration = 200f;

        backgroundBox.FadeColour(targetColour, duration, easing);
        accentBar.FadeColour(targetColour, duration, easing);
        backgroundBox.FadeTo(targetBgAlpha, duration, easing);
        accentBar.FadeTo(targetAccentAlpha, duration, easing);
    }
}
