using BackspaceClient.Game.Online.API;
using osu.Framework.Screens;

namespace BackspaceClient.Game.Screens;

public partial class MessangerScreen(APIProvider api) : Screen
{
    private APIProvider provider = api;
}
