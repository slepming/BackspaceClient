using System;
using BackspaceClient.Game.Configuration;
using BackspaceClient.Game.Online.API;
using osu.Framework.Allocation;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class LoginContainer : AuthorizationContainer
{
    protected override string TitleText => "Login";

    public LoginContainer(InstanceObserver observer) : base(observer) { }

    [BackgroundDependencyLoader]
    private void load()
    {
        FinalButton.Action = submit;

        PasswordTextBox.OnCommit += (_, newText) =>
        {
            if (newText) submit();
        };

        InstanceTextBox.OnCommit += (instance, newText) =>
        {
            if (InstanceObserver == null) ShowError(new Exception("Instance Observer is null"));
            if (newText)
            {
                InstanceConfig.SetValue(InstanceSettings.Home, instance.Current.Value);
                InstanceObserver.Current.Value = instance.Current.Value;
            }

            if (!string.IsNullOrEmpty(PasswordTextBox.Text) && !string.IsNullOrEmpty(LoginTextBox.Text)) submit();
        };
    }

    private void submit()
    {
        if (InstanceObserver == null) ShowError(new Exception("Instance Observer is null"));

        try
        {
            HideErrorContainer();
            Authorize(LoginTextBox.Text, PasswordTextBox.Text);
        }
        catch (Exception e)
        {
            ShowError(e);
        }
    }
}
