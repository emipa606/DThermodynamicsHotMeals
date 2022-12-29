using System.Reflection;
using HarmonyLib;
using Verse;

namespace DThermodynamicsCore;

internal class HarmonyPatches : Mod
{
    public HarmonyPatches(ModContentPack content) : base(content)
    {
        var harmony = new Harmony("io.github.dametri.thermodynamicscore");
        var assembly = Assembly.GetExecutingAssembly();
        harmony.PatchAll(assembly);
    }
}