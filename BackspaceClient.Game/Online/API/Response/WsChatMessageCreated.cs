using Newtonsoft.Json;

namespace BackspaceClient.Game.Online.API.Response;

public class WsChatMessageCreated
{
    [JsonProperty("message")] public DmChatMessageResponse Message { get; set; }
}
