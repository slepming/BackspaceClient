#nullable enable
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Logging;

namespace BackspaceClient.Game.Online.API;

public sealed class WebSocketClient(string? url = null, int? port = null) : IDisposable
{
    public event Action<string>? MessageReceived;
    public event Action? Closed;

    private readonly int? port = port;
    private string? url = url;
    private WebSocketChannel? channel;

    public async Task Start(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(url))
        {
            Logger.Log("URL is empty or null", level: LogLevel.Error);
            return;
        }

        string wsScheme = "ws";
        string hostAndPort = url;

        if (url.Contains("://"))
        {
            try
            {
                Uri instanceUri = new Uri(url);
                hostAndPort = instanceUri.Authority;

                if (instanceUri.Scheme == Uri.UriSchemeHttps || instanceUri.Scheme == "wss")
                {
                    wsScheme = "wss";
                }
            }
            catch (UriFormatException ex)
            {
                Logger.Log($"Invalid URL format: {ex.Message}", level: LogLevel.Error);
                return;
            }
        }
        else
        {
            if (port != null && !hostAndPort.Contains(":"))
            {
                hostAndPort = $"{hostAndPort}:{port}";
            }
        }

        var finalUri = new Uri($"{wsScheme}://{hostAndPort}/ws");
        Logger.Log($"Websocket URL {finalUri}", target: LoggingTarget.Network);

        if (finalUri.Host == "localhost" && finalUri.Scheme == "wss")
        {
            Logger.Log("Forcing ws:// for localhost to avoid local SSL handshake issues", level: LogLevel.Important);
            finalUri = new Uri($"ws://{finalUri.Authority}/ws");
        }

        var webSocket = new ClientWebSocket();

        await webSocket.ConnectAsync(finalUri, cancellationToken);

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
