using InventoryTweaks.Common.Configuration;
using InventoryTweaks.Utilities;
using JetBrains.Annotations;
using Terraria.UI;

namespace InventoryTweaks.Core.Input;

/// <summary>
///     Handles item distribution for item slots.
/// </summary>
/// <remarks>
///     <para>
///         Implements item distribution across item slots, allowing players to hold right-click
///         and smoothly distribute items into their inventories through hooks in
///         <see cref="ItemSlot" />.
///     </para>
///     <para>
///         Configuration options are available in <see cref="ClientConfiguration" />, including
///         toggles for enabling or disabling item distribution.
///     </para>
/// </remarks>
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public sealed class ItemDistributionManager : ILoadable
{
    /// <summary>
    ///     Gets or sets the index of the last item slot that the player used to insert an item.
    /// </summary>
    public static int LastInsertionSlot { get; private set; } = -1;

    /// <summary>
    ///     Gets or sets whether the player is inserting items.
    /// </summary>
    public static bool Inserting { get; private set; }

    /// <summary>
    ///     Gets the <see cref="ClientConfiguration" /> instance.
    /// </summary>
    private static ClientConfiguration Config => ClientConfiguration.Instance;

    void ILoadable.Load(Mod mod)
    {
        On_Main.DoDraw += Main_DoDraw_Hook;

        On_ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick_Hook;
        On_ItemSlot.RefreshStackSplitCooldown += ItemSlot_RefreshStackSplitCooldown_Hook;
    }

    void ILoadable.Unload() { }

    // Handles stalling the stack split cooldown.
    private static void Main_DoDraw_Hook(On_Main.orig_DoDraw orig, Main self, GameTime gameTime)
    {
        orig(self, gameTime);

        if (!Config.EnableDistribution || !Inserting)
        {
            return;
        }

        Main.stackCounter = 0;
        Main.stackSplit = 30;
    }

    // Handles stalling the stack split cooldown.
    private static void ItemSlot_RefreshStackSplitCooldown_Hook(On_ItemSlot.orig_RefreshStackSplitCooldown orig)
    {
        orig();

        if (!Config.EnableDistribution || !Inserting)
        {
            return;
        }

        Main.stackCounter = 0;
        Main.stackSplit = 30;
    }

    private static void ItemSlot_RightClick_Hook(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
    {
        orig(inv, context, slot);

        if (!Config.EnableDistribution || !ItemSlotUtils.IsInventoryContext(context) || ItemSlotUtils.IsNPCContext(context) || Main.mouseItem.IsAir)
        {
            return;
        }

        var item = inv[slot];

        if (Main.mouseRight && (slot != LastInsertionSlot || Main.mouseRightRelease) && Main.stackSplit < 30)
        {
            Inserting = true;

            if (item.IsAir)
            {
                item.SetDefaults(Main.mouseItem.type);

                Main.mouseItem.stack--;
            }
            else if (item.type == Main.mouseItem.type)
            {
                if (item.IsFull())
                {
                    return;
                }

                item.stack++;

                Main.mouseItem.stack--;
            }
        }
        else
        {
            Inserting = false;
        }

        LastInsertionSlot = slot;
    }
}