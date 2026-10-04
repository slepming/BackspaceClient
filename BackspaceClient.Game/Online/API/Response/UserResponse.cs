#nullable enable
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BackspaceClient.Game.Online.API.Response;

public sealed class UserResponse
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;

    [JsonProperty("displayName")]
    public string? DisplayName { get; set; }

    [JsonProperty("avatar")]
    public string? Avatar { get; set; }

    [JsonProperty("banner")]
    public string? Banner { get; set; }

    [JsonProperty("accentColor")]
    public string? AccentColor { get; set; }

    [JsonProperty("avatarColor")]
    public string? AvatarColor { get; set; }

    [JsonProperty("bio")]
    public string? Bio { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; } = "offline";

    [JsonProperty("customStatus")]
    public string? CustomStatus { get; set; }

    [JsonProperty("isAdmin")]
    public bool IsAdmin { get; set; }

    [JsonProperty("discoverable")]
    public bool Discoverable { get; set; }

    [JsonProperty("profileUpdatedAt")]
    public long ProfileUpdatedAt { get; set; }

    [JsonProperty("createdAt")]
    public long CreatedAt { get; set; }

    [JsonProperty("homeInstance")]
    public string? HomeInstance { get; set; }

    [JsonProperty("homeUserId")]
    public string? HomeUserId { get; set; }

    [JsonProperty("replicatedInstances")]
    public List<string> ReplicatedInstances { get; set; } = new();

    [JsonProperty("showActivity")]
    public bool ShowActivity { get; set; }

    [JsonProperty("federationHomeOrphaned")]
    public bool FederationHomeOrphaned { get; set; }

    [JsonIgnore]
    public bool IsFederated => HomeInstance != null;

    [JsonIgnore]
    public bool IsNative => HomeInstance == null;
}
