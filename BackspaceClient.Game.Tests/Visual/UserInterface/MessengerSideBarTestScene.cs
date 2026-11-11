using System;
using System.Linq;
using BackspaceClient.Game.Graphics.UserInterface;
using BackspaceClient.Game.Online.API.Response;
using NUnit.Framework;
using osu.Framework.Graphics;
using osuTK;

namespace BackspaceClient.Game.Tests.Visual.UserInterface;

public partial class MessengerSideBarTestScene : BackspaceClientTestScene
{
    private MessengerSideBar sideBar;

    protected readonly Random Random = new();

    [SetUp]
    public void Setup()
    {
        Scheduler.Add(() =>
        {
            Clear();
            sideBar = new()
            {
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(.2f, 1f)
            };
            Add(sideBar);
            sideBar.Show();
        });
    }

    [Test]
    public void CardManipulations()
    {
        AddRepeatStep("Increment cards count", () =>
        {
            sideBar.AddOrUpdateChannel(new()
            {
                Id = RandomString(15),
                Members = [
                    new User {
                        Username = RandomString(10)
                    }
                ]
            }, null);
        }, 5);

        AddWaitStep("Delay", 5);

        AddStep("Increment unread message", () =>
        {
            foreach (var kv in sideBar.Channels)
                ++kv.Value.UnreadMessagesCount.Value;
        });

        AddWaitStep("Delay", 5);

        AddStep("Decrement unread message", () =>
        {
            foreach (var kv in sideBar.Channels)
                --kv.Value.UnreadMessagesCount.Value;
        });

        AddWaitStep("Delay", 5);

        AddStep("Hide sidebar", () => sideBar.Hide());
    }

    public string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz :?.\"";
        return new string([.. Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)])]);
    }
}
