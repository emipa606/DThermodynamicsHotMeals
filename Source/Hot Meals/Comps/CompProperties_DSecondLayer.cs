using Verse;

namespace DHotMeals.Comps;

public class CompProperties_DSecondLayer : CompProperties
{
    private readonly AltitudeLayer altitudeLayer = AltitudeLayer.MoteOverhead;

    public GraphicData graphicData;

    public CompProperties_DSecondLayer()
    {
        compClass = typeof(CompDSecondLayer);
    }

    public float Altitude => altitudeLayer.AltitudeFor();
}