using Newtonsoft.Json;

namespace BackspaceClient.Game.Online.API;

public sealed class AuthMessage
{
    [JsonProperty("type")]
    public string Type => "auth";

    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("client")]
    public string Client { get; set; } = "desktop";
}
