﻿namespace DThermodynamicsCore.Comps;

public class CompProperties_DTemperatureIngestible : CompProperties_DTemperature
{
    public bool isDrink = false;
    public bool okFrozen = false;
    public bool roomTemperature = false;

    protected CompProperties_DTemperatureIngestible()
    {
        compClass = typeof(CompDTemperatureIngestible);
    }
}