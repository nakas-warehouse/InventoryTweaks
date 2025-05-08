using System.Runtime.CompilerServices;
using Terraria.UI;

namespace InventoryTweaks.Utilities;

/// <summary>
///     Provides <see cref="ItemSlot" /> utilities.
/// </summary>
public static class ItemSlotUtils
{
    /// <summary>
    ///     Checks whether an item slot context represents an inventory slot or not.
    /// </summary>
    /// <param name="context">The context to check.</param>
    /// <returns><c>true</c> if the provided context represents an inventory slot; otherwise, <c>false</c>.</returns>
    public static bool IsInventoryContext(int context)
    {
        return context == ItemSlot.Context.InventoryItem
               || context == ItemSlot.Context.InventoryAmmo
               || context == ItemSlot.Context.InventoryCoin
               || context == ItemSlot.Context.ChestItem
               || context == ItemSlot.Context.BankItem
               || context == ItemSlot.Context.VoidItem
               || context == ItemSlot.Context.MouseItem
               || context == ItemSlot.Context.TrashItem
               || context == ItemSlot.Context.ShopItem
               || context == ItemSlot.Context.HotbarItem
               || context == ItemSlot.Context.GuideItem
               || context == ItemSlot.Context.PrefixItem
               || context == ItemSlot.Context.CreativeSacrifice
               || context == ItemSlot.Context.EquipArmor
               || context == ItemSlot.Context.EquipArmorVanity
               || context == ItemSlot.Context.EquipAccessory
               || context == ItemSlot.Context.EquipAccessoryVanity
               || context == ItemSlot.Context.EquipPet
               || context == ItemSlot.Context.EquipDye
               || context == ItemSlot.Context.EquipMiscDye
               || context == ItemSlot.Context.EquipLight
               || context == ItemSlot.Context.EquipMount
               || context == ItemSlot.Context.EquipGrapple
               || context == ItemSlot.Context.EquipMinecart;
    }

    /// <summary>
    ///     Checks whether an item slot context represents an NPC slot or not.
    /// </summary>
    /// <param name="context">The context to check.</param>
    /// <returns><c>true</c> if the provided context represents an NPC slot; otherwise, <c>false</c>.</returns>
    public static bool IsNPCContext(int context)
    {
        return context == ItemSlot.Context.ShopItem;
    }
}