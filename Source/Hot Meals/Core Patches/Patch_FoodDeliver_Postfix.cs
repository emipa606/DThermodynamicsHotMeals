using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse.AI;

namespace DHotMeals.Core_Patches;

[HarmonyPatch(typeof(JobDriver_FoodDeliver))]
[HarmonyPatch("MakeNewToils")]
internal class Patch_FoodDeliver_Postfix
{
    private static IEnumerable<Toil> Postfix(IEnumerable<Toil> values, JobDriver_FoodDeliver __instance,
        bool ___eatingFromInventory, bool ___usingNutrientPasteDispenser)
    {
        int numToilsBeforeGoto;
        if (___eatingFromInventory)
        {
            numToilsBeforeGoto = 1;
        }
        else if (___usingNutrientPasteDispenser)
        {
            numToilsBeforeGoto = 2;
        }
        else
        {
            numToilsBeforeGoto = 3;
        }

        foreach (var toil in HeatMealInjector.InjectHeat(values, __instance, numToilsBeforeGoto))
        {
            yield return toil;
        }
    }
}