using System.Collections.Generic;
using System.Linq;
using BackspaceClient.Game.Online.API.Response;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class MessageContentContainer : FillFlowContainer
{
    private readonly List<MessageView> messages = [];

    public bool IsCanBeConsecutive => messages.Last().Model.Attachments.Count == 0 || messages.Last().Model.ReplyTo.Value == null;

    public User WhoseMessage
    {
        get
        {
            if (field != null) return field;
            if (messages.Count > 0)
                return messages.First().Model.User;
            else
                return null;
        }
        set;
    } = null;

    public MessageContentContainer(MessageView ms) => Add(ms);

    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = true;
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
    }

    public void Add(MessageView message)
    {
        messages.Add(message);

        base.Add(message);
    }
}
