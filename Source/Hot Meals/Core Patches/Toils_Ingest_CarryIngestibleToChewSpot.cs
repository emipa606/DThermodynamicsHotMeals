//using System.Collections.Generic;
//using HarmonyLib;
//using RimWorld;
//using Verse.AI;

//namespace DHotMeals.Core_Patches;

//[HarmonyPatch(typeof(Toils_Ingest), nameof(Toils_Ingest.CarryIngestibleToChewSpot))]
//public static class Toils_Ingest_CarryIngestibleToChewSpot
//{
//    public static IEnumerable<Toil> Postfix(IEnumerable<Toil> values)
//    {
//        foreach (var toil in HeatMealInjector.Heat())
//        {
//            yield return toil;
//        }
//        foreach (var baseToil in values)
//        {
//            yield return baseToil;
//        }
//    }
//}