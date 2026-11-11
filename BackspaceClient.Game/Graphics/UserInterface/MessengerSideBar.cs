using System;
using System.Collections.Generic;
using System.Linq;
using BackspaceClient.Game.Extensions;
using BackspaceClient.Game.Online.API.Response;
using BackspaceClient.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace BackspaceClient.Game.Graphics.UserInterface;

public partial class MessengerSideBar : OverlayContainer
{
    private static readonly Vector2 sidebar_size = new(0.2f, 1f);

    // TODO: In the future I have to create theme management
    private static readonly Colour4 surface_channel = Colour4.FromHex("#121215");
    private static readonly Colour4 surface_input = Colour4.FromHex("#09090B");
    private static readonly Colour4 border_divider = Colour4.FromHex("#1F1F23");
    private static readonly Colour4 border_input = Colour4.FromHex("#27272A");
    private static readonly Colour4 border_input_focused = Colour4.FromHex("#52525B");
    private static readonly Colour4 text_tertiary = Colour4.FromHex("#71717A");
    private static readonly Colour4 icon_search = Colour4.FromHex("#52525B");

    private const float search_bar_height = 52;
    private const float section_header_height = 34;
    private const float cards_bottom_padding = 78;
    private const float horizontal_gutter = 10;

    public string DmSectionTitle { get; } = "Direct Messages";

    public readonly Bindable<string> Selected = new();

    public IDictionary<string, UserCardContainer> Channels { get; } = new Dictionary<string, UserCardContainer>();
    /// <summary>
    /// User canonical id->channel id
    /// </summary>
    public IDictionary<string, string> Users { get; } = new Dictionary<string, string>();

    private Box backgroundBox;
    private Container searchBarContainer;
    private Container searchInputContainer;
    private BasicTextBox searchTextBox;
    private Box searchDivider;
    private Container sectionHeaderContainer;
    private SpriteText sectionTitleText;
    private Container cardsAreaContainer;
    private SearchContainer cardsContainer;

    public MessengerSideBar()
    {
        RelativeSizeAxes = Axes.Both;
        Size = sidebar_size;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            backgroundBox = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = surface_channel,
            },
            new Box
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                RelativeSizeAxes = Axes.Y,
                Width = 1f,
                Colour = border_divider,
            },
            searchBarContainer = new Container
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                RelativeSizeAxes = Axes.X,
                Height = search_bar_height,
                Padding = new MarginPadding { Horizontal = 12, Vertical = 10 },
                Child = searchInputContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 6f,
                    BorderThickness = 1.2f,
                    BorderColour = border_input,
                    Children =
                    [
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = surface_input,
                        },
                        new SpriteIcon
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Icon = FontAwesome.Solid.Search,
                            Size = new Vector2(12),
                            Colour = icon_search,
                            Margin = new MarginPadding { Left = 10 },
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Left = 22 },
                            Child = searchTextBox = new SearchTextBox
                            {
                                RelativeSizeAxes = Axes.Both,
                                PlaceholderText = "Find or start conversation",
                            },
                        },
                    ],
                },
            },
            searchDivider = new Box
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                RelativeSizeAxes = Axes.X,
                Height = 1,
                Position = new Vector2(0, search_bar_height),
                Colour = border_divider,
            },
            sectionHeaderContainer = new Container
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                RelativeSizeAxes = Axes.X,
                Height = section_header_height,
                Position = new Vector2(0, search_bar_height + 2),
                Padding = new MarginPadding { Horizontal = 14 },
                Child = sectionTitleText = new SpriteText
                {
                    Text = DmSectionTitle.ToUpperInvariant(),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Font = new FontUsage(family: BackspaceFonts.Inter, size: 11, weight: "Bold"),
                    Colour = text_tertiary,
                    Spacing = new Vector2(0.8f, 0),
                },
            },
            cardsAreaContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding
                {
                    Top = search_bar_height + section_header_height + 2,
                    Bottom = cards_bottom_padding,
                },
                Child = cardsContainer = new SearchContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(0, 4),
                    Padding = new MarginPadding { Horizontal = horizontal_gutter },
                },
            },
        ];

        searchTextBox.Current.BindValueChanged(v => cardsContainer.SearchTerm = v.NewValue);
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
                    Channels.Values.Where(c => c.Id != id).ForEach(c => c.Selected.Value = false); // lol
                }
            };
            Users[user.CanonicalKey()] = key;
            Channels[key] = card;
            cardsContainer.Add(card);
        }

        card.Username.Value = user.Username;
        card.DisplayName.Value = user.DisplayName ?? user.Username;
        card.AvatarUrl.Value = user.Avatar;
        card.Status.Value = user.Status;
        card.AvatarColor.Value = user.AvatarColor;
        if (unreadCount != null)
            if (unreadCount.TryGetValue(data.Id, out var count))
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
        cardsContainer.Clear();
        Channels.Clear();
    }

    protected override void PopIn()
    {
        this.FadeIn(250, Easing.OutQuint);
        this.MoveToX(0, 300, Easing.OutQuint);
    }

    protected override void PopOut()
    {
        this.FadeOut(200, Easing.InQuint);
        this.MoveToX(-40, 250, Easing.InQuint);
    }
}
