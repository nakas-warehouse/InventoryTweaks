using Terraria.ModLoader.Config;

namespace InventoryTweaks.Common.Configuration;

public sealed partial class ClientConfiguration : ModConfig
{
    public static ClientConfiguration Instance => ModContent.GetInstance<ClientConfiguration>();

    public override ConfigScope Mode { get; } = ConfigScope.ClientSide;
}