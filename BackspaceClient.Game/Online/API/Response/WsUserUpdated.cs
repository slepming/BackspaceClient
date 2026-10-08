using Newtonsoft.Json;

namespace BackspaceClient.Game.Online.API.Response;

/// <summary>
/// Websocket user_updated response
/// </summary>
public class WsUserUpdated
{
    [JsonProperty("user")] public User User { get; set; }
}
