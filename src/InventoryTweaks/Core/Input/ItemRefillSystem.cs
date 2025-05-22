using InventoryTweaks.Common.Configuration;
using InventoryTweaks.Core.Enums;
using InventoryTweaks.Utilities;
using JetBrains.Annotations;
using Terraria.Audio;

namespace InventoryTweaks.Core.Input;

/// <summary>
///     Handles automatic refilling of item stacks when consumed or manually triggered.
/// </summary>
/// <remarks>
///     <para>
///         When the player's held item stack is fully consumed, it is automatically refilled through
///         behavior implemented in <see cref="ItemRefillGlobalItem" />, handled in
///         <see cref="GlobalItem.OnConsumeItem" />.
///     </para>
///     <para>
///         Refill can also be manually triggered for the mouse item stack using a hotkey, handled in
///         <see cref="ModSystem.PostUpdateInput" />.
///     </para>
///     <para>
///         Configuration options are available in <see cref="ClientConfiguration" />, including the
///         sorting strategy and inventory feedback sounds.
///     </para>
/// </remarks>
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public sealed class ItemRefillSystem : ModSystem
{
    /// <summary>
    ///     Handles automatic refilling of item stacks when consumed.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         When the player's held item stack is fully consumed, it is automatically refilled through
    ///         behavior implemented in <see cref="OnConsumeItem" />.
    ///     </para>
    ///     <para>
    ///         Configuration options are available in <see cref="ClientConfiguration" />, including the
    ///         sorting strategy and inventory feedback sounds.
    ///     </para>
    /// </remarks>
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class ItemRefillGlobalItem : GlobalItem
    {
        public override void OnConsumeItem(Item item, Player player)
        {
            base.OnConsumeItem(item, player);

            if (!Config.EnableStackRefill)
            {
                return;
            }

            var refill = item == player.HeldItem && item.stack - 1 <= 0;

            if (!refill)
            {
                return;
            }

            var hasContainer = player.TryGetContainer(out var inventory);

            var refillInventory = hasContainer ? inventory : player.inventory;
            var refillLength = hasContainer ? inventory.Length : PLAYER_INVENTORY_LENGTH;

            RefillItemFromInventory(player.HeldItem, refillInventory, refillLength);
        }
    }

    /// <summary>
    ///     The length of the player's inventory. Excludes the 58th index, which is reserved for
    ///     <see cref="Main.mouseItem" />.
    /// </summary>
    public const int PLAYER_INVENTORY_LENGTH = 57;

    /// <summary>
    ///     Gets or sets the keybind for mouse item refill.
    /// </summary>
    public static ModKeybind Keybind { get; private set; }

    /// <summary>
    ///     Gets the <see cref="ClientConfiguration" /> instance.
    /// </summary>
    private static ClientConfiguration Config => ClientConfiguration.Instance;

    public override void Load()
    {
        base.Load();

        Keybind = KeybindLoader.RegisterKeybind(Mod, nameof(Keybind), "Mouse3");
    }

    public override void Unload()
    {
        base.Unload();

        Keybind = null;
    }

    public override void PostUpdateInput()
    {
        base.PostUpdateInput();

        if (!Keybind.JustPressed || !Config.EnableMouseRefill)
        {
            return;
        }

        RefillMouseItem();
    }

    public static void RefillMouseItem()
    {
        if (Main.mouseItem.IsAir || Main.mouseItem.IsFull())
        {
            return;
        }

        var player = Main.LocalPlayer;

        var hasContainer = player.TryGetContainer(out var inventory);

        var refillInventory = hasContainer ? inventory : player.inventory;
        var refillLength = hasContainer ? inventory.Length : PLAYER_INVENTORY_LENGTH;

        RefillItemFromInventory(Main.mouseItem, refillInventory, refillLength);
    }

    // TODO: Make this a utility method at "ItemUtils.cs".
    private static void RefillItemFromInventory(Item item, Item[] inventory, int length)
    {
        var indices = new int[length];

        for (var i = 0; i < length; i++)
        {
            indices[i] = i;
        }

        Array.Sort
        (
            indices,
            static (a, b) =>
            {
                var left = Main.LocalPlayer.inventory[a];
                var right = Main.LocalPlayer.inventory[b];

                return Config.SortType switch
                {
                    SortType.Ascending => left.stack.CompareTo(right.stack),
                    SortType.Descending => right.stack.CompareTo(left.stack),
                    _ => left.stack.CompareTo(right.stack)
                };
            }
        );

        var type = item.type;

        foreach (var index in indices)
        {
            var other = inventory[index];

            if (other == item || other.IsAir || other.type != type)
            {
                continue;
            }

            if (Config.EnableInventorySounds)
            {
                SoundEngine.PlaySound(in SoundID.MenuTick);
            }

            var stack = item.maxStack - item.stack;
            var transfer = Math.Min(other.stack, stack);

            other.stack -= transfer;

            item.stack += transfer;
        }
    }
}