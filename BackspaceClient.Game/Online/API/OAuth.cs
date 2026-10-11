using System.Net;
using System.Net.Http;
using BackspaceClient.Game.Online.API.Response;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Framework.Bindables;

namespace BackspaceClient.Game.Online.API;

public class OAuth(string username, [NotNull] string password)
{
    private readonly string username = username;

    [NotNull]
    private readonly string password = password;

    public readonly Bindable<OAuthToken> Token = new();

    private bool userAuthorization(string url)
    {
        try
        {
            AccessTokenRequest tokenRequest = new AccessTokenRequest()
            {
                Username = username,
                Password = password,
                Url = $@"{url}/api/auth/login"
            };

            using (tokenRequest)
            {
                tokenRequest.Perform();
                Token.Value = tokenRequest.ResponseObject;
                return true;
            }
        }
        catch (WebException)
        {
            throw;
        }
        catch
        {
            return false;
        }
    }

    internal string RequestAccessToken(string url)
    {
        if (!userAuthorization(url)) return null;

        return Token.Value.Token;
    }

    private class AccessTokenRequest : BackspaceJsonWebRequest<OAuthToken>
    {
        internal string Username;
        internal string Password;

        public AccessTokenRequest()
        {
            Method = HttpMethod.Post;
            ContentType = "application/json";
        }

        protected override void PrePerform()
        {
            // Using AddParameter leads to the error: "{"statusCode":415,"code":"FST_ERR_CTP_INVALID_MEDIA_TYPE","error":"Unsupported Media Type","message":"Unsupported Media Type: undefined"}"
            //AddParameter("username", Username);
            //AddParameter("password", Password);

            var payload = new { username = Username, password = Password };

            var serialized = JsonConvert.SerializeObject(payload);
            AddRaw(serialized);

            base.PrePerform();
        }
    }
}
