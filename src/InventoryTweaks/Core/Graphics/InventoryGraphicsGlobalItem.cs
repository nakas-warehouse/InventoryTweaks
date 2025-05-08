namespace InventoryTweaks.Core.Graphics;

public sealed class InventoryGraphicsGlobalItem : GlobalItem
{
    /// <summary>
    ///     The inventory draw scale of the item attached to this global.
    /// </summary>
    public float DrawScale { get; internal set; }

    /// <summary>
    ///     The inventory draw position of the item attached to this global.
    /// </summary>
    public Vector2? InventoryDrawPosition { get; internal set; }
    
    /// <summary>
    ///     Whether the item attached to this global is being hovered over or not.
    /// </summary>
    public bool Hovering { get; internal set; }
    
    /// <summary>
    ///     Whether the item attached to this global was being hovered over or not.
    /// </summary>
    public bool OldHovering { get; internal set; }

    public bool JustHovered => Hovering && !OldHovering;
    
    public override bool InstancePerEntity { get; } = true;
}