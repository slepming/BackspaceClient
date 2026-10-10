using BackspaceClient.Game.Extensions;
using osu.Framework.Graphics.Containers;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class ShakerContainer : Container
{
    public void Shake(double? length = null) => this.Shake(80, maximumLength: length);
}
