namespace InventoryTweaks.Core.Graphics;

public sealed class ItemSlotDataGlobalItem : GlobalItem
{
    /// <summary>
    ///     Gets or sets the inventory draw position of the item attached to this global.
    /// </summary>
    public Vector2? InventoryDrawPosition { get; internal set; }

    /// <summary>
    ///     Gets or sets the inventory draw scale of the item attached to this global.
    /// </summary>
    public float DrawScale { get; internal set; }

    /// <summary>
    ///     Gets or sets whether the item attached to this global is being hovered over.
    /// </summary>
    public bool Hovering { get; internal set; }

    /// <summary>
    ///     Gets or sets whether the item attached to this global was being hovered over.
    /// </summary>
    public bool OldHovering { get; internal set; }

    /// <summary>
    ///     Gets whether the item attached to this global has just been hovered.
    /// </summary>
    public bool JustHovered => Hovering && !OldHovering;

    public override bool InstancePerEntity { get; } = true;
}