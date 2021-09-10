using HarmonyLib;

namespace ComboSplitter.HarmonyPatches
{
    [HarmonyPatch(typeof(ComboUIController), nameof(ComboUIController.HandleComboDidChange), MethodType.Normal)]
    public static class ComboUIControllerPatch
    {
        [HarmonyPrefix] internal static bool Prefix() { return false; }
    }
}
