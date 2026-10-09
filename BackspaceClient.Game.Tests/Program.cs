using osu.Framework;
using osu.Framework.Platform;

namespace BackspaceClient.Game.Tests
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableDesktopHost("BackspaceClient(development)"))
            using (var game = new BackspaceClientTestBrowser())
                host.Run(game);
        }
    }
}
