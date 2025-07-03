using RimWorld;
using Verse;

namespace DThermodynamicsCore.Comps;

public class CompDTemperatureIngestible : CompDTemperature
{
    public new virtual CompProperties_DTemperatureIngestible PropsTemp => (CompProperties_DTemperatureIngestible)props;

    public override string GetState(double temp)
    {
        return base.GetState(temp);
    }

    protected virtual ThoughtDef GetIngestMemory()
    {
        return null;
    }

    protected virtual void MakeIngestMemory(ThoughtDef memory, Pawn ingester)
    {
    }

    public override void PostIngested(Pawn ingester)
    {
        var temp = ingester.TryGetComp<CompDTemperature>();
        if (temp != null)
        {
            temp.curTemp += (curTemp - temp.curTemp) / 5.0f;
        }

        if (ingester.needs.mood != null)
        {
            var memory = GetIngestMemory();
            if (memory != null)
            {
                MakeIngestMemory(memory, ingester);
            }
        }

        base.PostIngested(ingester);
    }

    public override string CompInspectStringExtra()
    {
        var s = GetState(curTemp);
        return s != "" ? $"{base.CompInspectStringExtra()} ({s})" : base.CompInspectStringExtra();
    }
}