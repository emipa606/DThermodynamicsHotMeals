using System.Reflection;
using HarmonyLib;
using Verse;

namespace DHotMeals;

public class DHotMealsHarmonyPatches : Mod
{
    public DHotMealsHarmonyPatches(ModContentPack content) : base(content)
    {
        new Harmony("io.github.dametri.hotmeals").PatchAll(Assembly.GetExecutingAssembly());
    }
}