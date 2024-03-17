using Verse;

// basically the same as CompProperties_TempControl, but they made it annoying to inherit and modify

namespace DThermodynamicsCore.Comps;

public class CompProperties_DTempControl : CompProperties
{
    public readonly float defaultTargetTemperature = 21f;
    public readonly float maxTargetTemperature = 50f;
    public readonly float minTargetTemperature = -50f;

    public CompProperties_DTempControl()
    {
        compClass = typeof(CompDTempControl);
    }
}