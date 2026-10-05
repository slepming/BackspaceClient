using System.Collections.Generic;
using Newtonsoft.Json;

namespace BackspaceClient.Game.Online.API.Response;

/// <summary>
/// <seealso href="https://github.com/TheZwiss/backspace/blob/main/docs/systems/api.md#auth-routesauthts--public-rate-limited">Authorization process returns OAuthToken contain user token and user information.</seealso>
/// </summary>
public class OAuthToken
{
    [JsonProperty("token")] public string Token { get; set; }
    [JsonProperty("user")] public User User { get; set; }
}
