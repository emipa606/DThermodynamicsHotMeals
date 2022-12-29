using Verse;

namespace DThermodynamicsCore.Comps;

public class CompProperties_DTemperature : CompProperties
{
    public double diffusionTime = 5000;

    public float initialTemp = 21f; // 70 fahrenheit
    public bool likesHeat = true;
    public DTemperatureLevels tempLevels = new DTemperatureLevels(); // ~100 fahrenheit

    public int tickRate = -1;

    public CompProperties_DTemperature()
    {
        compClass = typeof(CompDTemperature);
        if (tickRate == -1)
        {
            tickRate = Rand.RangeInclusive(200, 250);
        }
    }
}