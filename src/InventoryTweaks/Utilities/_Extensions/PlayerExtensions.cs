using System.Diagnostics.CodeAnalysis;

namespace InventoryTweaks.Utilities;

/// <summary>
///     Provides <see cref="Player" /> extension methods.
/// </summary>
public static class PlayerExtensions
{
    /// <summary>
    ///     Attempts to retrieve the inventory of the chest currently opened by the specified player.
    /// </summary>
    /// <param name="player">The player whose open chest inventory is to be retrieved.</param>
    /// <param name="inventory">
    ///     When this method returns <c>true</c>, contains the inventory items of the opened chest.
    ///     When this method returns <c>false</c>, contains <c>null</c>.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the player has an open chest and the inventory was retrieved successfully;
    ///     otherwise, <c>false</c>.
    /// </returns>
    public static bool TryGetChest(this Player player, [MaybeNullWhen(false)] out Item[] inventory)
    {
        var hasChest = player.chest != -1;

        if (hasChest)
        {
            inventory = Main.chest[player.chest].item;

            return true;
        }

        inventory = null;

        return false;
    }

    /// <summary>
    ///     Attempts to retrieve the inventory of the piggy bank currently opened by the specified player.
    /// </summary>
    /// <param name="player">The player whose open piggy bank inventory is to be retrieved.</param>
    /// <param name="inventory">
    ///     When this method returns <c>true</c>, contains the inventory items of the opened piggy bank.
    ///     When this method returns <c>false</c>, contains <c>null</c>.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the player has an open piggy bank and the inventory was retrieved successfully;
    ///     otherwise, <c>false</c>.
    /// </returns>
    public static bool TryGetPiggy(this Player player, [MaybeNullWhen(false)] out Item[] inventory)
    {
        var hasPiggy = player.chest == -2;

        if (hasPiggy)
        {
            inventory = player.bank.item;

            return true;
        }

        inventory = null;

        return false;
    }

    /// <summary>
    ///     Attempts to retrieve the inventory of the safe currently opened by the specified player.
    /// </summary>
    /// <param name="player">The player whose open safe inventory is to be retrieved.</param>
    /// <param name="inventory">
    ///     When this method returns <c>true</c>, contains the inventory items of the opened safe.
    ///     When this method returns <c>false</c>, contains <c>null</c>.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the player has an open safe and the inventory was retrieved successfully;
    ///     otherwise, <c>false</c>.
    /// </returns>
    public static bool TryGetSafe(this Player player, [MaybeNullWhen(false)] out Item[] inventory)
    {
        var hasSafe = player.chest == -3;

        if (hasSafe)
        {
            inventory = player.bank2.item;

            return true;
        }

        inventory = null;

        return false;
    }

    /// <summary>
    ///     Attempts to retrieve the inventory of the void vault currently opened by the specified player.
    /// </summary>
    /// <param name="player">The player whose open void vault inventory is to be retrieved.</param>
    /// <param name="inventory">
    ///     When this method returns <c>true</c>, contains the inventory items of the opened void vault.
    ///     When this method returns <c>false</c>, contains <c>null</c>.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the player has an open void vault and the inventory was retrieved successfully;
    ///     otherwise, <c>false</c>.
    /// </returns>
    public static bool TryGetVoidVault(this Player player, [MaybeNullWhen(false)] out Item[] inventory)
    {
        var hasVoid = player.chest == -5;

        if (hasVoid)
        {
            inventory = player.bank4.item;

            return true;
        }

        inventory = null;

        return false;
    }


    /// <summary>
    ///     Attempts to retrieve the inventory of any container currently opened by the specified player,
    ///     including chests, piggy banks, safes, and void vaults.
    /// </summary>
    /// <param name="player">The player whose open container inventory is to be retrieved.</param>
    /// <param name="inventory">
    ///     When this method returns <c>true</c>, contains the inventory items of the opened container.
    ///     When this method returns <c>false</c>, contains <c>null</c>.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the player is accessing any supported container and the inventory was
    ///     retrieved successfully; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryGetContainer(this Player player, [MaybeNullWhen(false)] out Item[] inventory)
    {
        if (player.TryGetChest(out inventory))
        {
            return true;
        }

        if (player.TryGetPiggy(out inventory))
        {
            return true;
        }

        if (player.TryGetSafe(out inventory))
        {
            return true;
        }

        if (player.TryGetVoidVault(out inventory))
        {
            return true;
        }

        inventory = null;

        return false;
    }
}