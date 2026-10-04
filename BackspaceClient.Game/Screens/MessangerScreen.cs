using System.Threading.Tasks;
using BackspaceClient.Game.Online.API;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Logging;
using osu.Framework.Screens;

namespace BackspaceClient.Game.Screens;

public partial class MessangerScreen(APIProvider api) : Screen
{
    private APIProvider provider = api;
    private WebSocketClient webSocketClient;

    [BackgroundDependencyLoader]
    private void load()
    {
        webSocketClient = new(null, 3005);
        webSocketClient.MessageReceived += m => {
            Logger.Log($"WebSocket: {m}");
        };
        webSocketClient.Closed += () => {
            Logger.Log("Closed");
        };
    }

    protected override void LoadAsyncComplete()
    {
        base.LoadAsyncComplete();
        Scheduler.Add(() => {
            Task.Run(async () =>
            {
                await webSocketClient.Start();
                await webSocketClient.SendAsync(JsonConvert.SerializeObject(new AuthMessage() { Token = provider.AccessKey }));
            });
        });
    }
}
