using System.ComponentModel;
using InventoryTweaks.Core.Enums;
using Terraria.ModLoader.Config;

namespace InventoryTweaks.Common.Configuration;

public sealed partial class ClientConfiguration
{
    [Header("Input")]
    [DefaultValue(true)]
    public bool EnableQuickShift { get; set; } = true;

    [DefaultValue(true)]
    public bool EnableQuickControl { get; set; } = true;

    [DefaultValue(true)]
    public bool EnableStackRefill { get; set; } = true;

    [DefaultValue(true)]
    public bool EnableMouseRefill { get; set; } = true;

    [DefaultValue(true)]
    public bool EnableDistribution { get; set; } = true;

    [DefaultValue(typeof(StackType), nameof(StackType.Single))]
    public StackType StackType { get; set; } = StackType.Single;

    [DefaultValue(typeof(SortType), nameof(SortType.Ascending))]
    public SortType SortType { get; set; } = SortType.Ascending;
}