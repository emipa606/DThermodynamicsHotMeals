using System.Collections.Generic;
using System.Reflection;
using DHotMeals;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace DThermodynamicsCore.Core_Patches;

[HarmonyPatch]
public class JobDriver_Serve_MakeNewToils
{
    public static bool Prepare()
    {
        return ModsConfig.IsActive("Orion.Gastronomy");
    }

    public static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method("Gastronomy.Waiting.JobDriver_Serve:MakeNewToils");
    }

    public static IEnumerable<Toil> Postfix(IEnumerable<Toil> values, JobDriver __instance)
    {
        foreach (var toil in HeatMealInjector.InjectHeat(values, __instance, 4, TargetIndex.B))
        {
            yield return toil;
        }
    }
}