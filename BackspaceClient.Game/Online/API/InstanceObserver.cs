using BackspaceClient.Game.Online.API.Request;
using osu.Framework.Bindables;
using osu.Framework.Logging;

namespace BackspaceClient.Game.Online.API;

/// <summary>
/// Instance observer for instant switching between instances
/// </summary>
public class InstanceObserver
{
    /// <summary>
    /// Current instance URL
    /// </summary>
    public Bindable<string> Current { get; } = new("http://localhost:3005");

    public void UpdateInstance(string url) => Current.Value = url;

    public InstanceObserver()
    {
        Current.BindValueChanged(i => Logger.Log($"Instance server updated: {i.NewValue}"));
    }

    public void PerformAPIRequest<T>(InstanceAPIRequest<T> request) where T : class
    {
        request.URL = Current.Value;
        request.Perform();
    }
}
