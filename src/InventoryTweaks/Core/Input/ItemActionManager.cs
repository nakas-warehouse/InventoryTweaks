using InventoryTweaks.Common.Configuration;
using InventoryTweaks.Utilities;
using MagicStorage;
using MonoMod.Cil;
using Terraria.UI;

namespace InventoryTweaks.Core.Input;

/// <summary>
///     Handles quick-shift and quick-control interactions for item slots.
/// </summary>
/// <remarks>
///     <para>
///         Implements quick-shift and quick-control behavior through hooks and edits in
///         <see cref="ItemSlot" />.
///     </para>
///     <para>
///         Support for these interactions is also implemented for the Magic Storage mod,
///         allowing consistent behavior across both standard and modded inventories.
///     </para>
///     <para>
///         Configuration options are available in <see cref="ClientConfiguration" />, including
///         toggles for enabling or disabling quick-shift and quick-control features.
///     </para>
/// </remarks>
public sealed class ItemActionManager : ILoadable
{
    /// <summary>
    ///     Gets or sets the index of the last item slot that the player used to trash an item.
    /// </summary>
    public static int LastTrashSlot { get; private set; } = -1;

    /// <summary>
    ///     Gets or sets the index of the last item slot that the player used to equip an item.
    /// </summary>
    public static int LastEquipSlot { get; private set; } = -1;

    /// <summary>
    ///     Gets the <see cref="ClientConfiguration" /> instance.
    /// </summary>
    private static ClientConfiguration Config => ClientConfiguration.Instance;

    void ILoadable.Load(Mod mod)
    {
        On_ItemSlot.LeftClick_SellOrTrash += ItemSlot_LeftClick_SellOrTrash_Hook;
        On_ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick_Hook;

        IL_ItemSlot.LeftClick_ItemArray_int_int += ItemSlot_LeftClick_Edit;
        IL_ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick_Edit;
    }

    void ILoadable.Unload() { }

    /// <summary>
    ///     Checks whether the player can quick-shift a given item slot from the Magic Storage mod.
    /// </summary>
    /// <param name="inv">The inventory of the item slot.</param>
    /// <param name="context">The context of the item slot.</param>
    /// <param name="slot">The index of the item slot.</param>
    /// <returns>
    ///     <c>true</c> if the player can quick-shift in the given item slot; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanQuickShiftMagicStorage(Item[] inv, int context, int slot)
    {
        return ItemSlotUtils.IsInventoryContext(context) && Main.mouseLeft && Config.EnableQuickShift && ItemSlot.ShiftInUse && IsStorageOpen(inv, context, slot);
    }

    /// <summary>
    ///     Checks whether the player can quick-shift a given item slot.
    /// </summary>
    /// <param name="context">The context of the item slot.</param>
    /// <returns>
    ///     <c>true</c> if the player can quick-shift in the given item slot; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanQuickShift(int context)
    {
        return ItemSlotUtils.IsInventoryContext(context) && Main.mouseLeft && Config.EnableQuickShift && ItemSlot.ShiftInUse && InputUtils.HasCursorOverride;
    }

    /// <summary>
    ///     Checks whether the player can quick-control on a given item slot.
    /// </summary>
    /// <param name="context">The context of the item slot.</param>
    /// <param name="slot">The index of the item slot.</param>
    /// <returns>
    ///     <c>true</c> if the player can quick-control in the given item slot; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanQuickControl(int context, int slot)
    {
        return ItemSlotUtils.IsInventoryContext(context) && Main.mouseLeft && Config.EnableQuickControl && ItemSlot.ControlInUse && slot != LastTrashSlot && InputUtils.HasCursorOverride;
    }

    // Handles caching the last item slot used to trash an item to prevent endless trashing.
    private static bool ItemSlot_LeftClick_SellOrTrash_Hook(On_ItemSlot.orig_LeftClick_SellOrTrash orig, Item[] inv, int context, int slot)
    {
        var result = orig(inv, context, slot);

        if (result)
        {
            LastTrashSlot = slot;
        }

        return result;
    }

    // Handles caching the last item slot used to equip an item to prevent endless equipping.
    private static void ItemSlot_RightClick_Hook(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
    {
        orig(inv, context, slot);

        if (!Config.EnableQuickShift)
        {
            return;
        }

        LastEquipSlot = slot;
    }

    private static void ItemSlot_LeftClick_Edit(ILContext il)
    {
        try
        {
            var c = new ILCursor(il);

            if (!c.TryGotoNext(MoveType.Before, static i => i.MatchStloc1()))
            {
                throw new Exception();
            }

            c.Index++;

            c.EmitLdarg0();
            c.EmitLdarg1();
            c.EmitLdarg2();

            c.EmitLdloca(1);

            c.EmitDelegate
            (
                static (Item[] inv, int context, int slot, ref bool value) =>
                {
                    value |= CanQuickShift(context);
                    value |= CanQuickControl(context, slot);

                    if (!ModLoader.HasMod("MagicStorage"))
                    {
                        return;
                    }

                    value |= CanQuickShiftMagicStorage(inv, context, slot);
                }
            );
        }
        catch (Exception)
        {
            MonoModHooks.DumpIL(InventoryTweaks.Instance, il);
        }
    }

    private static void ItemSlot_RightClick_Edit(ILContext il)
    {
        try
        {
            var c = new ILCursor(il);

            while (c.TryGotoNext(MoveType.After, static i => i.MatchLdsfld<Main>(nameof(Main.mouseRightRelease))))
            {
                c.EmitLdarg1();
                c.EmitLdarg2();

                c.EmitDelegate
                (
                    static (bool value, int context, int slot) =>
                    {
                        return !ItemDistributionManager.Inserting && ItemSlotUtils.IsInventoryContext(context) && (slot != LastEquipSlot || Main.mouseRightRelease);
                    }
                );
            }
        }
        catch (Exception)
        {
            MonoModHooks.DumpIL(InventoryTweaks.Instance, il);
        }
    }

    [JITWhenModsEnabled("MagicStorage")]
    private static bool IsStorageOpen(Item[] inv, int context, int slot)
    {
        var player = Main.LocalPlayer;

        if (!player.TryGetModPlayer(out StoragePlayer storagePlayer))
        {
            return false;
        }

        var hasContext = context == ItemSlot.Context.InventoryItem || context == ItemSlot.Context.InventoryCoin || context == ItemSlot.Context.InventoryAmmo;

        var item = inv[slot];

        var hasItem = item is { favorited: false, IsAir: false };
        var hasStorage = storagePlayer.ViewingStorage() is { X: > 0, Y: > 0 };

        return hasContext && hasItem && hasStorage;
    }
}