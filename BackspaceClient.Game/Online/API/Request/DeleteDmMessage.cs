using BackspaceClient.Game.Online.API.Response;

namespace BackspaceClient.Game.Online.API.Request;

public partial class DeleteDmMessage : InstanceAPIRequest<SuccessResponse>
{
    public string MessageId;

    protected override string Target => $"dm/messages/{MessageId}";
}
