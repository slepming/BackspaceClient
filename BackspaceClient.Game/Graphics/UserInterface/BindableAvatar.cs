using BackspaceClient.Game.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Logging;

namespace BackspaceClient.Game.Graphics.UserInterface;

/// <summary>
/// The avatar bindable that changes when a bindable value one of a bindable
/// </summary>
public partial class BindableAvatar(Bindable<string> avatarUrl, Bindable<string> color, string instanceUrl) : CompositeDrawable
{
    private BindableOnlineAvatar onlineAvatar = new(avatarUrl, instanceUrl);
    private Circle circleAvatar;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRangeInternal(
            [
                circleAvatar = new()
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.Black,
                },
                new DelayedLoadUnloadWrapper(() => onlineAvatar, 500, 500),
            ]
        );

        color.BindValueChanged(v =>
        {
            if (string.IsNullOrEmpty(v.NewValue)) return;

            Logger.Log(v.NewValue);
            circleAvatar.Colour = ColourParsing.Parse(v.NewValue);
            Logger.Log(circleAvatar.Colour.ToString());
        }, true);
    }
}
