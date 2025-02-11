﻿using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse.AI;

namespace DHotMeals.Core_Patches;

[HarmonyPatch(typeof(JobDriver_FoodFeedPatient), "MakeNewToils")]
public static class JobDriver_FoodFeedPatient_MakeNewToils
{
    public static IEnumerable<Toil> Postfix(IEnumerable<Toil> values, JobDriver_FoodFeedPatient __instance)
    {
        var numToilsBeforeGoto = 2;
        if (__instance.pawn.inventory != null && __instance.pawn.inventory.Contains(__instance.job.targetA.Thing))
        {
            numToilsBeforeGoto--;
        }

        foreach (var toil in HeatMealInjector.InjectHeat(values, __instance, numToilsBeforeGoto))
        {
            yield return toil;
        }
    }
}