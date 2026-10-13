using BackspaceClient.Game.Graphics.UserInterface.Containers;
using BackspaceClient.Game.Online.API.Response;
using BackspaceClient.Game.Tests.Visual.Templates;
using NUnit.Framework;
using osu.Framework.Graphics;

namespace BackspaceClient.Game.Tests.Visual.UserInterface;

public partial class ChatContainerTestScene : DataTestScene
{
    private ChatContainer chat;

    private string templateMessageId;

    [SetUp]
    public void Setup()
    {
        Clear();
        Schedule(() =>
        {
            templateMessageId = RandomString(15);
            Add(chat = new(null)
            {
                RelativeSizeAxes = Axes.Both
            });
        });
    }

    [Test]
    public void ChatManipulations()
    {
        AddRepeatStep("Add message", () =>
        {
            chat.AddOrUpdateMessage(CreateMessage());
        }, 5);

        AddStep("Add markdown message", () =>
        {
            var mes = CreateMessage();
            mes.Content = "# Hello\n ## Привет \n Why i'm freak: \n - Cuz I Have github account\n - I haven't work";
            chat.AddOrUpdateMessage(mes);
        });
    }

    [Test]
    public void EditMessages()
    {
        AddRepeatStep("Change/add template message", () =>
        {
            var templateMessage = CreateMessage(true);
            templateMessage.Id = templateMessageId;
            chat.AddOrUpdateMessage(templateMessage);
        }, 5);
    }

    [Test]
    public void Consecutive()
    {
        AddRepeatStep("Add Consecutive Message", messageWithSameUserId, 3);

        AddRepeatStep("Add message", () =>
        {
            chat.AddOrUpdateMessage(CreateMessage());
        }, 2);
        AddRepeatStep("Add Consecutive Message", messageWithSameUserId, 3);
    }

    [Test]
    public void DeleteMessage()
    {
        AddStep("Add message", () =>
        {
            var templateMessage = CreateMessage(true);
            templateMessage.Id = templateMessageId;
            chat.AddOrUpdateMessage(templateMessage);
        });
        AddStep("Delete message", () => {
                chat.RemoveMessage(templateMessageId);
        });
        AddStep("Add message", () =>
        {
            var templateMessage = CreateMessage(true);
            templateMessageId = RandomString(15);
            templateMessage.Id = templateMessageId;
            chat.AddOrUpdateMessage(templateMessage);
        });
        AddStep("Silent message", () => {
                chat.RemoveMessage(templateMessageId, true);
        });
    }

    [Test]
    public void ReplyMessage()
    {
        AddStep("Add message", () =>
        {
            var templateMessage = CreateMessage();
            templateMessage.Id = templateMessageId;
            chat.AddOrUpdateMessage(templateMessage);
        });

        AddStep("Reply message", () =>
        {
            var templateMessage = CreateMessage();
            templateMessage.ReplyToId = templateMessageId;
            chat.AddOrUpdateMessage(templateMessage);
        });
    }

    private void messageWithSameUserId()
    {
        var templateMessage = CreateMessage();
        templateMessage.User.Id = templateMessageId;

        templateMessage.Content = RandomString(5);
        chat.AddOrUpdateMessage(templateMessage);
    }
}
