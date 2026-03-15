using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse.AI;

namespace DHotMeals.Core_Patches;

[HarmonyPatch(typeof(JobDriver_Ingest), "PrepareToIngestToils_ToolUser")]
public static class JobDriver_Ingest_PrepareToIngestToils_ToolUser
{
    public static IEnumerable<Toil> Postfix(IEnumerable<Toil> values, JobDriver_Ingest __instance)
    {
        foreach (var baseToil in values)
        {
            if (baseToil.debugName == "CarryIngestibleToChewSpot")
            {
                foreach (var toil in HeatMealInjector.Heat(__instance))
                {
                    yield return toil;
                }
            }
            yield return baseToil;
        }
    }
}