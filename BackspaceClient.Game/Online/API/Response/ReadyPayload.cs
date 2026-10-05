using System.Collections.Generic;
using Newtonsoft.Json;

namespace BackspaceClient.Game.Online.API.Response;

public class ReadyPayload
{
    [JsonProperty("user")] public User User { get; set; }
    [JsonProperty("dmChannels")] public List<DmChannel> DmChannels;
}
