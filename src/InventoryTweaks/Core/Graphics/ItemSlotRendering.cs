using InventoryTweaks.Common.Configuration;
using InventoryTweaks.Utilities;
using JetBrains.Annotations;
using Terraria.Audio;
using Terraria.UI;

namespace InventoryTweaks.Core.Graphics;

/// <summary>
/// 
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public sealed class ItemSlotRendering : ILoadable
{
    /// <summary>
    /// 
    /// </summary>
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    private sealed class ItemSlotRenderingGlobalItem : GlobalItem
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

    /// <summary>
    ///     Gets the <see cref="ClientConfiguration" /> instance.
    /// </summary>
    private static ClientConfiguration Config => ClientConfiguration.Instance;

    void ILoadable.Load(Mod mod)
    {
        On_ItemSlot.DrawItemIcon += ItemSlot_DrawItemIcon_Hook;
    }
    
    void ILoadable.Unload() { }

    private static float ItemSlot_DrawItemIcon_Hook
    (
        On_ItemSlot.orig_DrawItemIcon orig,
        Item item,
        int context,
        SpriteBatch spriteBatch,
        Vector2 screenPositionForItemCenter,
        float scale,
        float sizeLimit,
        Color environmentColor
    )
    {
        if (!ItemSlotUtils.IsInventoryContext(context))
        {
            return orig(item, context, spriteBatch, screenPositionForItemCenter, scale, sizeLimit, environmentColor);
        }

        UpdateMovementEffects(item, ref screenPositionForItemCenter);
        UpdateScaleEffects(item, ref scale);
        UpdateHoverEffects(item, in screenPositionForItemCenter);

        return orig(item, context, spriteBatch, screenPositionForItemCenter, scale, sizeLimit, environmentColor);
    }

    private static void UpdateMovementEffects(Item item, ref Vector2 position)
    {
        if (!Config.EnableMovementEffects || !item.TryGetGlobalItem(out ItemSlotRenderingGlobalItem graphics))
        {
            return;
        }

        graphics.InventoryDrawPosition = graphics.InventoryDrawPosition.HasValue ? Vector2.SmoothStep(graphics.InventoryDrawPosition.Value, position, 0.5f) : position;

        if (item == Main.mouseItem)
        {
            return;
        }

        position = graphics.InventoryDrawPosition.Value;
    }

    private static void UpdateScaleEffects(Item item, ref float scale)
    {
        if (!Config.EnableEffects || !item.TryGetGlobalItem(out ItemSlotRenderingGlobalItem graphics))
        {
            return;
        }

        var hovering = (Config.EnableHoverEffects && graphics.Hovering) || (Config.EnableMouseEffects && Main.mouseItem == item) || (Config.EnableSelectedEffects && Main.LocalPlayer.HeldItem == item);

        graphics.DrawScale = MathHelper.SmoothStep(graphics.DrawScale, hovering ? Config.HoveredItemScale : Config.UnhoveredItemScale, 0.5f);

        scale = graphics.DrawScale;
    }

    private static void UpdateHoverEffects(Item item, in Vector2 position)
    {
        if (!Config.EnableHoverEffects || !item.TryGetGlobalItem(out ItemSlotRenderingGlobalItem graphics))
        {
            return;
        }

        var hitbox = new Rectangle((int)position.X - 20, (int)position.Y - 20, 40, 40);

        graphics.Hovering = hitbox.Contains(Main.MouseScreen.ToPoint());

        if (Config.EnableInventorySounds && graphics.JustHovered)
        {
            SoundEngine.PlaySound(in SoundID.MenuTick);
        }

        graphics.OldHovering = graphics.Hovering;
    }
}