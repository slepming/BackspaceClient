using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace BackspaceClient.Game.Graphics.UserInterface;

[LongRunningLoad]
public partial class BindableOnlineAvatar(Bindable<string> avatarUrl, string instanceUrl) : Sprite
{
    [BackgroundDependencyLoader]
    private void load(TextureStore textureStore)
    {
        if (avatarUrl == null) return;

        avatarUrl.BindValueChanged(v =>
        {
            if (string.IsNullOrEmpty(v.NewValue)) return;

            Texture = textureStore.Get($"{instanceUrl}/api/uploads/{v.NewValue}"); // TODO: find out why webp avatar calls exception
        }, true);
    }
}
