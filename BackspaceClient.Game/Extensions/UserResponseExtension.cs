using BackspaceClient.Game.Online.API.Response;

namespace BackspaceClient.Game.Extensions;

public static class UserResponseExtesion
{
    public static string CanonicalKey(this User user)
        => user.HomeInstance is null
            ? user.Id
            : $"{user.HomeInstance}:{user.HomeUserId}";
}
