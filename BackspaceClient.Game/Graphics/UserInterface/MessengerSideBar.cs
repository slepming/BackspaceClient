using System;
using System.Collections.Generic;
using System.Linq;
using BackspaceClient.Game.Extensions;
using BackspaceClient.Game.Graphics.UserInterface.Containers;
using BackspaceClient.Game.Online.API.Response;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osuTK;

namespace BackspaceClient.Game.Graphics.UserInterface;

public partial class MessengerSideBar : Container, IHandleGlobalKeyboardInput
{
    private static readonly Colour4 drawer_bg_colour = Colour4.FromHex("#121215");
    private static readonly Colour4 drawer_border_colour = Colour4.FromHex("#27272A");
    private static readonly Colour4 handle_colour = Colour4.FromHex("#52525B");
    private static readonly Colour4 search_bg_colour = Colour4.FromHex("#09090B");
    private static readonly Colour4 search_header_bg_colour = Colour4.FromHex("#121215");

    private const float drawer_width = 230f;
    private const float collapsed_peek = 8f;
    private const float expanded_x = 0f;
    private const float search_height = 36f;
    private const float search_header_height = search_height + 20f;

    public readonly Bindable<string> Selected = new();

    public IDictionary<string, UserCardContainer> Channels { get; } = new Dictionary<string, UserCardContainer>();

    /// <summary>
    /// User canonical id->channel id
    /// </summary>
    public IDictionary<string, string> Users { get; } = new Dictionary<string, string>();

    private BasicTextBox searchTextBox;
    private SearchContainer<UserCardContainer> usersFlow;

    private Container uvulaContainer;

    public MessengerSideBar()
    {
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;
        RelativeSizeAxes = Axes.Y;
        Height = 0.85f;
        Width = drawer_width;
        X = -(drawer_width - collapsed_peek);

        Masking = true;
        CornerRadius = 12f;
        BorderThickness = 1.2f;
        BorderColour = drawer_border_colour;
        EdgeEffect = new()
        {
            Type = EdgeEffectType.Shadow,
            Colour = Colour4.Black.Opacity(.6f),
            Radius = 32,
            Offset = new Vector2(0, 12),
        };
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = drawer_bg_colour,
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding
                {
                    Left = 12,
                    Right = 14,
                    Top = search_header_height,
                    Bottom = 12,
                },
                Child = new BackspaceScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = usersFlow = new SearchContainer<UserCardContainer>
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 6),
                        Padding = new MarginPadding { Top = 6 },
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = search_header_height,
                Children =
                [
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = search_header_bg_colour,
                    },
                    new Box
                    {
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        RelativeSizeAxes = Axes.X,
                        Height = 1f,
                        Colour = drawer_border_colour,
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Left = 12, Right = 14, Top = 12, Bottom = 8 },
                        Child = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Masking = true,
                            CornerRadius = 6f,
                            BorderThickness = 1f,
                            BorderColour = drawer_border_colour,
                            Children =
                            [
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = search_bg_colour,
                                },
                                searchTextBox = new SearchTextBox
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    PlaceholderText = "Search...",
                                }
                            ]
                        }
                    }
                ]
            },
            uvulaContainer = new Container
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Size = new Vector2(3, 44),
                Margin = new MarginPadding { Right = 2.5f },
                Masking = true,
                CornerRadius = 1.5f,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = handle_colour,
                }
            },
        ];

        searchTextBox.Current.BindValueChanged(v => usersFlow.SearchTerm = v.NewValue);
    }

    protected override bool OnHover(HoverEvent e)
    {
        slideBarToRight();
        return base.OnHover(e);
    }

    private void slideBarToRight()
    {
        this.MoveToX(expanded_x, 260, Easing.OutQuint);
        uvulaContainer.ResizeTo(new Vector2(3, 48));
    }

    private void slideBarToLeft()
    {
        this.MoveToX(-(drawer_width - collapsed_peek), 260, Easing.OutQuint);
        uvulaContainer.ResizeTo(new Vector2(4, 54));
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        if (!searchTextBox.HasFocus)
            slideBarToLeft();

        base.OnHoverLost(e);
    }

    /// <summary>
    /// Add user into side bar messages(aka dm channels)
    /// </summary>
    public void AddOrUpdateChannel(DmChannel data, string instance, User client = null, IDictionary<string, int> unreadCount = null)
    {
        var key = data.Id;
        if (string.IsNullOrEmpty(data.Id)) throw new ArgumentNullException("Channel id equals null");

        if (data.Members == null || data.Members.Count < 1)
        {
            if (Channels.TryGetValue(key, out var userCard)) userCard.Show();
            else throw new ArgumentNullException("Channel members less than 1 and Channels haven't this channel");

            return;
        }

        var user = data.Members.Last(u => u.CanonicalKey() != client?.CanonicalKey());

        if (!Channels.TryGetValue(key, out var card))
        {
            card = new UserCardContainer(instance)
            {
                Id = key,
                Action = (id, s) =>
                {
                    if (s.Value)
                        return;

                    s.Value = true;
                    Selected.Value = id;
                    Channels.Values.Where(c => c.Id != id).ForEach(c => c.Selected.Value = false);
                }
            };
            Users[user.CanonicalKey()] = key;
            Channels[key] = card;
            usersFlow.Add(card);
        }

        card.Username.Value = user.Username;
        card.DisplayName.Value = user.DisplayName ?? user.Username;
        card.AvatarUrl.Value = user.Avatar;
        card.Status.Value = user.Status;
        card.AvatarColor.Value = user.AvatarColor;

        if (unreadCount != null && unreadCount.TryGetValue(data.Id, out var count))
            card.UnreadMessagesCount.Value = count;

        card.Show();
    }

    public void UpdateUser(User data)
    {
        var key = data.CanonicalKey();
        if (!Users.TryGetValue(key, out var channelKey))
            throw new KeyNotFoundException($"User key with canonical id {key} not found; More information: Id={data.Id},HomeInstance={data.HomeInstance},HomeUserId={data.HomeUserId}");

        if (!Channels.TryGetValue(channelKey, out var card))
            throw new KeyNotFoundException($"Channel key with id {channelKey} not found");

        card.Username.Value = data.Username;
        card.DisplayName.Value = data.DisplayName ?? data.Username;
        card.AvatarUrl.Value = data.Avatar;
        card.Status.Value = data.Status;
        card.AvatarColor.Value = data.AvatarColor;
    }

    public void RemoveDmChannel(string key)
    {
        if (!Channels.TryGetValue(key, out var card))
            return;

        card.Hide();
    }

    public void ClearChannels()
    {
        usersFlow.Clear();
        Channels.Clear();
        Users.Clear();
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Key == osuTK.Input.Key.LControl)
            slideBarToRight();

        return base.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyUpEvent e)
    {
        if (e.Key == osuTK.Input.Key.LControl && !IsHovered && !searchTextBox.HasFocus)
            slideBarToLeft();

        base.OnKeyUp(e);
    }
}
