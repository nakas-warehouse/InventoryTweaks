using System.Collections.Generic;
using InventoryTweaks.Common.Configuration;
using InventoryTweaks.Utilities;
using Terraria.Audio;
using Terraria.UI;

namespace InventoryTweaks.Core.Graphics;

public sealed class ItemSlotRendererSystem : ModSystem
{
    /// <summary>
    ///     The name of the interface layer used to render item slots.
    /// </summary>
    public const string LAYER_NAME = $"{nameof(InventoryTweaks)}";

    /// <summary>
    ///     Gets the <see cref="ClientConfiguration" /> instance.
    /// </summary>
    private static ClientConfiguration Config => ClientConfiguration.Instance;

    public override void Load()
    {
        base.Load();

        On_ItemSlot.DrawItemIcon += ItemSlot_DrawItemIcon_Hook;
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        base.ModifyInterfaceLayers(layers);

        var index = layers.FindIndex(static layer => layer.Name == "Vanilla: Inventory");

        if (index == -1)
        {
            return;
        }

        layers.Insert(index + 1, new LegacyGameInterfaceLayer(LAYER_NAME, DrawIcons));
    }

    private static bool DrawIcons()
    {
        return true;
    }

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
        if (!Config.EnableMovementEffects || !item.TryGetGlobalItem(out ItemSlotDataGlobalItem graphics))
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
        if (!Config.EnableEffects || !item.TryGetGlobalItem(out ItemSlotDataGlobalItem graphics))
        {
            return;
        }

        var hovering = (Config.EnableHoverEffects && graphics.Hovering) || (Config.EnableMouseEffects && Main.mouseItem == item) || (Config.EnableSelectedEffects && Main.LocalPlayer.HeldItem == item);

        graphics.DrawScale = MathHelper.SmoothStep(graphics.DrawScale, hovering ? Config.HoveredItemScale : Config.UnhoveredItemScale, 0.5f);

        scale = graphics.DrawScale;
    }

    private static void UpdateHoverEffects(Item item, in Vector2 position)
    {
        if (!Config.EnableHoverEffects || !item.TryGetGlobalItem(out ItemSlotDataGlobalItem graphics))
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