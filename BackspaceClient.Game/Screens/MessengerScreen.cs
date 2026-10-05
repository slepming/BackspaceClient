using System.Linq;
using System.Threading.Tasks;
using BackspaceClient.Game.Graphics.UserInterface;
using BackspaceClient.Game.Online.API;
using BackspaceClient.Game.Online.API.Response;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
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
        };

        AddInternal(sideBar = new()
        {
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(.2f, 1f),
        });

        initializeDispatchers();
    }

    protected override void LoadComplete()
    {
        base.LoadAsyncComplete();
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
    }

    private void wsHandleReady(ReadyPayload ready)
    {
        Logger.Log($"Channels count: {ready.DmChannels.Count}");
        Schedule(() =>
        {
            foreach (var channel in ready.DmChannels)
            {
                sideBar.AddOrUpdateUser(channel.Members.Last());
            }
        });
    }
}
