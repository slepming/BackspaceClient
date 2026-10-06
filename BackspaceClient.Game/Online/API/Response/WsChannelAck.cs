using Newtonsoft.Json;

namespace BackspaceClient.Game.Online.API.Response;

public class WsChannelAck
{
    [JsonProperty("channelId")] public string ChannelId { get; set; }
    [JsonProperty("messageId")] public string MessageId { get; set; }
}
