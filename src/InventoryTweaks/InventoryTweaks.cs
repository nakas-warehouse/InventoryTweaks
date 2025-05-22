using JetBrains.Annotations;

namespace InventoryTweaks;

/// <summary>
///     The <see cref="Mod"/> implementation of Inventory Tweaks.
/// </summary>
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public sealed class InventoryTweaks : Mod
{
    /// <summary>
    ///     Gets the <see cref="InventoryTweaks"/> instance.
    /// </summary>
    public static InventoryTweaks Instance => ModContent.GetInstance<InventoryTweaks>();
}