using System;
using System.Collections.Generic;
using BackspaceClient.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace BackspaceClient.Game.Graphics.UserInterface;

public partial class UserCardContainer : Container, IFilterable
{
    private static readonly Colour4 card_bg_colour = Colour4.FromHex("#27272A");
    private static readonly Colour4 unread_card_bg_colour = Colour4.FromHex("#3F3F46");

    private static readonly Colour4 text_colour = Colour4.FromHex("#A1A1AA");
    private static readonly Colour4 text_unread_colour = Colour4.FromHex("#F4F4F5");

    private static readonly Colour4 accent_bar_colour = Colour4.FromHex("#F4F4F5");
    private static readonly Colour4 unread_badge_bg_colour = Colour4.FromHex("#EF4444");
    private static readonly Colour4 unread_badge_text_colour = Colour4.FromHex("#FFFFFF");

    private static readonly Colour4 status_indicator_base_colour = Colour4.FromHex("#121215");
    private static readonly Colour4 status_offline_colour = Colour4.FromHex("#52525B");
    private static readonly Colour4 status_online_colour = Colour4.FromHex("#22C55E");
    private static readonly Colour4 status_dnd_colour = Colour4.FromHex("#EF4444");
    private static readonly Colour4 status_default_colour = Colour4.FromHex("#A855F7");

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
    private Container accentBarContainer;
    private Box accentBar;
    private Box unreadIndicatorBackground;

    private Circle statusIndicator;

    private SpriteText usernameText;
    private SpriteText unreadCountText;

    private CircularContainer unreadIndicatorContainer;

    private readonly string instanceUrl;

    public UserCardContainer(string currentInstance)
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        AutoSizeDuration = 300f;
        AutoSizeEasing = Easing.OutCubic;
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
                Height = 48,
                CornerRadius = 8,
                Masking = true,
                Children =
                [
                    backgroundBox = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = card_bg_colour,
                        Alpha = 0,
                    },
                    accentBarContainer = new Container
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Size = new Vector2(3, 22),
                        Masking = true,
                        CornerRadius = 1.5f,
                        Alpha = 0,
                        Child = accentBar = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = accent_bar_colour,
                        },
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Left = 10, Right = 10 },
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
                                        Child = new BindableAvatar(AvatarUrl, AvatarColor, instanceUrl)
                                        {
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
                                        BorderThickness = 2f,
                                        Position = new Vector2(-3, -3),
                                    },
                                ],
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Padding = new MarginPadding { Left = 42 },
                                Children =
                                [
                                    usernameText = new SpriteText
                                    {
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        RelativeSizeAxes = Axes.X,
                                        Padding = new MarginPadding { Right = 38 },
                                        Font = new FontUsage(size: 14, family: BackspaceFonts.Inter),
                                        Colour = text_colour,
                                        Truncate = true,
                                    },
                                    unreadIndicatorContainer = new CircularContainer
                                    {
                                        AutoSizeAxes = Axes.X,
                                        Height = 20f,
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.CentreRight,
                                        Masking = true,
                                        Alpha = 0f,
                                        Scale = new Vector2(0.8f),
                                        Children =
                                        [
                                            // Минимальная ширина 20px, чтобы для "1" был ровный круг, а для "12" или "99+" — капсула
                                            new Container
                                            {
                                                Width = 20f,
                                                Height = 20f,
                                            },
                                            unreadIndicatorBackground = new Box
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Colour = unread_badge_bg_colour,
                                            },
                                            new Container
                                            {
                                                AutoSizeAxes = Axes.X,
                                                RelativeSizeAxes = Axes.Y,
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                Padding = new MarginPadding { Horizontal = 6 },
                                                Child = unreadCountText = new SpriteText
                                                {
                                                    Font = new FontUsage(size: 11, family: BackspaceFonts.Inter, weight: "Bold"),
                                                    Colour = unread_badge_text_colour,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                },
                                            },
                                        ]
                                    }
                                ],
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
        Selected.BindValueChanged(_ => updateVisualState(), true);
        UnreadMessagesCount.BindValueChanged(c =>
        {
            var unreadCount = c.NewValue;
            if (unreadCount <= 0)
            {
                unreadIndicatorContainer.FadeTo(0f, 180, Easing.OutQuint);
                unreadIndicatorContainer.ScaleTo(0.75f, 180, Easing.OutQuint);
            }
            else
            {
                unreadCountText.Text = unreadCount > 99 ? "99+" : unreadCount.ToString();
                unreadIndicatorContainer.FadeTo(1f, 200, Easing.OutQuint);
                unreadIndicatorContainer.ScaleTo(1f, 250, Easing.OutBack);
            }

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
        bool isSelected = Selected.Value;

        Colour4 targetBgColour = hasUnread ? unread_card_bg_colour : card_bg_colour;
        Colour4 targetTextColour = (hasUnread || isSelected || IsHovered) ? text_unread_colour : text_colour;

        float targetBgAlpha;
        float targetAccentAlpha;
        float targetAccentHeight;

        if (isSelected)
        {
            targetBgAlpha = 0.85f;
            targetAccentAlpha = 1f;
            targetAccentHeight = 22f;
        }
        else if (IsHovered)
        {
            targetBgAlpha = 0.45f;
            targetAccentAlpha = hasUnread ? 0.85f : 0f;
            targetAccentHeight = hasUnread ? 14f : 8f;
        }
        else
        {
            targetBgAlpha = hasUnread ? 0.25f : 0f;
            targetAccentAlpha = hasUnread ? 0.9f : 0f;
            targetAccentHeight = 8f;
        }

        const Easing easing = Easing.OutQuint;
        const float duration = 200f;

        usernameText.Font = new FontUsage(
            size: 14,
            family: BackspaceFonts.Inter,
            weight: (hasUnread || isSelected) ? "Bold" : "Regular"
        );
        usernameText.FadeColour(targetTextColour, duration, easing);

        backgroundBox.FadeColour(targetBgColour, duration, easing);
        backgroundBox.FadeTo(targetBgAlpha, duration, easing);

        accentBarContainer.FadeTo(targetAccentAlpha, duration, easing);
        accentBarContainer.ResizeHeightTo(targetAccentHeight, duration, easing);
    }
}
