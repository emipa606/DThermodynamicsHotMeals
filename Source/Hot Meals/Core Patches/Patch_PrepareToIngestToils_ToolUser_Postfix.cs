using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse.AI;

namespace DHotMeals.Core_Patches;

[HarmonyPatch(typeof(JobDriver_Ingest))]
[HarmonyPatch("PrepareToIngestToils_ToolUser")]
public static class Patch_PrepareToIngestToils_ToolUser_Postfix
{
    public static readonly List<Toil> carryToils = [];

    private static IEnumerable<Toil> Postfix(IEnumerable<Toil> values, JobDriver_Ingest __instance)
    {
        var food = __instance.job.GetTarget(TargetIndex.A);
        foreach (var baseToil in values)
        {
            if (carryToils.Contains(baseToil))
            {
                carryToils.Remove(baseToil);
                foreach (var toil in HeatMealInjector.Heat(__instance))
                {
                    yield return toil;
                }
            }

            yield return baseToil;
        }

        if (!food.Thing.def.IsDrug)
        {
            yield break;
        }

        foreach (var toil in HeatMealInjector.Heat(__instance))
        {
            yield return toil;
        }

        yield return Toils_Ingest.FindAdjacentEatSurface(TargetIndex.B, TargetIndex.A);
    }
}