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
            Logger.Log($"{raw}", target: LoggingTarget.Network);
            if (string.IsNullOrEmpty(type)) return;

            var deserializedRaw = JsonConvert.DeserializeObject<T>(raw);

            if (deserializedRaw != null)
                handler(deserializedRaw);
        };
    }

    public void Dispatch(string raw)
    {
        if (raw == null) return;

        string type;

        var snowball = JsonConvert.DeserializeObject<WsResponse>(raw);
        if (string.IsNullOrEmpty(snowball.Type))
        {
            Logger.Log($"Websocket message have empty type, raw: {raw}", target: LoggingTarget.Network);
        }

        type = snowball.Type ?? string.Empty;

        if (!handlers.TryGetValue(type, out var handler))
        {
            Logger.Log($"Unknown message type: {type} and raw: {raw}", level: LogLevel.Error, target: LoggingTarget.Network);
            return;
        }

        handler(raw);
    }
}
