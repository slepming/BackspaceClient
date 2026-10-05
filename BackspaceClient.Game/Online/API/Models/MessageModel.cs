#nullable enable
using System.Collections.Generic;
using System.Linq;
using BackspaceClient.Game.Online.API.Response;
using osu.Framework.Bindables;

namespace BackspaceClient.Game.Online.API.Models;

public class MessageModel
{
    public string Id { get; }
    public string ChannelId { get; }
    public string UserId { get; }
    public long CreatedAt { get; }

    public Bindable<string?> Content { get; } = new();
    public Bindable<long?> EditedAt { get; } = new();
    public Bindable<string?> ReplyToId { get; } = new();
    public BindableList<Reaction> Reactions { get; private set; } = new();
    public BindableList<Embed> Embeds { get; private set; } = new();
    public BindableList<Attachment> Attachments { get; private set; } = new();

    public MessageModel(MessageWithUserResponse msg)
    {
        Id = msg.Id;
        ChannelId = msg.ChannelId;
        UserId = msg.UserId;
        CreatedAt = msg.CreatedAt;
        UpdateFromResponse(msg);
    }

    public void UpdateFromResponse(MessageWithUserResponse msg)
    {
        Content.Value = msg.Content;
        EditedAt.Value = msg.EditedAt;
        ReplyToId.Value = msg.ReplyToId;
        Reactions = new(msg.Reactions ?? new List<Reaction>());
        Embeds = new(msg.Embeds ?? new List<Embed>());
        Attachments = new(msg.Attachments ?? new List<Attachment>());
    }

    public void AddReaction(Reaction reaction)
    {
        var updated = Reactions.ToList();

        var existing = updated.FindIndex(r => r.User.Id == reaction.User.Id && r.Emoji == reaction.Emoji);
        if (existing >= 0)
            return;

        updated.Add(reaction);
        Reactions = new(updated);
    }

    public void RemoveReaction(string userId, string emoji)
    {
        var updated = Reactions.ToList();
        updated.RemoveAll(r => r.User.Id == userId && r.Emoji == emoji);
        Reactions = new(updated);
    }
}
