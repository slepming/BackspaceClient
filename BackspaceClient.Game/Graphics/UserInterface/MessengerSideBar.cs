using System;
using System.Collections.Generic;
using System.Linq;
using BackspaceClient.Game.Extensions;
using BackspaceClient.Game.Online.API.Response;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace BackspaceClient.Game.Graphics.UserInterface;

public partial class MessengerSideBar : Container
{
    private static readonly Vector2 sidebar_size = new(0.2f, 1f);

    // TODO: In the future I have to create theme creating
    private static readonly Colour4 surface_channel = Colour4.FromHex("1a1a23");
    private static readonly Colour4 surface_input = Colour4.White.Opacity(0.03f);
    private static readonly Colour4 border_hard = Colour4.White.Opacity(0.06f);
    private static readonly Colour4 text_tertiary = Colour4.White.Opacity(0.45f);

    private const float search_bar_height = 48;
    private const float section_header_height = 32;
    private const float cards_bottom_padding = 78;
    private const float horizontal_gutter = 8;

    public string DmSectionTitle { get; } = "Direct Messages";

    public IDictionary<string, UserCardContainer> Channels { get; } = new Dictionary<string, UserCardContainer>();

    private Box backgroundBox;
    private Container searchBarContainer;
    private Container searchInputContainer;
    private BasicTextBox searchTextBox;
    private Box searchDivider;
    private Container sectionHeaderContainer;
    private SpriteText sectionTitleText;
    private Container cardsAreaContainer;
    private FillFlowContainer cardsContainer;

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
            searchBarContainer = new Container
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                RelativeSizeAxes = Axes.X,
                Height = search_bar_height,
                Padding = new MarginPadding { Horizontal = 10, Vertical = 8 },
                Child = searchInputContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 4,
                    BorderThickness = 1,
                    BorderColour = border_hard,
                    Children =
                    [
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = surface_input,
                        },
                        searchTextBox = new SearchTextBox
                        {
                            RelativeSizeAxes = Axes.Both,
                            PlaceholderText = "Find or start conversation",
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
                Colour = border_hard,
            },
            sectionHeaderContainer = new Container
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                RelativeSizeAxes = Axes.X,
                Height = section_header_height,
                Position = new Vector2(0, search_bar_height),
                Padding = new MarginPadding { Horizontal = 16 },
                Child = sectionTitleText = new SpriteText
                {
                    Text = DmSectionTitle,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Font = new FontUsage(size: 14, weight: "Bold"),
                    Colour = text_tertiary,
                    Spacing = new Vector2(0.8f, 0),
                },
            },
            cardsAreaContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding
                {
                    Top = search_bar_height + section_header_height,
                    Bottom = cards_bottom_padding,
                },
                Child = cardsContainer = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding { Horizontal = horizontal_gutter },
                },
            },
        ];
    }

    /// <summary>
    /// Add user into side bar messages(aka dm channels)
    /// </summary>
    public void AddOrUpdateUser(DmChannel data)
    {
        var key = data.Id;
        if (string.IsNullOrEmpty(data.Id)) throw new ArgumentNullException("Channel id equals null");

        if (data.Members == null || data.Members.Count < 1) {
            if (Channels.TryGetValue(key, out var userCard)) userCard.Show();
            else throw new ArgumentNullException("Channel members less than 1 and Channels haven't this channel");

            return;
        }

        var user = data.Members.Last();

        if (!Channels.TryGetValue(key, out var card))
        {
            card = new UserCardContainer { Id = key };
            Channels[key] = card;
            cardsContainer.Add(card);
        }

        card.Username.Value = user.Username;
        card.DisplayName.Value = user.DisplayName ?? user?.Username;
        card.AvatarUrl.Value = user.Avatar;
        card.Status.Value = user.Status;
        card.AvatarColor.Value = user.AvatarColor;
        card.Show();
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
}
