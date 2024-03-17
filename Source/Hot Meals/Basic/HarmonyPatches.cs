using System.Reflection;
using HarmonyLib;
using Verse;

namespace DHotMeals;

public class HarmonyPatches : Mod
{
    public HarmonyPatches(ModContentPack content) : base(content)
    {
        new Harmony("io.github.dametri.hotmeals").PatchAll(Assembly.GetExecutingAssembly());
    }
}