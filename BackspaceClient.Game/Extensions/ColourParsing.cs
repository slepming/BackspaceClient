#nullable enable

using System.Reflection;
using osu.Framework.Graphics;

namespace BackspaceClient.Game.Extensions;

public static class ColourParsing
{
    public static Colour4 Parse(string name)
    {
        var color_name = char.ToUpper(name[0]) + name[1..];
        var clr = typeof(Colour4).GetProperty(color_name, BindingFlags.Static | BindingFlags.Public)?.GetValue(null);

        return clr == null ? Colour4.White : (Colour4)clr;
    }
}
