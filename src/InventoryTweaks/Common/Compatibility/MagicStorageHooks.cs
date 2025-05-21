using System.Reflection;
using JetBrains.Annotations;
using MagicStorage;
using MagicStorage.UI.States;

namespace InventoryTweaks.Common.Compatibility;

/// <summary>
/// 
/// </summary>
[JITWhenModsEnabled("MagicStorage")]
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public sealed class MagicStorageHooks : ILoadable
{
    private delegate Item GetRecipeCallback(CraftingUIState.RecipesPage self, int slot, ref int context);

    private delegate Item GetHeaderCallback(int slot, ref int context);

    bool ILoadable.IsLoadingEnabled(Mod mod)
    {
        return ModLoader.HasMod("MagicStorage");
    }

    void ILoadable.Load(Mod mod)
    {
        MonoModHooks.Add(typeof(CraftingGUI).GetMethod("GetHeader", BindingFlags.NonPublic | BindingFlags.Static), CraftingGUI_GetHeader_Hook);
        MonoModHooks.Add(typeof(CraftingUIState).GetNestedType("RecipesPage").GetMethod("GetRecipe", BindingFlags.NonPublic | BindingFlags.Instance), CraftingUIState_GetRecipe_Hook);
    }

    void ILoadable.Unload() { }

    // Makes the item a clone instead of a reference to prevent the same instance being modified with different parameters at once.
    private static Item CraftingGUI_GetHeader_Hook(GetHeaderCallback orig, int slot, ref int context)
    {
        return orig(slot, ref context).Clone();
    }

    // Makes the item a clone instead of a reference to prevent the same instance being modified with different parameters at once.
    private static Item CraftingUIState_GetRecipe_Hook(GetRecipeCallback orig, CraftingUIState.RecipesPage self, int slot, ref int context)
    {
        return orig(self, slot, ref context).Clone();
    }
}