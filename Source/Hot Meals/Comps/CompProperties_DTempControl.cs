using Verse;

// basically the same as CompProperties_TempControl, but they made it annoying to inherit and modify

namespace DThermodynamicsCore.Comps;

public class CompProperties_DTempControl : CompProperties
{
    public float defaultTargetTemperature = 21f;
    public float maxTargetTemperature = 50f;
    public float minTargetTemperature = -50f;

    public CompProperties_DTempControl()
    {
        compClass = typeof(CompDTempControl);
    }
}