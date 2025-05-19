using System.Text;
using DHotMeals.Comps;
using RimWorld;
using Verse;

namespace DHotMeals;

internal class StatWorker_FoodType : StatWorker
{
    public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
    {
        if (req.HasThing && req.Thing.def.HasComp(typeof(CompDFoodTemperature)))
        {
            return 0;
        }

        return -1;
    }


    private void BuildColdString(CompDFoodTemperature comp, ref StringBuilder s)
    {
        var levels = comp.PropsTemp.tempLevels;
        s.AppendLine();
        s.AppendLine("HoMe.IdealTempUnder".Translate(levels.goodTemp.ToStringTemperature()));
        s.AppendLine(
            "HoMe.OKTemp".Translate(levels.goodTemp.ToStringTemperature(), levels.okTemp.ToStringTemperature()));
        s.AppendLine(
            "HoMe.BadTemp".Translate(levels.okTemp.ToStringTemperature(), levels.badTemp.ToStringTemperature()));
        s.AppendLine("HoMe.AwfulTempOver".Translate(levels.badTemp.ToStringTemperature()));
        if (comp.PropsTemp.okFrozen)
        {
            return;
        }

        s.AppendLine();
        s.AppendLine("HoMe.FrozenMoodWarning".Translate(GenText.ToStringTemperature(0)));
    }

    private void BuildHotString(CompDFoodTemperature comp, ref StringBuilder s)
    {
        var levels = comp.PropsTemp.tempLevels;
        s.AppendLine();
        s.AppendLine("HoMe.IdealTempOver".Translate(levels.goodTemp.ToStringTemperature()));
        s.AppendLine(
            "HoMe.OKTemp".Translate(levels.okTemp.ToStringTemperature(), levels.goodTemp.ToStringTemperature()));
        s.AppendLine(
            "HoMe.BadTemp".Translate(levels.badTemp.ToStringTemperature(), levels.okTemp.ToStringTemperature()));
        s.AppendLine("HoMe.AwfulTempUnder".Translate(levels.badTemp.ToStringTemperature()));
        if (comp.PropsTemp.okFrozen)
        {
            return;
        }

        s.AppendLine();
        s.AppendLine("HoMe.FrozenMoodWarning".Translate(GenText.ToStringTemperature(0)));
    }

    private void BuildRTString(CompDFoodTemperature comp, ref StringBuilder s)
    {
        var levels = comp.PropsTemp.tempLevels;
        s.AppendLine();
        s.AppendLine("HoMe.toohotInfo".Translate(levels.okTemp.ToStringTemperature()));
        s.AppendLine(
            "HoMe.IdealTemp".Translate(levels.goodTemp.ToStringTemperature(), levels.okTemp.ToStringTemperature()));
        s.AppendLine("HoMe.toocoldInfo".Translate(levels.goodTemp.ToStringTemperature()));
        if (comp.PropsTemp.okFrozen)
        {
            return;
        }

        s.AppendLine();
        s.AppendLine("HoMe.FrozenMoodWarning".Translate(GenText.ToStringTemperature(0)));
    }

    public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
    {
        var s = new StringBuilder();
        if (!req.HasThing)
        {
            return "";
        }

        var comp = req.Thing.TryGetComp<CompDFoodTemperature>();
        if (comp == null)
        {
            return s.ToString();
        }

        switch (comp.PropsTemp.mealType)
        {
            case MealTempTypes.HotMeal:
                s.AppendLine("HoMe.eatHot".Translate());
                BuildHotString(comp, ref s);
                break;
            case MealTempTypes.ColdMeal:
                s.AppendLine("HoMe.eatCold".Translate());
                BuildColdString(comp, ref s);
                break;
            case MealTempTypes.HotDrink:
                s.AppendLine("HoMe.drinkHot".Translate());
                BuildHotString(comp, ref s);
                break;
            case MealTempTypes.ColdDrink:
                s.AppendLine("HoMe.drinkCold".Translate());
                BuildColdString(comp, ref s);
                break;
            case MealTempTypes.RoomTempMeal:
                s.AppendLine("HoMe.eatMiddle".Translate());
                BuildRTString(comp, ref s);
                break;
            case MealTempTypes.RawResource:
                s.AppendLine("HoMe.rawStuff".Translate());
                break;
        }

        return s.ToString();
    }

    public override string GetExplanationFinalizePart(StatRequest req, ToStringNumberSense numberSense, float finalVal)
    {
        var s = new StringBuilder();
        s.AppendLine();
        s.AppendLine();
        s.AppendLine();
        s.AppendLine();
        s.AppendLine();
        s.AppendLine();
        s.AppendLine();
        s.AppendLine();
        return s.ToString();
    }

    public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense,
        StatRequest optionalReq, bool finalized = true)
    {
        if (!optionalReq.HasThing)
        {
            return "HoMe.None".Translate();
        }

        var comp = optionalReq.Thing.TryGetComp<CompDFoodTemperature>();
        if (comp != null)
        {
            return comp.GetFoodType();
        }

        return "HoMe.None".Translate();
    }
}