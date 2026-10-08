using Newtonsoft.Json;

namespace BackspaceClient.Game.Online.API.Response;

/// <summary>
/// Websocket response object for type parsing
/// </summary>
public class WsResponse
{
    [JsonProperty("type")] public string Type { get; set; }
}
