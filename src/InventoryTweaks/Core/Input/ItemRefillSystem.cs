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
    ///     When the player's held item stack is fully consumed, it is automatically refilled through
    ///     behavior implemented in <see cref="OnConsumeItem"/>.
    /// </remarks>
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    private sealed class ItemRefillGlobalItem : GlobalItem
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

            var length = player.inventory.Length;
            var indices = new int[length];

            for (var i = 0; i < length; i++)
            {
                indices[i] = i;
            }

            Array.Sort
            (
                indices,
                static (left, right) =>
                {
                    var leftItem = Main.LocalPlayer.inventory[left];
                    var rightItem = Main.LocalPlayer.inventory[right];

                    return Config.SortType switch
                    {
                        SortType.Ascending => leftItem.stack.CompareTo(rightItem.stack),
                        SortType.Descending => rightItem.stack.CompareTo(leftItem.stack),
                        _ => leftItem.stack.CompareTo(rightItem.stack)
                    };
                }
            );

            for (var i = 0; i < length; i++)
            {
                var index = indices[i];
                var inventory = player.inventory[index];

                if (inventory.IsAir || inventory == player.HeldItem || inventory.type != player.HeldItem.type)
                {
                    continue;
                }

                if (Config.EnableInventorySounds)
                {
                    SoundEngine.PlaySound(in SoundID.MenuTick);
                }

                var value = Math.Min(inventory.stack, player.HeldItem.maxStack);

                inventory.stack -= value;

                player.HeldItem.stack += value;

                break;
            }
        }
    }

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

        if (!Keybind.JustPressed)
        {
            return;
        }

        RefillMouseItem();
    }

    private static void RefillMouseItem()
    {
        if (!Config.EnableMouseRefill || Main.mouseItem.IsAir || Main.mouseItem.IsFull())
        {
            return;
        }

        var player = Main.LocalPlayer;

        var length = player.inventory.Length;
        var indices = new int[length];

        for (var i = 0; i < length; i++)
        {
            indices[i] = i;
        }

        Array.Sort
        (
            indices,
            static (left, right) =>
            {
                var leftItem = Main.LocalPlayer.inventory[left];
                var rightItem = Main.LocalPlayer.inventory[right];

                return Config.SortType switch
                {
                    SortType.Ascending => leftItem.stack.CompareTo(rightItem.stack),
                    SortType.Descending => rightItem.stack.CompareTo(leftItem.stack),
                    _ => leftItem.stack.CompareTo(rightItem.stack)
                };
            }
        );


        for (var i = 0; i < length; i++)
        {
            var index = indices[i];
            var item = player.inventory[index];

            if (item.IsAir || item.type != Main.mouseItem.type)
            {
                continue;
            }

            if (Config.EnableInventorySounds)
            {
                SoundEngine.PlaySound(in SoundID.MenuTick);
            }

            var stack = Main.mouseItem.maxStack - Main.mouseItem.stack;
            var value = Math.Min(item.stack, stack);

            item.stack -= value;

            Main.mouseItem.stack += value;
        }
    }
}