using System.Linq;
using InventoryTweaks.Utilities;
using MonoMod.Cil;
using Terraria.Audio;
using Terraria.UI;

namespace InventoryTweaks.Core.Input;

// TODO: Cleanup this mess. v1.1.1 maybe?
public sealed class ItemDistributionSystem : ILoadable
{
    public static int LastInsertionSlot { get; private set; } = -1;
    
    public static bool Inserting { get; private set; }
    
    void ILoadable.Load(Mod mod)
    {
        On_Main.DoDraw += Main_DoDraw_Hook;
        
        On_ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick_Hook;
        On_ItemSlot.RefreshStackSplitCooldown += ItemSlot_RefreshStackSplitCooldown_Hook;
    }

    void ILoadable.Unload() { }
    
    private static void Main_DoDraw_Hook(On_Main.orig_DoDraw orig, Main self, GameTime gametime)
    {
        orig(self, gametime);

        if (!Inserting)
        {
            return;
        }

        Main.stackCounter = 0;
        Main.stackSplit = 30;
    }
    
    private static void ItemSlot_RefreshStackSplitCooldown_Hook(On_ItemSlot.orig_RefreshStackSplitCooldown orig)
    {
        orig();

        if (!Inserting)
        {
            return;
        }

        Main.stackCounter = 0;
        Main.stackSplit = 30;
    }

    private static void ItemSlot_RightClick_Hook(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
    {
        orig(inv, context, slot);
        
        var item = inv[slot];

        if (!ItemSlotUtils.IsInventoryContext(context) || ItemSlotUtils.IsNPCContext(context) || Main.mouseItem.IsAir)
        {
            return;
        }
        
        if (Main.mouseRight && slot != LastInsertionSlot && Main.stackSplit < 30)
        {
            Inserting = true;
            
            if (item.IsAir)
            {
                item.SetDefaults(Main.mouseItem.type);
                    
                Main.mouseItem.stack--;
            }
            else if (item.type == Main.mouseItem.type)
            {
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