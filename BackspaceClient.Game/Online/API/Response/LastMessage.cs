using Newtonsoft.Json;

namespace BackspaceClient.Game.Online.API.Response;

public class LastMessage
{
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("dmChannelId")] public string DmChannelId { get; set; }
    [JsonProperty("userId")] public string UserId { get; set; }
    [JsonProperty("content")] public string Content { get; set; }
}
