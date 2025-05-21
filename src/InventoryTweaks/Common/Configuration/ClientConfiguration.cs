using JetBrains.Annotations;
using Terraria.ModLoader.Config;

namespace InventoryTweaks.Common.Configuration;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public sealed partial class ClientConfiguration : ModConfig
{
    /// <summary>
    ///     Gets the <see cref="ClientConfiguration" /> instance.
    /// </summary>
    public static ClientConfiguration Instance => ModContent.GetInstance<ClientConfiguration>();

    public override ConfigScope Mode { get; } = ConfigScope.ClientSide;
}