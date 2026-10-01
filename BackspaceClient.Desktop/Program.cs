using osu.Framework.Platform;
using osu.Framework;
using BackspaceClient.Game;

namespace BackspaceClient.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableDesktopHost(@"BackspaceClient"))
            using (osu.Framework.Game game = new BackspaceClientGame())
                host.Run(game);
        }
    }
}
