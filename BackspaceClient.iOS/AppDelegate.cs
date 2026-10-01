using osu.Framework.iOS;
using BackspaceClient.Game;

namespace BackspaceClient.iOS
{
    /// <inheritdoc />
    public class AppDelegate : GameApplicationDelegate
    {
        /// <inheritdoc />
        protected override osu.Framework.Game CreateGame() => new BackspaceClientGame();
    }
}
