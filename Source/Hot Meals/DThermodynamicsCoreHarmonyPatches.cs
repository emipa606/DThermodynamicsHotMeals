using System.Reflection;
using HarmonyLib;
using Verse;

namespace DThermodynamicsCore;

internal class DThermodynamicsCoreHarmonyPatches : Mod
{
    public DThermodynamicsCoreHarmonyPatches(ModContentPack content) : base(content)
    {
        new Harmony("io.github.dametri.thermodynamicscore").PatchAll(Assembly.GetExecutingAssembly());
    }
}