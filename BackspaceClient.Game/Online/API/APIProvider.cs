using osu.Framework.Graphics;

namespace BackspaceClient.Game.Online.API;

public partial class APIProvider : Component
{
    private readonly OAuth oauth;
    public string AccessKey { get; private set; }
    public bool HasKey => AccessKey != null;

    public APIProvider(string username, string passw)
    {
        oauth = new OAuth(username, passw);
        oauth.Token.BindValueChanged(a =>
        {
            if (a.NewValue != null)
                AccessKey = a.NewValue.Token;
        });
        oauth.RequestAccessToken();
    }
}
