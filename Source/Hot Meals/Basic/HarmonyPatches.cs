using System.Reflection;
using HarmonyLib;
using Verse;

namespace DHotMeals;

public class HarmonyPatches : Mod
{
    public HarmonyPatches(ModContentPack content) : base(content)
    {
        var harmony = new Harmony("io.github.dametri.hotmeals");
        var assembly = Assembly.GetExecutingAssembly();
        harmony.PatchAll(assembly);
    }
}