using DHotMeals.Comps;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace DHotMeals.Core_Patches;

[HarmonyPatch(typeof(Toils_Ingest))]
[HarmonyPatch("CarryIngestibleToChewSpot")]
internal class Patch_CarryIngestibleToChewSpot_Postfix
{
    public static void Postfix(Pawn pawn, TargetIndex ingestibleInd, Toil __result)
    {
        if (pawn.jobs.curDriver is not JobDriver_Ingest jdi)
        {
            return;
        }

        if (!pawn.RaceProps.ToolUser)
        {
            return;
        }

        var food = jdi.job.GetTarget(ingestibleInd);
        var comp = food.Thing.TryGetComp<CompDFoodTemperature>();
        if (comp == null)
        {
            return;
        }

        if (comp.PropsTemp.likesHeat || HotMealsSettings.thawIt && comp.curTemp < 0 && !comp.PropsTemp.okFrozen)
        {
            Patch_PrepareToIngestToils_ToolUser_Postfix.carryToils.Add(__result);
        }
    }
}