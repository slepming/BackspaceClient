using System.Linq;
using System.Threading.Tasks;
using BackspaceClient.Game.Graphics.UserInterface;
using BackspaceClient.Game.Graphics.UserInterface.Containers;
using BackspaceClient.Game.Online.API;
using BackspaceClient.Game.Online.API.Response;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace BackspaceClient.Game.Screens;

public partial class MessengerScreen(APIProvider api) : Screen
{
    private APIProvider provider = api;
    private WebSocketClient webSocketClient;
    private WsDispatcher webSocketDispatcher = new();

    private MessengerSideBar sideBar;
    private ChatContainer chatContainer;

    [Resolved]
    private ScreenStack screenStack { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        webSocketClient = new(null, 3005);
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

        initializeDispatchers();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        Scheduler.Add(() =>
        {
            Task.Run(async () =>
            {
                await webSocketClient.Start();
                await webSocketClient.SendAsync(JsonConvert.SerializeObject(new AuthMessage() { Token = provider.AccessKey }));
            });
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
                sideBar.AddOrUpdateChannel(channel);
            }
        });
    }

    protected override void Dispose(bool k)
    {
        webSocketClient.Dispose();
    }
}
