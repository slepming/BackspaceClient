using BackspaceClient.Game.Online.API;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class LoginContainer : AuthorizationContainer
{
    protected override string TitleText => "Login";

    public LoginContainer(InstanceObserver observer) : base(observer) {}
}
