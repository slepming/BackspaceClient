using System;
using System.Collections.Generic;
using BackspaceClient.Game.Online.API.Response;
using Newtonsoft.Json;
using osu.Framework.Logging;

namespace BackspaceClient.Game.Online.API;

public class WsDispatcher
{
    private readonly Dictionary<string, Action<string>> handlers = new();

    public void Register<T>(string type, Action<T> handler)
    {
        handlers[type] = raw =>
        {
            if (string.IsNullOrEmpty(type)) return;

            var deserializedRaw = JsonConvert.DeserializeObject<T>(raw);

            if (deserializedRaw != null)
                handler(deserializedRaw);
        };
    }

    public void Dispatch(string raw) {
        if (raw == null) return;

        string type;

        var snowball = JsonConvert.DeserializeObject<WsResponse>(raw);
        type = snowball.Type ?? string.Empty;

        if (!handlers.TryGetValue(type, out var handler))
        {
            Logger.Log($"Unknown message type: {type} and raw: {raw}", level: LogLevel.Error);
            return;
        }

        handler(raw);
    }
}
