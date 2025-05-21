using System.ComponentModel;
using Newtonsoft.Json;
using Terraria.ModLoader.Config;

namespace InventoryTweaks.Common.Configuration;

public sealed partial class ClientConfiguration
{
    [JsonIgnore]
    public bool EnableEffects => EnableHoverEffects || EnableMouseEffects || EnableSelectedEffects;

    [Header("Graphics")]
    [DefaultValue(true)]
    public bool EnableMovementEffects { get; set; } = true;

    [DefaultValue(true)]
    public bool EnableHoverEffects { get; set; } = true;

    [DefaultValue(true)]
    public bool EnableMouseEffects { get; set; } = true;

    [DefaultValue(true)]
    public bool EnableSelectedEffects { get; set; } = true;

    [Increment(0.05f)]
    [Range(0.8f, 2f)]
    [DefaultValue(1.2f)]
    public float HoveredItemScale { get; set; } = 1.2f;

    [Increment(0.05f)]
    [Range(0.4f, 1f)]
    [DefaultValue(0.8f)]
    public float UnhoveredItemScale { get; set; } = 0.8f;
}