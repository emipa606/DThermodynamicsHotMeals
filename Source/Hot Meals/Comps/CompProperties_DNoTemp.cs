using Verse;

namespace DThermodynamicsCore.Comps;

public class CompProperties_DNoTemp : CompProperties
{
    public string inspectString;

    public CompProperties_DNoTemp(string inspect)
    {
        compClass = typeof(CompDNoTemp);
        inspectString = inspect;
    }
}