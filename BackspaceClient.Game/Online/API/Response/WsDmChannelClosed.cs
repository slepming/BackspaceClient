using Newtonsoft.Json;

namespace BackspaceClient.Game.Online.API.Response;

public class WsDmChannelClosed
{
    [JsonProperty("dmChannelId")] public string Id { get; set; }
}
