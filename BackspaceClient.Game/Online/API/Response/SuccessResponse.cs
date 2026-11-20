using Newtonsoft.Json;

namespace BackspaceClient.Game.Online.API.Response;

public partial class SuccessResponse
{
    [JsonProperty("success")] public bool Success { get; set; }
}

