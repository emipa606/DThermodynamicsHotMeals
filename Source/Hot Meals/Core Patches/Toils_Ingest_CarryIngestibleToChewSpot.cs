using DHotMeals.Comps;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace DHotMeals.Core_Patches;

[HarmonyPatch(typeof(Toils_Ingest), nameof(Toils_Ingest.CarryIngestibleToChewSpot))]
internal class Toils_Ingest_CarryIngestibleToChewSpot
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
            JobDriver_Ingest_PrepareToIngestToils_ToolUser.carryToils.Add(__result);
        }
    }
}