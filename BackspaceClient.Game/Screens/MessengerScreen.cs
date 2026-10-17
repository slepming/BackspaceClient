using System;
using System.Collections.Generic;
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
    private WsDispatcher webSocketDispatcher = new();

    // Design
    private MessengerSideBar sideBar;
    private ChatContainer chatContainer;

    [Resolved]
    private ScreenStack screenStack { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        instanceObserver.StartWebSocket();

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
                chatContainer = new(instanceObserver)
                {
                    RelativeSizeAxes = Axes.Both,
                    Provider = provider,
                }
            ]
        });

        sideBar.Selected.BindValueChanged(c =>
        {
            if (string.IsNullOrEmpty(c.NewValue)) return;
            chatContainer.DmChannelId = c.NewValue;

            Task.Run(() =>
            {
                lock (instanceObserver)
                {
                    var messageRequest = new GetMessagesRequest(c.NewValue);
                    messageRequest.AttachApi(provider);
                    instanceObserver.PerformAPIRequest(messageRequest);

                    Schedule(() =>
                    {
                        chatContainer.ClearMessages();

                        foreach (var message in messageRequest.Response)
                        {
                            chatContainer.AddOrUpdateMessage(message, true);
                        }
                    });
                }
            });
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        instanceObserver.WebSocketClient.MessageReceived += webSocketDispatcher.Dispatch;
        instanceObserver.WebSocketClient.Closed += () =>
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
                await instanceObserver.WebSocketClient.Start();
                await instanceObserver.WebSocketClient.SendAsync(JsonConvert.SerializeObject(new AuthMessage() { Token = provider.AccessKey }));
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
        webSocketDispatcher.Register<WsChatMessageCreated>("dm_message_created", messageCreated);
        webSocketDispatcher.Register<WsChannelUnreadCount>("channel_unread_count", channelUnreadCountUpdate);
    }

    private void channelUnreadCountUpdate(WsChannelUnreadCount c)
    {
        Schedule(() =>
        {
            foreach(var kv in c.Counts)
            {
                if (sideBar.Channels.TryGetValue(kv.Key, out var userCard))
                    userCard.UnreadMessagesCount.Value = kv.Value;
            }
        });
    }

    private void messageCreated(WsChatMessageCreated response)
    {
        Schedule(() =>
        {
            chatContainer.AddOrUpdateMessage(response.Message);
        });
    }

    private void updateUser(WsUserUpdated user)
    {
        Schedule(() =>
        {
            try
            {
                sideBar.UpdateUser(user.User);
            }
            catch (KeyNotFoundException k)
            {
                Logger.Error(k, "KeyNotFoundError");
            }
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
            sideBar.AddOrUpdateChannel(channel, instanceObserver.Current.Value);
        });
    }

    private void wsHandleReady(ReadyPayload ready)
    {
        Logger.Log($"Channels count: {ready.DmChannels.Count}");
        Schedule(() =>
        {
            foreach (var channel in ready.DmChannels)
            {
                sideBar.AddOrUpdateChannel(channel, instance: instanceObserver.Current.Value, ready.User, ready.UnreadCounts);
            }
        });
    }
}
