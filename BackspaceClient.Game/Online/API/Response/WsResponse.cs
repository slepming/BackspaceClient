using Newtonsoft.Json;

namespace BackspaceClient.Game.Online.API.Response;

public class WsResponse
{
    [JsonProperty("type")] public string Type { get; set; }
}
