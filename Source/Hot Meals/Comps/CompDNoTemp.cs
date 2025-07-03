using Verse;

namespace DThermodynamicsCore.Comps;

public class CompDNoTemp : ThingComp
{
    protected virtual CompProperties_DNoTemp PropsNoTemp => (CompProperties_DNoTemp)props;

    public override string CompInspectStringExtra()
    {
        return PropsNoTemp.inspectString;
    }
}