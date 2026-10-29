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
        });
    }

    [Test]
    public void CardManipulations()
    {
        AddStep("Increment cards count", () =>
        {
            for (int i = 0; i <= 5; ++i)
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
            }
        });

        AddStep("Increment unread message", () =>
        {
            foreach(var kv in sideBar.Channels)
                ++kv.Value.UnreadMessagesCount.Value;
        });

        AddStep("Decrement unread message", () =>
        {
            foreach(var kv in sideBar.Channels)
                --kv.Value.UnreadMessagesCount.Value;
        });
    }

    public string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz :?.\"";
        return new string([.. Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)])]);
    }
}
