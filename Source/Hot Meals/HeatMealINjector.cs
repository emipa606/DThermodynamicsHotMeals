using System.Collections.Generic;
using DHotMeals.Comps;
using Verse;
using Verse.AI;

namespace DHotMeals;

public static class HeatMealInjector
{
    public static IEnumerable<Toil> InjectHeat(IEnumerable<Toil> values, JobDriver jd, int num,
        TargetIndex foodIndex = TargetIndex.A, TargetIndex finalLocation = TargetIndex.C)
    {
        using var enumerator = values.GetEnumerator();
        for (var i = 0; i < num; i++)
        {
            enumerator.MoveNext();
            yield return enumerator.Current;
        }

        foreach (var toil in Heat(jd, foodIndex, finalLocation))
        {
            yield return toil;
        }


        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }

    public static IEnumerable<Toil> Heat(JobDriver jd, TargetIndex foodIndex = TargetIndex.A,
        TargetIndex finalLocation = TargetIndex.C, TargetIndex tableIndex = TargetIndex.None)
    {
        var oldFinal = jd.job.GetTarget(finalLocation);

        var empty = new Toil();
        yield return Toils_Jump.JumpIf(empty, delegate
        {
            var actor = empty.actor;
            var curJob = actor.jobs.curJob;
            LocalTargetInfo food = curJob.GetTarget(foodIndex).Thing;

            var comp = food.Thing?.TryGetComp<CompDFoodTemperature>();
            if (comp == null)
            {
                return true;
            }

            if (comp.PropsTemp.likesHeat)
            {
                return comp.curTemp >= comp.PropsTemp.tempLevels.goodTemp;
            }

            if (HotMealsSettings.thawIt && !comp.PropsTemp.okFrozen)
            {
                return comp.curTemp > 0;
            }

            return true;
        });
        Thing heater = null;
        var getHeater = new Toil();
        getHeater.initAction = delegate
        {
            var actor = getHeater.actor;
            var curJob = actor.jobs.curJob;
            var foodToHeat = curJob.GetTarget(foodIndex).Thing;
            Thing table = null;
            if (tableIndex != TargetIndex.None)
            {
                table = curJob.GetTarget(tableIndex).Thing;
            }

            heater = Toils_HeatMeal.FindPlaceToHeatFood(foodToHeat, actor, searchNear: table);
            if (heater != null)
            {
                curJob.SetTarget(finalLocation, heater);
            }
        };
        yield return getHeater;
        yield return Toils_Jump.JumpIf(empty, delegate
        {
            return heater == null;
        });
        if (!HotMealsSettings.multipleHeat)
        {
            yield return Toils_Reserve.Reserve(finalLocation);
            yield return Toils_Goto.GotoThing(finalLocation, PathEndMode.InteractionCell);
            yield return Toils_HeatMeal.HeatMeal(foodIndex, finalLocation).FailOnDespawnedNullOrForbiddenPlacedThings()
                .FailOnCannotTouch(finalLocation, PathEndMode.InteractionCell);
            yield return Toils_Reserve.Release(finalLocation);
        }
        else
        {
            yield return Toils_Goto.GotoThing(finalLocation, PathEndMode.Touch);
            yield return Toils_HeatMeal.HeatMeal(foodIndex, finalLocation).FailOnDespawnedNullOrForbiddenPlacedThings()
                .FailOnCannotTouch(finalLocation, PathEndMode.Touch);
        }

        yield return empty;
        if (oldFinal == LocalTargetInfo.Invalid)
        {
            yield break;
        }

        var resetC = new Toil();
        resetC.initAction = delegate
        {
            var actor = resetC.actor;
            var curJob = actor.jobs.curJob;
            curJob.SetTarget(finalLocation, oldFinal);
        };
        yield return resetC;
    }
}