using System.Collections.Generic;
using DThermodynamicsCore.Comps;
using RimWorld;
using Verse;
using Verse.AI;

namespace DHotMeals.Comps;

public class CompDFoodTemperature : CompDTemperatureIngestible
{
    public double heatUpRate = 0.1;

    public double initialHeatingTemp = 0f;
    public double targetCookingTemp;

    public new virtual CompProperties_DFoodTemperature PropsTemp => props as CompProperties_DFoodTemperature;

    public virtual void SetHeatUpRate(Thing heater)
    {
        var workRate = heater.GetStatValue(StatDefOf.WorkTableWorkSpeedFactor);
        if (workRate > 0)
        {
            heatUpRate = 0.25 * workRate * HotMealsSettings.heatSpeedMult;
        }
        else
        {
            heatUpRate = 0.125 * HotMealsSettings.heatSpeedMult;
        }
    }

    public virtual void HeatUp()
    {
        curTemp += heatUpRate;
    }

    public string GetFoodType()
    {
        if (PropsTemp.displayName != null)
        {
            return PropsTemp.displayName;
        }

        var s = "";
        if (PropsTemp.roomTemperature)
        {
            s += "HoMe.Lukewarm".Translate();
        }
        else if (PropsTemp.likesHeat)
        {
            s += "HoMe.Hot".Translate();
        }
        else
        {
            s += "HoMe.Cold".Translate();
        }

        s += " ";
        if (PropsTemp.isDrink)
        {
            s += "HoMe.drink".Translate();
        }
        else
        {
            s += "HoMe.meal".Translate();
        }

        return s;
    }

    public int GetPositiveMoodEffect(double temp)
    {
        if (PropsTemp.roomTemperature)
        {
            if (temp >= Levels.goodTemp && temp < Levels.okTemp)
            {
                return 1;
            }

            return 0;
        }

        if (PropsTemp.likesHeat)
        {
            return temp > Levels.goodTemp ? 1 : 0;
        }

        if (temp < 0 && !PropsTemp.okFrozen)
        {
            return 0;
        }

        return temp < Levels.goodTemp ? 1 : 0;
    }

    public int GetNegativeMoodEffect(double temp)
    {
        if (PropsTemp.roomTemperature)
        {
            if (temp < 0 && !PropsTemp.okFrozen)
            {
                return -3;
            }

            if (temp < Levels.goodTemp) // too cold
            {
                return -1;
            }

            return temp < Levels.okTemp
                ? // between good (lower bound) and ok (upper bound); just right
                0
                :
                // too hot
                1;
        }

        if (PropsTemp.likesHeat)
        {
            if (temp < 0 && !PropsTemp.okFrozen)
            {
                return -3;
            }

            if (temp > Levels.okTemp)
            {
                return 0;
            }

            if (temp > Levels.badTemp)
            {
                return -1;
            }

            if (temp > Levels.reallyBadTemp)
            {
                return -2;
            }

            // colder than reallyBadTemp (default: 0.01 celsius)
            return -3;
        }

        if (temp < 0 && !PropsTemp.okFrozen)
        {
            return -3;
        }

        if (temp < Levels.okTemp)
        {
            return 0;
        }

        if (temp < Levels.badTemp)
        {
            return 1;
        }

        return temp < Levels.reallyBadTemp
            ? 2
            :
            // hotter than really bad temp
            3;
    }


    public override string GetState(double temp)
    {
        if (PropsTemp.noHeat)
        {
            if (curTemp > 0)
            {
                return "HoMe.edible".Translate();
            }

            return "HoMe.frozensolid".Translate();
        }

        if (GetPositiveMoodEffect(temp) == 1)
        {
            return "HoMe.justright".Translate();
        }

        var negLevel = GetNegativeMoodEffect(temp);
        if (negLevel == -3)
        {
            return "HoMe.frozensolid".Translate();
        }

        if (negLevel == -2)
        {
            return "HoMe.toocold".Translate();
        }

        if (negLevel == -1)
        {
            return "HoMe.toochilly".Translate();
        }

        if (negLevel == 0)
        {
            return "HoMe.edible".Translate();
        }

        if (negLevel == 1)
        {
            return "HoMe.toowarm".Translate();
        }

        if (negLevel == 2)
        {
            return "HoMe.toohot".Translate();
        }

        if (negLevel == 3)
        {
            return "HoMe.scalding".Translate();
        }

        return "";
    }

