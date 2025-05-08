using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace InventoryTweaks.Core.Configuration;

public sealed partial class ClientConfiguration : ModConfig
{
    [Header("Audio")]
    [DefaultValue(true)]
    public bool EnableInventorySounds { get; set; } = true;
}