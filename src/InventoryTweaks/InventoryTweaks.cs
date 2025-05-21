using JetBrains.Annotations;

namespace InventoryTweaks;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public sealed class InventoryTweaks : Mod
{
    public static InventoryTweaks Instance => ModContent.GetInstance<InventoryTweaks>();
}