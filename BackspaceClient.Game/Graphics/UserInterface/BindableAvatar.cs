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
public partial class BindableAvatar(Bindable<string> avatarUrl, Bindable<string> color) : CompositeDrawable
{
    private BindableOnlineAvatar onlineAvatar = new(avatarUrl);
    private Circle circleAvatar;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRangeInternal(
            [
                new DelayedLoadUnloadWrapper(() => onlineAvatar, 500, 500),
                circleAvatar = new()
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.Black,
                }
            ]
        );

        color.BindValueChanged(v =>
        {
            Logger.Log(v.NewValue);
            circleAvatar.Colour = ColourParsing.Parse(v.NewValue);
            Logger.Log(circleAvatar.Colour.ToString());
        });
    }
}
