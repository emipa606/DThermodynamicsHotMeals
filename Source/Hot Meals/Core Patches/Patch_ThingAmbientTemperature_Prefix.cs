using DThermodynamicsCore.Comps;
using HarmonyLib;
using Verse;

namespace DThermodynamicsCore.Core_Patches;

[HarmonyPatch(typeof(Thing))]
[HarmonyPatch("AmbientTemperature", MethodType.Getter)]
internal class Patch_ThingAmbientTemperature_Prefix
{
    public static bool Prefix(ref float __result, Thing __instance)
    {
        var comp = __instance.TryGetComp<CompDTemperature>();
        if (comp == null)
        {
            return true;
        }

        __result = comp.GetCurTemp();
        return false;
    }
}