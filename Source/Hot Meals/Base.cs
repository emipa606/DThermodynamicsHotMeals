using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DHotMeals;

[StaticConstructorOnStartup]
public static class Base
{
    public static readonly bool VGPRunning;
    public static readonly bool RimFridgeRunning;
    public static readonly Type CompRefrigeratorType;
    public static readonly FieldInfo RimFridgeTempField;

    static Base()
    {
        if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "VGP Vegetable Garden"))
        {
            VGPRunning = true;
        }

        RimFridgeRunning = ModLister.GetActiveModWithIdentifier("rimfridge.kv.rw", true) != null;

        if (RimFridgeRunning)
        {
            CompRefrigeratorType = AccessTools.TypeByName("RimFridge.CompRefrigerator");
            RimFridgeTempField = CompRefrigeratorType.GetField("currentTemp");
        }

        foreach (var def in DefDatabase<ThingDef>.AllDefs)
        {
            if (MealSorter.IsRace(def))
            {
                //MealAssigner.addRace(def); // not today
                continue;
            }

            if (def.ingestible == null)
            {
                continue;
            }

            if (IsExplicitlyDefined(def))
            {
                continue;
            }

            if (MealSorter.IsExcluded(def))
            {
                continue;
            }

            if (MealSorter.IsRawTasty(def))
            {
                MealAssigner.AddRawTastyMeal(def);
            }
            else if (MealSorter.IsHotDrink(def))
            {
                MealAssigner.AddHotDrink(def);
            }
            else if (MealSorter.IsColdDrink(def))
            {
                MealAssigner.AddColdDrink(def);
            }
            else if (MealSorter.IsRawResource(def))
            {
                MealAssigner.AddRawResource(def);
            }
            else if (MealSorter.IsNonPerishable(def))
            {
                MealAssigner.AddNonperishableMeal(def);
            }
            else if (MealSorter.IsColdMeal(def))
            {
                MealAssigner.AddColdMeal(def);
            }
            else if (MealSorter.IsHotMeal(def))
            {
                MealAssigner.AddHotMeal(def);
            }
        }

        foreach (var tc in MealAssigner.AllCats())
        {
            tc.ResolveReferences();
            tc.PostLoad();
        }

        ResourceCounter.ResetDefs();
        DefDatabase<ThingCategoryDef>.ResolveAllReferences();
        DefDatabase<RecipeDef>.ResolveAllReferences();
    }

    private static bool IsExplicitlyDefined(ThingDef def)
    {
        var s = MealAssigner.fixedTypes.Keys.ToList()
            .FindLast(x => def.defName.ToLower() == x.ToLower() || def.label.ToLower() == x.ToLower());
        if (s == null)
        {
            return false;
        }

        var category = MealAssigner.fixedTypes[s];
        switch (category)
        {
            case MealTempTypes.None:
                break;
            case MealTempTypes.HotMeal:
                MealAssigner.AddHotMeal(def);
                break;
            case MealTempTypes.ColdMeal:
                MealAssigner.AddColdMeal(def);
                break;
            case MealTempTypes.HotDrink:
                MealAssigner.AddHotDrink(def);
                break;
            case MealTempTypes.ColdDrink:
                MealAssigner.AddColdDrink(def);
                break;
            case MealTempTypes.RoomTempMeal:
                MealAssigner.AddRoomTemperatureMeal(def);
                break;
            case MealTempTypes.NonPerishable:
                MealAssigner.AddNonperishableMeal(def);
                break;
            case MealTempTypes.RawTasty:
                MealAssigner.AddRawTastyMeal(def);
                break;
            case MealTempTypes.RawResource:
                MealAssigner.AddRawResource(def);
                break;
        }

        return true;
    }


    [RimWorld.DefOf]
    public static class DefOf
    {
        // microwave
        public static ThingDef DMicrowave;

        // thing categories
        public static ThingCategoryDef DFoodHotMeals;
        public static ThingCategoryDef DFoodColdMeals;
        public static ThingCategoryDef DFoodHotDrinks;
        public static ThingCategoryDef DFoodColdDrinks;
        public static ThingCategoryDef DFoodRTMeals;
        public static ThingCategoryDef DFoodNonperishable;

        public static ThingCategoryDef DFoodRawTasty;

        // job
        public static JobDef HeatMeal;

        // thoughts
        public static ThoughtDef DAteGoodThing;
        public static ThoughtDef DAteTooHot;
        public static ThoughtDef DAteTooCold;
        public static ThoughtDef DAteMeh;
    }
}