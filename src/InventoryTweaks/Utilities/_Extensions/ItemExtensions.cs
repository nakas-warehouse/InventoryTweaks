namespace InventoryTweaks.Utilities;

/// <summary>
///     Provides <see cref="Item"/> extension methods.
/// </summary>
public static class ItemExtensions
{
    /// <summary>
    ///     Checks whether an item has a full stack or not.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns><c>true</c> if the item has a full stack; otherwise, <c>false</c>.</returns>
    public static bool IsFull(this Item item)
    {
        return item.stack >= item.maxStack;
    }
}