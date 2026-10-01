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

public class User
{
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("username")] public string Username { get; set; }
    [JsonProperty("displayName")] public string DisplayName { get; set; }
    [JsonProperty("avatar")] public string Avatar { get; set; }
    [JsonProperty("banner")] public string Banner { get; set; }
    [JsonProperty("accentColor")] public string AccentColor { get; set; }
    [JsonProperty("avatarColor")] public string AvatarColor { get; set; }
    [JsonProperty("bio")] public string Bio { get; set; }
    [JsonProperty("customStatus")] public string CustomStatus { get; set; }
    [JsonProperty("status")] public string Status { get; set; }
    [JsonProperty("isAdmin")] public bool IsAdmin { get; set; }
    [JsonProperty("discoverable")] public bool Discoverable { get; set; }
    [JsonProperty("profileUpdatedAt")] public long ProfileUpdatedAt { get; set; }
    [JsonProperty("createdAt")] public long CreatedAt { get; set; }
    [JsonProperty("homeInstance")] public string HomeInstance { get; set; }
    [JsonProperty("homeUserId")] public string HomeUserId { get; set; }
    [JsonProperty("replicatedInstances")] public List<string> ReplicatedInstances { get; set; }
    [JsonProperty("showActivity")] public bool ShowActivity { get; set; }
    [JsonProperty("federationHomeOrphaned")] public bool FederationHomeOrphaned { get; set; }
}
