using System;
using System.Net.Http;
using BackspaceClient.Game.Online.API.Response;
using Newtonsoft.Json;
using osu.Framework.Logging;

namespace BackspaceClient.Game.Online.API.Request;

public class SendMessageReqeust(string dmChannelId) : InstanceAPIRequest<DmChatMessageResponse>
{
    private readonly string dmChannelId = dmChannelId;

    protected override string Target => $"dm/{dmChannelId}/messages";

    public string Content { get; set; }
    public string Attachments { get; set; }
    public string ReplyToId { get; set; }

    public override void Perform()
    {
        Logger.Log($@"Performing request POST {this}", LoggingTarget.Network);
        WebRequest = CreateWebRequest();
        WebRequest.ContentType = "application/json";
        WebRequest.Method = HttpMethod.Post;
        WebRequest.AllowRetryOnTimeout = false;

        if (API != null && API.AccessKey != null)
            WebRequest.AddHeader("Authorization", $"Bearer {API.AccessKey}");

        var payload = new
        {
            content = Content,
        };

        WebRequest.AddRaw(JsonConvert.SerializeObject(payload));

        try
        {
            WebRequest.Perform();
        }
        catch (Exception e)
        {
            Logger.Error(e, "An error occurred while performing request");
        }

        PostProcess();
    }
}
