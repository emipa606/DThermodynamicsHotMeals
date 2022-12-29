using DHotMeals;
using HarmonyLib;
using RimWorld;

namespace DThermodynamicsCore.Core_Patches;

[HarmonyPatch(typeof(CompRottable))]
[HarmonyPatch("TicksUntilRotAtTemp")]
internal class Patch_TicksUntilRotAtTemp_Prefix
{
    public static bool Prefix(ref float temp, CompRottable __instance)
    {
        if (!HotMealsSettings.warmersSlowRot)
        {
            return true;
        }

        var rotter = __instance.parent;
        var map = rotter.Map;
        var thingList = map?.thingGrid.ThingsListAt(rotter.PositionHeld);
        if (thingList == null)
        {
            return true;
        }

        foreach (var thing in thingList)
        {
            if (thing is not Building_HeatedStorage bhs || !bhs.IsActive())
            {
                continue;
            }

            temp = 1f;
            break;
        }

        return true;
    }
}