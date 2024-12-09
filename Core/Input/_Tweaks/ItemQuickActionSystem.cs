using System.Runtime.CompilerServices;
using InventoryTweaks.Core.Configuration;
using InventoryTweaks.Core.Input;
using InventoryTweaks.Utilities;
using MonoMod.Cil;
using Terraria.UI;

namespace InventoryTweaks.Core.Tweaks;

public sealed partial class ItemQuickActionSystem : ILoadable
{
    /// <summary>
    ///     The index of the last item slot that the player used to trash an item.
    /// </summary>
    public static int LastTrashSlot { get; private set; } = -1;
    
    /// <summary>
    ///     The index of the last item slot that the player used to equip an item.
    /// </summary>
    public static int LastEquipSlot { get; private set; } = -1;
    
    void ILoadable.Load(Mod mod)
    {
        On_ItemSlot.LeftClick_SellOrTrash += ItemSlot_LeftClick_SellOrTrash_Hook;
        On_ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick_Hook;
        
        IL_ItemSlot.LeftClick_ItemArray_int_int += ItemSlot_LeftClick_Edit;
        IL_ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick_Edit;
    }

    void ILoadable.Unload() { }

    private static bool ItemSlot_LeftClick_SellOrTrash_Hook(On_ItemSlot.orig_LeftClick_SellOrTrash orig, Item[] inv, int context, int slot)
    {
        var result = orig(inv, context, slot);
        
        var config = ClientConfiguration.Instance;

        if (result)
        {
            LastTrashSlot = slot;
        }

        return result;
    }
    
    private static void ItemSlot_RightClick_Hook(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
    {
        orig(inv, context, slot);
        
        var config = ClientConfiguration.Instance;
        
        if (!config.EnableQuickShift)
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
            
            c.EmitDelegate(static (Item[] inv, int context, int slot, ref bool value) =>
            {
                value |= CanQuickShift(context, slot);
                value |= CanQuickControl(context, slot);
                
                if (!ModLoader.HasMod("MagicStorage"))
                {
                    return;
                }

                value |= CanQuickShiftMagicStorage(inv, context, slot);
            });
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
                        return !ItemDistributionSystem.Inserting && ItemSlotUtils.IsInventoryContext(context) && (slot != LastEquipSlot || Main.mouseRightRelease);
                    }
                );
            }
        }
        catch (Exception)
        {
            MonoModHooks.DumpIL(InventoryTweaks.Instance, il);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CanQuickShiftMagicStorage(Item[] inv, int context, int slot)
    {
        var config = ClientConfiguration.Instance;

        return ItemSlotUtils.IsInventoryContext(context)
               && Main.mouseLeft 
               && config.EnableQuickShift 
               && ItemSlot.ShiftInUse 
               && MagicStorageUtils.IsStorageOpen(inv, context, slot);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CanQuickShift(int context, int slot)
    {
        var config = ClientConfiguration.Instance;

        return ItemSlotUtils.IsInventoryContext(context)
               && Main.mouseLeft
               && config.EnableQuickShift
               && ItemSlot.ShiftInUse
               && InputUtils.HasCursorOverride;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CanQuickControl(int context, int slot)
    {
        var config = ClientConfiguration.Instance;

        return ItemSlotUtils.IsInventoryContext(context)
               && Main.mouseLeft
               && config.EnableQuickControl
               && ItemSlot.ControlInUse
               && slot != LastTrashSlot
               && InputUtils.HasCursorOverride;
    }
}