    public override ThoughtDef GetIngestMemory()
    {
        if (GetPositiveMoodEffect(curTemp) > 0)
        {
            return Base.DefOf.DAteGoodThing;
        }

        var negEffect = GetNegativeMoodEffect(curTemp);
        switch (negEffect)
        {
            case < 0:
                return Base.DefOf.DAteTooCold;
            case > 0:
                return Base.DefOf.DAteTooHot;
            default:
                return Base.DefOf.DAteMeh;
        }
    }

    public override void MakeIngestMemory(ThoughtDef memory, Pawn ingester)
    {
        var foodMem = (Thought_MealTemp)ThoughtMaker.MakeThought(memory);
        foodMem.createdTemp = curTemp;
        foodMem.comp = this;
        ingester.needs.mood.thoughts.memories.TryGainMemory(foodMem);
    }

    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
        if (PropsTemp.roomTemperature)
        {
            targetCookingTemp = PropsTemp.tempLevels.okTemp - 5;
        }
        else if (PropsTemp.likesHeat)
        {
            targetCookingTemp = PropsTemp.tempLevels.goodTemp + 20;
        }
        else
        {
            targetCookingTemp = 0;
        }
    }

    public override string CompInspectStringExtra()
    {
        return $"{GetFoodType()}, {base.CompInspectStringExtra()}";
    }

    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
    {
        JobFailReason.Clear();
        if (!PropsTemp.likesHeat && (!HotMealsSettings.thawIt || PropsTemp.okFrozen) ||
            !selPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) || parent.stackCount <= 0)
        {
            yield break;
        }

        Thing heater = null;
        if (curTemp >= PropsTemp.tempLevels.goodTemp + 5)
        {
            JobFailReason.Is("alreadyHeated".Translate());
        }
        else if ((heater = Toils_HeatMeal.FindPlaceToHeatFood(parent, selPawn, 9999)) == null)
        {
            JobFailReason.Is("noPlaceToHeat".Translate());
        }
        else if (!selPawn.CanReserve(parent))
        {
            var pawn = selPawn.Map.reservationManager.FirstRespectedReserver(parent, selPawn);
            if (pawn == null)
            {
                pawn = selPawn.Map.physicalInteractionReservationManager.FirstReserverOf(selPawn);
            }

            JobFailReason.Is(pawn != null ? "ReservedBy".Translate(pawn.LabelShort, pawn) : "Reserved".Translate());
        }


        Job job = null;
        if (heater != null && parent.stackCount > 0)
        {
            job = JobMaker.MakeJob(Base.DefOf.HeatMeal);
            job.targetA = parent;
            job.targetC = heater;
            job.count = parent.stackCount;
        }

        if (JobFailReason.HaveReason)
        {
            string command;
            command = "CannotGenericWorkCustom".Translate(PropsTemp.likesHeat
                ? "heatMeal".Translate(parent.Label)
                : "thawMeal".Translate(parent.Label));

            command += $": {JobFailReason.Reason.CapitalizeFirst()}";
            yield return new FloatMenuOption(command, null);
            JobFailReason.Clear();
        }
        else if (job != null)
        {
            string command;
            command = PropsTemp.likesHeat
                ? "heatMeal".Translate(parent.Label).CapitalizeFirst()
                : "thawMeal".Translate(parent.Label).CapitalizeFirst();

            yield return new FloatMenuOption(command, delegate { selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc); });
        }
    }
}