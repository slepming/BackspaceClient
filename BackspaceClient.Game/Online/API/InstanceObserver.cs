using BackspaceClient.Game.Online.API.Request;
using osu.Framework.Bindables;

namespace BackspaceClient.Game.Online.API;

/// <summary>
/// Instance observer for instant switching between instances
/// </summary>
public class InstanceObserver()
{
    /// <summary>
    /// Current instance URL
    /// </summary>
    public Bindable<string> Current { get; } = new("http://localhost:3005");

    public void UpdateInstance(string url) => Current.Value = url;

    public void PerformAPIRequest<T>(InstanceAPIRequest<T> request) where T: class
    {
        request.URL = Current.Value;
        request.Perform();
    }
}
