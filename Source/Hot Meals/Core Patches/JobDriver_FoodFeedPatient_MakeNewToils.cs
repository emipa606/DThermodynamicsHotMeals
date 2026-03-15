using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse.AI;

namespace DHotMeals.Core_Patches;

[HarmonyPatch(typeof(JobDriver_FoodFeedPatient), "MakeNewToils")]
public static class JobDriver_FoodFeedPatient_MakeNewToils
{
    public static IEnumerable<Toil> Postfix(IEnumerable<Toil> values, JobDriver_FoodFeedPatient __instance)
    {
        var currentIndex = 0;
        foreach (var toil in values)
        {
            if (currentIndex is 5 or 10 or 13)
            {
                foreach (var heatToil in HeatMealInjector.Heat(__instance))
                {
                    yield return heatToil;
                }
            }

            yield return toil;
            currentIndex++;
        }
    }
}