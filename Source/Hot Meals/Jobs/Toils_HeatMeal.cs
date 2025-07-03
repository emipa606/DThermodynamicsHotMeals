using System.Collections.Generic;
using DHotMeals.Comps;
using RimWorld;
using Verse;
using Verse.AI;

namespace DHotMeals;

public static class Toils_HeatMeal
{
    private static readonly List<ThingDef> allowedHeaters = [Base.DefOf.DMicrowave];

    public static Thing FindPlaceToHeatFood(Thing food, Pawn pawn, float searchRadius = -1f, Thing searchNear = null)
    {
        searchNear ??= food;

        if (searchRadius < 1f)
        {
            searchRadius = HotMealsSettings.searchRadius;
        }

        var result = GenClosest.ClosestThing_Regionwise_ReachablePrioritized(searchNear.PositionHeld,
            searchNear.MapHeld, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial),
            PathEndMode.Touch, TraverseParms.For(pawn), searchRadius, valid,
            thing => thing.GetStatValue(StatDefOf.WorkTableWorkSpeedFactor));
        return result;

        bool valid(Thing m)
        {
            if (m.def.building is not { isMealSource: true })
            {
                return false;
            }

            if (!HotMealsSettings.useCookingAppliances && !allowedHeaters.Contains(m.def))
            {
                return false;
            }

            var compRefuelable = m.TryGetComp<CompRefuelable>();
            if (compRefuelable is { HasFuel: false })
            {
                return false;
            }

            var compPowerTrader = m.TryGetComp<CompPowerTrader>();
            if (compPowerTrader is { PowerOn: false })
            {
                return false;
            }

            return !m.IsForbidden(pawn) && (HotMealsSettings.multipleHeat || pawn.CanReserve(m, 1, 1));
        }
    }

    public static Toil HeatMeal(TargetIndex foodIndex = TargetIndex.A, TargetIndex heaterIndex = TargetIndex.C)
    {
        var toil = new Toil();
        toil.initAction = delegate
        {
            var actor = toil.actor;
            var curJob = actor.jobs.curJob;
            var food = curJob.GetTarget(foodIndex).Thing;
            var comp = food.TryGetComp<CompDFoodTemperature>();
            if (comp == null)
            {
                Log.Error("Tried to heat food with no temperatureingestible comp");
                return;
            }

            var heater = curJob.GetTarget(heaterIndex).Thing;
            comp.initialHeatingTemp = comp.curTemp;
            comp.SetHeatUpRate(heater);
        };

        toil.tickIntervalAction = delegate(int delta)
        {
            var actor = toil.actor;
            var curJob = actor.jobs.curJob;
            var food = curJob.GetTarget(foodIndex).Thing;
            var comp = food.TryGetComp<CompDFoodTemperature>();
            if (comp == null)
            {
                Log.Error("Tried to heat food with no temperature ingestible comp");
                actor.jobs.curDriver.ReadyForNextToil();
                return;
            }

            if (comp.curTemp >= comp.targetCookingTemp)
            {
                actor.jobs.curDriver.ReadyForNextToil();
                return;
            }

            if (toil.actor.CurJob.GetTarget(heaterIndex).Thing is IBillGiverWithTickAction billGiverWithTickAction)
            {
                billGiverWithTickAction.UsedThisTick();
            }

            actor.GainComfortFromCellIfPossible(delta, true);
            comp.HeatUp();
        };
        toil.defaultCompleteMode = ToilCompleteMode.Never;
        var cook = DefDatabase<EffecterDef>.GetNamed("Cook");
        toil.WithEffect(() => cook, foodIndex);
        var cooking = DefDatabase<SoundDef>.GetNamed("Recipe_CookMeal");
        toil.PlaySustainerOrSound(() => cooking);
        toil.WithProgressBar(foodIndex, delegate
        {
            var actor = toil.actor;
            var curJob = actor.CurJob;
            var food = curJob.GetTarget(foodIndex).Thing;
            var comp = food.TryGetComp<CompDFoodTemperature>();
            if (comp == null)
            {
                return 1f;
            }

            var target = comp.targetCookingTemp - comp.initialHeatingTemp;
            var progress = comp.curTemp - comp.initialHeatingTemp;
            return (float)(progress / target);
        });
        toil.activeSkill = () => SkillDefOf.Cooking;
        return toil;
    }
}