using System;
using System.Linq;
using BackspaceClient.Game.Graphics.UserInterface.Containers;
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
            Add(chat = new()
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
            mes.Content = "# Hello\n ## Hello2 \n Why i'm freak: \n - Cuz I Have github account\n - I haven't work";
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
}
