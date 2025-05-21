using InventoryTweaks.Common.Configuration;
using InventoryTweaks.Core.Enums;
using JetBrains.Annotations;
using MonoMod.Cil;
using Terraria.UI;

namespace InventoryTweaks.Core.Input;

/// <summary>
///     Handles item pickup behavior when selecting items from an item slot.
/// </summary>
/// <remarks>
///     <para>
///         Modifies how items are picked up from an item slot through edits in
///         <see cref="ItemSlot"/>, allowing the player to pick up different quantities from a stack.
///     </para>
///     <para>
///         Configuration options are available in <see cref="ClientConfiguration" />, including
///         the stacking strategy.
///     </para>
/// </remarks>
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public sealed class ItemPickupManager : ILoadable
{
    void ILoadable.Load(Mod mod)
    {
        IL_ItemSlot.PickupItemIntoMouse += ItemSlot_PickupItemIntoMouse_Edit;
    }

    void ILoadable.Unload() { }

    private static void ItemSlot_PickupItemIntoMouse_Edit(ILContext il)
    {
        try
        {
            var c = new ILCursor(il);

            if (!c.TryGotoNext(MoveType.After, static i => i.MatchLdcI4(1)))
            {
                throw new Exception();
            }

            c.EmitLdarg0();
            c.EmitLdarg2();

            c.EmitDelegate
            (
                static (int value, Item[] inv, int slot) =>
                {
                    var config = ClientConfiguration.Instance;

                    var item = inv[slot];

                    var stack = config.StackType switch
                    {
                        StackType.Single => value,
                        StackType.Half => item.stack / 2,
                        StackType.Full => item.stack,
                        _ => value
                    };

                    return Math.Max(stack, 1);
                }
            );
        }
        catch (Exception)
        {
            MonoModHooks.DumpIL(InventoryTweaks.Instance, il);
        }
    }
}