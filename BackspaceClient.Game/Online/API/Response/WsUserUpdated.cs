using Newtonsoft.Json;

namespace BackspaceClient.Game.Online.API.Response;

public class WsUserUpdated
{
    [JsonProperty("user")] public User User { get; set; }
}
