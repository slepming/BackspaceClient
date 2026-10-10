using System.Collections.Generic;
using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace BackspaceClient.Game.Configuration;

public partial class InstanceConfiguration(Storage storage, IDictionary<InstanceSettings, object> defaultOverrides = null) : IniConfigManager<InstanceSettings>(storage, defaultOverrides)
{
    protected override string Filename => "instance.ini";

    protected override void InitialiseDefaults()
    {
        SetDefault(InstanceSettings.Home, "https://backspaced.org");
    }
}


public enum InstanceSettings
{
    Home,
}
