using System;
using System.Linq;
using BackspaceClient.Game.Online.API.Response;

namespace BackspaceClient.Game.Tests.Visual.Templates;

public abstract partial class DataTestScene : BackspaceClientTestScene
{
    protected readonly Random Random = new();

    protected string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz #:?.\"";
        return new string([.. Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)])]);
    }

    protected User CreateUser()
    {
        return new User()
        {
            Id = RandomString(15),
            Username = RandomString(10),
            DisplayName = RandomString(5),
        };
    }

    protected DmChatMessageResponse CreateMessage(bool edited = false)
    {
        var user = CreateUser();
        return new()
        {
            Id = RandomString(15),
            ChannelId = RandomString(15),
            UserId = user.Id,
            Content = RandomString(25),
            User = user,
            EditedAt = edited ? Random.NextInt64() : null
        };
    }
}
