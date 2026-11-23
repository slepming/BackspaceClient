#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace BackspaceClient.Game.Input.Bindings;

public partial class GlobalActionContainer : KeyBindingContainer<GlobalActions>, IHandleGlobalKeyboardInput, IKeyBindingHandler<GlobalActions>
{
    public override IEnumerable<IKeyBinding> DefaultKeyBindings => messengerBindings.Concat(chatBindings);

    private readonly IKeyBindingHandler<GlobalActions>? handler;

    public GlobalActionContainer(BackspaceClientGameBase? game) : base(matchingMode: KeyCombinationMatchingMode.Modifiers)
    {
        if (game is IKeyBindingHandler<GlobalActions> h)
            handler = h;
    }

    public static IEnumerable<KeyBinding> GetBindingsFor(GlobalActionCategories cat)
    {
        switch (cat)
        {
            case GlobalActionCategories.Messenger:
                return messengerBindings;
            case GlobalActionCategories.Chat:
                return chatBindings;
            default:
                throw new ArgumentOutOfRangeException(nameof(cat), cat, $"Unexpected {nameof(GlobalActionCategories)}");
        }
        ;
    }

    public static IEnumerable<GlobalActions> GetBindingsActionsFor(GlobalActionCategories cat) =>
        GetBindingsFor(cat).Select(binding => binding.Action).Cast<GlobalActions>().Distinct();

    public bool OnPressed(KeyBindingPressEvent<GlobalActions> e) => handler?.OnPressed(e) == true;

    public void OnReleased(KeyBindingReleaseEvent<GlobalActions> e) => handler?.OnReleased(e);

    private static IEnumerable<KeyBinding> messengerBindings =>
    [
        new KeyBinding(new[] {InputKey.Control, InputKey.E}, GlobalActions.OpenSideBar),
    ];

    private static IEnumerable<KeyBinding> chatBindings =>
    [
        new KeyBinding(new[] {InputKey.Control, InputKey.C}, GlobalActions.ClearAttachments),
    ];
}

public enum GlobalActions
{
    OpenRecentMessengerCards,
    OpenSideBar,
    ClearAttachments
}

public enum GlobalActionCategories
{
    Messenger,
    Chat
}
