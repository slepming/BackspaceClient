using System.Net.Http;
using BackspaceClient.Game.Online.API.Response;

namespace BackspaceClient.Game.Online.API.Request;

public partial class DeleteMessage : InstanceAPIRequest<SuccessResponse>
{
    public string MessageId;

    protected override string Target => $"messages/{MessageId}";
}
