#nullable enable
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BackspaceClient.Game.Online.API.Response;

public class MessageWithUserResponse
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("channelId")]
    public string ChannelId { get; set; } = string.Empty;

    [JsonProperty("userId")]
    public string UserId { get; set; } = string.Empty;

    [JsonProperty("content")]
    public string? Content { get; set; }

    [JsonProperty("replyToId")]
    public string? ReplyToId { get; set; }

    [JsonProperty("editedAt")]
    public long? EditedAt { get; set; }

    [JsonProperty("createdAt")]
    public long CreatedAt { get; set; }

    [JsonProperty("user")]
    public User User { get; set; } = new();

    [JsonProperty("attachments")]
    public List<Attachment> Attachments { get; set; } = new();

    [JsonProperty("embeds")]
    public List<Embed> Embeds { get; set; } = new();

    [JsonProperty("reactions")]
    public List<Reaction> Reactions { get; set; } = new();

    [JsonProperty("replyTo")]
    public MessageWithUserResponse? ReplyTo { get; set; }
}

public class Attachment
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("filename")]
    public string Filename { get; set; } = string.Empty;

    [JsonProperty("originalName")]
    public string OriginalName { get; set; } = string.Empty;

    [JsonProperty("mimetype")]
    public string Mimetype { get; set; } = string.Empty;

    [JsonProperty("size")]
    public long Size { get; set; }

    [JsonProperty("thumbnailFilename")]
    public string? ThumbnailFilename { get; set; }

    [JsonProperty("width")]
    public int? Width { get; set; }

    [JsonProperty("height")]
    public int? Height { get; set; }

    [JsonProperty("duration")]
    public double? Duration { get; set; }

    [JsonProperty("playable")]
    public bool? Playable { get; set; }

    [JsonProperty("sourceUrl")]
    public string? SourceUrl { get; set; }

    [JsonProperty("federationStatus")]
    public string? FederationStatus { get; set; }

    [JsonProperty("federationMeta")]
    public string? FederationMeta { get; set; }

    [JsonProperty("createdAt")]
    public long CreatedAt { get; set; }
}

public class Embed
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;

    [JsonProperty("embedType")]
    public string EmbedType { get; set; } = string.Empty;

    [JsonProperty("provider")]
    public string? Provider { get; set; }

    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("image")]
    public string? Image { get; set; }

    [JsonProperty("embedUrl")]
    public string? EmbedUrl { get; set; }

    [JsonProperty("width")]
    public int? Width { get; set; }

    [JsonProperty("height")]
    public int? Height { get; set; }

    [JsonProperty("color")]
    public string? Color { get; set; }

    [JsonProperty("createdAt")]
    public long CreatedAt { get; set; }
}

public class Reaction
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("emoji")]
    public string Emoji { get; set; } = string.Empty;

    [JsonProperty("user")]
    public User User { get; set; } = new();

    [JsonProperty("createdAt")]
    public long CreatedAt { get; set; }
}
