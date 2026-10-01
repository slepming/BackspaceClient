using System;
using System.Net.Http;
using JetBrains.Annotations;
using osu.Framework.IO.Network;
using osu.Framework.Logging;

namespace BackspaceClient.Game.Online.API;

public abstract class APIRequest<T> where T : class
{
    protected virtual WebRequest CreateWebRequest() => new BackspaceJsonWebRequest<T>(URI);

    [CanBeNull]
    public T Response { get; private set; }

    protected abstract string Target { get; }

    protected abstract string URI { get; }

    protected APIProvider API;

    protected WebRequest WebRequest;

    public virtual void Perform()
    {
        Logger.Log($@"Performing request {this}", LoggingTarget.Network);
        WebRequest = CreateWebRequest();
        WebRequest.Method = HttpMethod.Get;
        WebRequest.AllowRetryOnTimeout = false;

        WebRequest.AddHeader(@"Accept-Language", "en");
        WebRequest.AddHeader(@"x-api-version", "2");

        if (API != null && API.AccessKey != null)
            WebRequest.AddHeader("Authorization", $"Bearer {API.AccessKey}");

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

    public void AttachApi(APIProvider api)
    {
        if (API != null && API != api)
            throw new InvalidOperationException("Attached API cannot be changed after initial set.");

        API = api;
    }

    protected void PostProcess()
    {
        if (WebRequest != null)
            Response = ((BackspaceJsonWebRequest<T>)WebRequest).ResponseObject;
    }
}
