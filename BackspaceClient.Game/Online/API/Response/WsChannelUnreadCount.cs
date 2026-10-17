using System.Collections.Generic;
using Newtonsoft.Json;

namespace BackspaceClient.Game.Online.API.Response;

public class WsChannelUnreadCount
{
    [JsonProperty("counts")] public Dictionary<string, int> Counts { get; set; } = [];
}
