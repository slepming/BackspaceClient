using System.Collections.Generic;
using BackspaceClient.Game.Online.API.Response;
using osu.Framework.IO.Network;

namespace BackspaceClient.Game.Online.API.Request;

public class GetMessagesRequest(string dm_channel_id, int limit = 50) : InstanceAPIRequest<List<DmChatMessageResponse>>
{
    protected override string Target => $"/dm/{dm_channel_id}/messages";

    protected override WebRequest CreateWebRequest()
    {
        var req = base.CreateWebRequest();

        req.AddParameter("limit", limit.ToString());

        return req;
    }
}
