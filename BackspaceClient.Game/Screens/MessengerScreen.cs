using System;
using System.Linq;
using System.Threading.Tasks;
using BackspaceClient.Game.Configuration;
using BackspaceClient.Game.Graphics.UserInterface;
using BackspaceClient.Game.Graphics.UserInterface.Containers;
using BackspaceClient.Game.Online.API;
using BackspaceClient.Game.Online.API.Request;
using BackspaceClient.Game.Online.API.Response;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace BackspaceClient.Game.Screens;

public partial class MessengerScreen(APIProvider api, InstanceObserver instanceObserver) : Screen
{
    // Network
    private APIProvider provider = api;
    private WebSocketClient webSocketClient;
    private WsDispatcher webSocketDispatcher = new();

    // Design
    private MessengerSideBar sideBar;
    private ChatContainer chatContainer;

    [Resolved]
    private ScreenStack screenStack { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        webSocketClient = new(instanceObserver.Current.Value);

        AddInternal(new FillFlowContainer()
        {
            Direction = FillDirection.Horizontal,
            RelativeSizeAxes = Axes.Both,
            Children =
            [
                sideBar = new()
                {
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(.2f, 1f),
                },
                chatContainer = new()
                {
                    RelativeSizeAxes = Axes.Both,
                }
            ]
        });

        sideBar.Selected.BindValueChanged(c =>
        {
            if (string.IsNullOrEmpty(c.NewValue)) return;

            var messageRequest = new GetMessagesRequest(c.NewValue);
            messageRequest.AttachApi(provider);
            instanceObserver.PerformAPIRequest(messageRequest);

            chatContainer.ClearMessages();

            foreach(var message in messageRequest.Response)
            {
                chatContainer.AddOrUpdateMessage(message);
            }
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        webSocketClient.MessageReceived += m =>
        {
            webSocketDispatcher.Dispatch(m);
        };
        webSocketClient.Closed += () =>
        {
            Logger.Log("Closed");
            Schedule(() =>
            {
                screenStack.Push(new MainScreen());
            });
        };

        initializeDispatchers();

        Task.Run(async () =>
        {
            try
            {
                await webSocketClient.Start();
                await webSocketClient.SendAsync(JsonConvert.SerializeObject(new AuthMessage() { Token = provider.AccessKey }));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Websocket error");
            }
        });
    }

    private void initializeDispatchers()
    {
        webSocketDispatcher.Register<ReadyPayload>("ready", wsHandleReady);
        webSocketDispatcher.Register<DmChannel>("dm_channel_created", dmChannelCreated);
        webSocketDispatcher.Register<WsDmChannelClosed>("dm_channel_closed", dmChannelClosed);
        webSocketDispatcher.Register<WsChannelAck>("channel_ack", channelAck);
        webSocketDispatcher.Register<WsUserUpdated>("user_updated", updateUser);
    }

    private void updateUser(WsUserUpdated user)
    {
        Schedule(() =>
        {
            sideBar.UpdateUser(user.User);
        });
    }

    private void channelAck(WsChannelAck ack)
    {
        dmChannelCreated(new() { Id = ack.ChannelId });
    }

    private void dmChannelClosed(WsDmChannelClosed channel)
    {
        Schedule(() =>
        {
            if (!string.IsNullOrEmpty(channel.Id)) sideBar.RemoveDmChannel(channel.Id);
            else Logger.Log("Channel id equals null", level: LogLevel.Important);
        });
    }

    private void dmChannelCreated(DmChannel channel)
    {
        Schedule(() =>
        {
            sideBar.AddOrUpdateChannel(channel);
        });
    }

    private void wsHandleReady(ReadyPayload ready)
    {
        Logger.Log($"Channels count: {ready.DmChannels.Count}");
        Schedule(() =>
        {
            foreach (var channel in ready.DmChannels)
            {
                sideBar.AddOrUpdateChannel(channel, ready.User.Username);
            }
        });
    }

    protected override void Dispose(bool k)
    {
        webSocketClient.Dispose();
    }
}
