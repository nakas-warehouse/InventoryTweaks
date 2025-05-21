using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace InventoryTweaks.Common.Configuration;

public sealed partial class ClientConfiguration
{
    [Header("Audio")]
    [DefaultValue(true)]
    public bool EnableInventorySounds { get; set; } = true;
}