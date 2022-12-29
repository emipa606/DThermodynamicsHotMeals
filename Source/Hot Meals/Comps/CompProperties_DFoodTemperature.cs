using DThermodynamicsCore.Comps;

namespace DHotMeals.Comps;

public class CompProperties_DFoodTemperature : CompProperties_DTemperatureIngestible
{
    public string displayName = null;
    public MealTempTypes mealType = MealTempTypes.None;
    public bool noHeat = false;

    public CompProperties_DFoodTemperature()
    {
        compClass = typeof(CompDFoodTemperature);
    }
}