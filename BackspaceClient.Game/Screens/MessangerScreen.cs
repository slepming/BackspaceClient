using System.Threading.Tasks;
using BackspaceClient.Game.Graphics.UserInterface;
using BackspaceClient.Game.Online.API;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace BackspaceClient.Game.Screens;

public partial class MessangerScreen(APIProvider api) : Screen
{
    private APIProvider provider = api;
    private WebSocketClient webSocketClient;

    private MessengerSideBar sideBar;

    [BackgroundDependencyLoader]
    private void load()
    {
        webSocketClient = new(null, 3005);
        webSocketClient.MessageReceived += m => {
            Logger.Log($"WebSocket: {m}");
        };
        webSocketClient.Closed += () =>
        {
            Logger.Log("Closed");
        };

        AddInternal(sideBar = new() {
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(.2f, 1f),
        });
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
