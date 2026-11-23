using System;
using BackspaceClient.Game.Online.API.Request;
using osu.Framework.Bindables;
using osu.Framework.Logging;

namespace BackspaceClient.Game.Online.API;

/// <summary>
/// Instance observer to instant switching between instances
/// </summary>
public class InstanceObserver : IDisposable, ICanPerformNetworkRequest
{
    /// <summary>
    /// Current instance URL
    /// </summary>
    public Bindable<string> Current { get; } = new("http://localhost:3005");

    public WebSocketClient WebSocketClient { get; private set; }
    public APIProvider Provider { get; private set; }

    public void UpdateInstance(string url) => Current.Value = url;

    public InstanceObserver()
    {
        Current.BindValueChanged(i => Logger.Log($"Instance server updated: {i.NewValue}"));
    }

    public void PerformAPIRequest<T>(InstanceAPIRequest<T> request) where T : class
    {
        request.AttachApi(Provider);
        request.URL = Current.Value;
        request.Perform();
    }

    public void StartWebSocket() => WebSocketClient = new WebSocketClient(Current.Value);

    public void SetProvider(APIProvider provider) => Provider = provider;

    public void Dispose()
    {
        WebSocketClient.Dispose();
    }
}

public interface ICanPerformNetworkRequest
{
    void PerformAPIRequest<T>(InstanceAPIRequest<T> request) where T : class;
}
