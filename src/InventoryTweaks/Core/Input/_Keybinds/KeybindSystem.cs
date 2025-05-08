using Microsoft.Xna.Framework.Input;

namespace InventoryTweaks.Core.Input;

public sealed class KeybindSystem : ModSystem
{
    public static ModKeybind MouseRefillKeybind { get; private set; }

    public override void Load()
    {
        base.Load();

        MouseRefillKeybind = KeybindLoader.RegisterKeybind(Mod, nameof(MouseRefillKeybind), "Mouse3");
    }

    public override void Unload()
    {
        base.Unload();

        MouseRefillKeybind = null;
    }
}