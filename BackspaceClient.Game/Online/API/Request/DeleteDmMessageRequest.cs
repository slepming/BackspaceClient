using System.Net.Http;
using BackspaceClient.Game.Online.API.Response;

namespace BackspaceClient.Game.Online.API.Request;

/// Deletes message from personal chat with companion
public partial class DeleteDmMessageRequest : InstanceAPIRequest<SuccessResponse>
{
    public string MessageId;

    protected override string Target => $"dm/messages/{MessageId}";

    public DeleteDmMessageRequest() => Method = HttpMethod.Delete;
}
