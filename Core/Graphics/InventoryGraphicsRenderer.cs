using InventoryTweaks.Core.Configuration;
using InventoryTweaks.Utilities;
using Terraria.Audio;
using Terraria.UI;

namespace InventoryTweaks.Core.Graphics;

// TODO: Make items render above item slots regardless of their index to avoid layering issues.
public sealed class InventoryGraphicsRenderer : ILoadable
{
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
        if (!ItemSlotUtils.IsInventoryContext(context) || !item.TryGetGlobalItem(out InventoryGraphicsGlobalItem graphics))
        {
            return orig(item, context, spriteBatch, screenPositionForItemCenter, scale, sizeLimit, environmentColor);
        }

        var drawPosition = screenPositionForItemCenter;
        var drawScale = scale;

        var config = ClientConfiguration.Instance;

        if (config.EnableMovementEffects)
        {
            if (graphics.InventoryDrawPosition.HasValue)
            {
                graphics.InventoryDrawPosition = Vector2.SmoothStep(graphics.InventoryDrawPosition.Value, screenPositionForItemCenter, 0.5f);
            }
            else
            {
                graphics.InventoryDrawPosition = screenPositionForItemCenter;
            }

            if (item != Main.mouseItem)
            {
                drawPosition = graphics.InventoryDrawPosition.Value;
            }
        }

        var hitbox = new Rectangle
        (
            (int)screenPositionForItemCenter.X - 20,
            (int)screenPositionForItemCenter.Y - 20,
            40,
            40
        );

        graphics.Hovering = hitbox.Contains(Main.MouseScreen.ToPoint());

        if (config.EnableInventorySounds && graphics.JustHovered)
        {
            SoundEngine.PlaySound(in SoundID.MenuTick);
        }

        graphics.OldHovering = graphics.Hovering;

        if (config.EnableEffects)
        {
            var hovering = (config.EnableHoverEffects && graphics.Hovering)
                           || (config.EnableMouseEffects && Main.mouseItem == item)
                           || (config.EnableSelectedEffects && Main.LocalPlayer.HeldItem == item);

            graphics.DrawScale = MathHelper.SmoothStep(graphics.DrawScale, hovering ? config.HoveredItemScale : config.UnhoveredItemScale, 0.5f);

            drawScale = graphics.DrawScale;
        }

        return orig(item, context, spriteBatch, drawPosition, drawScale, sizeLimit, environmentColor);
    }
}