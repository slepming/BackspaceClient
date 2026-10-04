#nullable enable
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace BackspaceClient.Game.Online.API;

public sealed class WebSocketClient(string? url, int? port) : IDisposable
{
    public event Action<string>? MessageReceived;
    public event Action? Closed;

    private readonly int? port = port;
    private readonly string? url = url;
    private WebSocketChannel? channel;

    public async Task Start(CancellationToken cancellationToken = default)
    {
        var webSocket = new ClientWebSocket();
        var uri = new Uri($@"wss://{url ?? "localhost"}/ws");
        if (port != null && url == null)
            uri = new Uri($@"ws://{url ?? "localhost"}:{port}/ws");
        await webSocket.ConnectAsync(uri, cancellationToken);
        channel = new WebSocketChannel(webSocket);
        channel.MessageReceived += msg => MessageReceived?.Invoke(msg);
        channel.ClosedPrematurely += () => Closed?.Invoke();
        channel.Start(cancellationToken);
    }

    public async Task SendAsync(string message, CancellationToken token = default)
    {
        if (channel == null)
            throw new InvalidOperationException($@"Must {nameof(Start)} first.");

        await channel.SendAsync(message, token);
    }

    public async Task StopAsync(CancellationToken stoppingToken = default)
    {
        try
        {
            if (channel != null)
                await channel.StopAsync(stoppingToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // has to be caught manually because outer task isn't accepting `stoppingToken`.
        }
    }

    public void Dispose()
    {
        channel?.Dispose();
    }
}
