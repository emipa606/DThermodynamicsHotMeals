using RimWorld;
using Verse;

namespace DHotMeals;

public static class MealSorter
{
    public static bool IsRawTasty(ThingDef def)
    {
        if (def.ingestible.preferability == FoodPreferability.RawTasty)
        {
            return true;
        }

        return def.ingestible.joyKind != null && IsRawResource(def);
    }

    public static bool IsRawResource(ThingDef def)
    {
        if (def.GetStatValueAbstract(StatDefOf.Nutrition) == 0)
        {
            return false;
        }

        var ing = def.ingestible;
        if (ing != null)
        {
            if (ing.preferability == FoodPreferability.RawBad)
            {
                return true;
            }

            var ft = ing.foodType;
            var compare = FoodTypeFlags.Meat | FoodTypeFlags.Corpse | FoodTypeFlags.VegetableOrFruit |
                          FoodTypeFlags.AnimalProduct | FoodTypeFlags.Seed | FoodTypeFlags.Plant;
            if ((compare & ft) != FoodTypeFlags.None)
            {
                return true;
            }
        }

        if (def.thingCategories == null)
        {
            return false;
        }

        return def.thingCategories.Contains(DefDatabase<ThingCategoryDef>.GetNamed("RC2_FruitsRaw", false))
               || def.thingCategories.Contains(ThingCategoryDefOf.MeatRaw)
               || def.thingCategories.Contains(ThingCategoryDefOf.PlantFoodRaw)
               || def.thingCategories.Contains(ThingCategoryDefOf.PlantMatter)
               || def.thingCategories.Any(x => x.defName == "CookingSupplies");
    }

    public static bool IsExcluded(ThingDef def)
    {
        if (def.category == ThingCategory.Plant)
        {
            return true;
        }

        var drugCompProp = def.GetCompProperties<CompProperties_Drug>();
        if (drugCompProp != null && drugCompProp.chemical != ChemicalDefOf.Alcohol
                                 && drugCompProp.chemical != DefDatabase<ChemicalDef>.GetNamed("RC2_Caffeine", false)
                                 && drugCompProp.chemical !=
                                 DefDatabase<ChemicalDef>.GetNamed("RC2_AmbrosiaAlcohol", false))
        {
            return true;
        }

        if (def.ingestible.joyKind == null)
        {
            return false;
        }

        return def.ingestible.joyKind.defName == "Chemical" &&
               (def.ingestible.foodType & FoodTypeFlags.Fluid) == FoodTypeFlags.None;
    }

    public static bool IsNonPerishable(ThingDef def)
    {
        var ing = def.ingestible;
        if (ing == null)
        {
            return false;
        }

        var types = FoodTypeFlags.Meal | FoodTypeFlags.Processed;
        if ((ing.foodType & types) == FoodTypeFlags.None)
        {
            return false;
        }

        var rot = def.GetCompProperties<CompProperties_Rottable>();
        return rot == null || rot.daysToRotStart > 25;
    }

    public static bool IsHotDrink(ThingDef def)
    {
        var ing = def.ingestible;
        if (ing != null)
        {
            // this one is hard without fixed definitions
        }

        return false;
    }

    public static bool IsColdDrink(ThingDef def)
    {
        var ing = def.ingestible;
        if (ing == null)
        {
            return false;
        }

        if (ing.ingestCommandString != null && ing.ingestCommandString.ToLower().Contains("drink"))
        {
            return true;
        }

        if (ing.nurseable)
        {
            return true;
        }

        var ft = ing.foodType;
        var filter = FoodTypeFlags.Liquor | FoodTypeFlags.Fluid;
        if ((ft & filter) != FoodTypeFlags.None)
        {
            return true;
        }

        var drugCompProp = def.GetCompProperties<CompProperties_Drug>();
        return drugCompProp != null && (drugCompProp.chemical == ChemicalDefOf.Alcohol ||
                                        drugCompProp.chemical !=
                                        DefDatabase<ChemicalDef>.GetNamed("RC2_Caffeine", false));
    }

    public static bool IsHotMeal(ThingDef def)
    {
        if (def.GetStatValueAbstract(StatDefOf.Nutrition) == 0)
        {
            return false;
        }

        var ing = def.ingestible;
        if (ing == null)
        {
            return false;
        }

        if (ing.preferability == FoodPreferability.NeverForNutrition)
        {
            return false;
        }

        var ft = ing.foodType;
        var compare =
            FoodTypeFlags
                .Meal; //| FoodTypeFlags.Meat | FoodTypeFlags.Corpse | FoodTypeFlags.VegetableOrFruit | FoodTypeFlags.AnimalProduct;
        return (compare & ft) != FoodTypeFlags.None;
    }

    public static bool IsColdMeal(ThingDef def)
    {
        var ing = def.ingestible;
        if (ing == null)
        {
            return false;
        }

        return def.thingCategories != null &&
               def.thingCategories.Contains(DefDatabase<ThingCategoryDef>.GetNamed("SweetMeals", false));
    }

    public static bool IsRace(ThingDef def)
    {
        return def.race != null;
    }
}