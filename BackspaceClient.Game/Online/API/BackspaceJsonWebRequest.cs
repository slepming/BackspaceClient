using osu.Framework.IO.Network;

namespace BackspaceClient.Game.Online.API;

public partial class BackspaceJsonWebRequest<T>(string url = null) : JsonWebRequest<T>(url) where T : class
{
    protected override string UserAgent => "BackspaceClient";
}
