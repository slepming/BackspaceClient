namespace BackspaceClient.Game.Online.API.Request;

public abstract class InstanceAPIRequest<T> : APIRequest<T>, IHaveInstanceUrl where T : class
{
    protected override string URI => $@"{URL}/api/{Target}";

    public string URL { get; set; }
}
