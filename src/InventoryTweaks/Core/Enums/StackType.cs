namespace InventoryTweaks.Core.Enums;

[Flags]
public enum StackType
{
    Single = 0,
    Half = 1 << 0,
    Full = 1 << 1 
}