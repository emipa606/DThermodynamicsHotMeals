using DHotMeals;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DThermodynamicsCore.Core_Patches;

[HarmonyPatch(typeof(CompRottable), nameof(CompRottable.CompInspectStringExtra))]
internal class CompRottable_RotCompInspectStringExtra
{
    public static void Postfix(ref string __result, CompRottable __instance)
    {
        if (!HotMealsSettings.warmersSlowRot)
        {
            return;
        }

        var rotter = __instance.parent;
        var map = rotter.Map;
        var thingList = map?.thingGrid.ThingsListAt(rotter.PositionHeld);
        if (thingList == null)
        {
            return;
        }

        foreach (var thing in thingList)
        {
            if (thing is not Building_HeatedStorage bhs || !bhs.IsActive())
            {
                continue;
            }

            __result = __result.Replace("NotRefrigerated".Translate(), "HoMe.Warmed".Translate());
            break;
        }
    }
}