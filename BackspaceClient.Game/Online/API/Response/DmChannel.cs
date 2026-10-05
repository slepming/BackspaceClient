using System.Collections.Generic;
using Newtonsoft.Json;

namespace BackspaceClient.Game.Online.API.Response;

public class DmChannel
{
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("federatedId")] public string FederatedId { get; set; }
    [JsonProperty("members")] public List<User> Members { get; set; }
}
