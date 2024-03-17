using System.Reflection;
using HarmonyLib;
using Verse;

namespace DThermodynamicsCore;

internal class HarmonyPatches : Mod
{
    public HarmonyPatches(ModContentPack content) : base(content)
    {
        new Harmony("io.github.dametri.thermodynamicscore").PatchAll(Assembly.GetExecutingAssembly());
    }
}