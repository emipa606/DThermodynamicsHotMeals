using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse.AI;

namespace DHotMeals.Core_Patches;

[HarmonyPatch(typeof(JobDriver_SocialRelax))]
[HarmonyPatch("MakeNewToils")]
internal class Patch_SocialRelax_Postfix
{
    private static IEnumerable<Toil> Postfix(IEnumerable<Toil> values, JobDriver_SocialRelax __instance)
    {
        if (!__instance.job.GetTarget(TargetIndex.C).HasThing)
        {
            foreach (var value in values)
            {
                yield return value;
            }

            yield break;
        }

        var numToilsBeforeGoto = 2;

        foreach (var toil in HeatMealInjector.InjectHeat(values, __instance, numToilsBeforeGoto, TargetIndex.C,
                     TargetIndex.B))
        {
            yield return toil;
        }
    }
}