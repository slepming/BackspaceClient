using BackspaceClient.Game.Online.API;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class SignUpContainer : AuthorizationContainer
{
    protected override string TitleText => "SignUp";

    public SignUpContainer(InstanceObserver observer) : base(observer) { }
}
