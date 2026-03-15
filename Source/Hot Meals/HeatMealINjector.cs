using System.Collections.Generic;
using DHotMeals.Comps;
using Verse;
using Verse.AI;

namespace DHotMeals;

public static class HeatMealInjector
{
    public static IEnumerable<Toil> InjectHeat(IEnumerable<Toil> values, JobDriver jd, int num,
        TargetIndex foodIndex = TargetIndex.A, TargetIndex tableIndex = TargetIndex.None)
    {
        int currentIndex = 0;
        foreach (var toil in values)
        {
            if (currentIndex == num)
            {
                foreach (var heatToil in Heat(jd, foodIndex, tableIndex))
                {
                    yield return heatToil;
                }
            }
            yield return toil;
            currentIndex++;
        }
    }

    public static IEnumerable<Toil> Heat(JobDriver jd, TargetIndex foodIndex = TargetIndex.A,
        TargetIndex tableIndex = TargetIndex.None)
    {
        var exit = ToilMaker.MakeToil("ExitPoint");
        var clean = ToilMaker.MakeToil("clean");
        var curJob = jd.job;
        clean.initAction = delegate
        {
            var queue = curJob.GetTargetQueue(TargetIndex.B);
            if (!queue.NullOrEmpty())
            {
                curJob.SetTarget(TargetIndex.C, queue[0]);
                queue.RemoveAt(0);
            }
        };
        yield return Toils_Jump.JumpIf(exit, delegate
        {
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
        var getHeater = ToilMaker.MakeToil("GetHeaterToil");
        getHeater.initAction = delegate
        {
            var actor = getHeater.actor;
            var foodToHeat = curJob.GetTarget(foodIndex).Thing;
            Thing table = null;
            if (tableIndex != TargetIndex.None)
            {
                table = curJob.GetTarget(tableIndex).Thing;
            }

            curJob.GetTargetQueue(TargetIndex.B).Insert(0, curJob.GetTarget(TargetIndex.C));
            var heater = Toils_HeatMeal.FindPlaceToHeatFood(foodToHeat, actor, searchNear: table);
            curJob.SetTarget(TargetIndex.C, heater ?? LocalTargetInfo.Invalid);
        };
        yield return getHeater;
        yield return Toils_Jump.JumpIf(clean, () => !jd.job.GetTarget(TargetIndex.C).IsValid);
        if (!HotMealsSettings.multipleHeat)
        {
            yield return Toils_Reserve.Reserve(TargetIndex.C);
            yield return Toils_Goto.GotoThing(TargetIndex.C, PathEndMode.InteractionCell);
            yield return Toils_HeatMeal.HeatMeal(foodIndex, TargetIndex.C).FailOnDespawnedNullOrForbiddenPlacedThings()
                .FailOnCannotTouch(TargetIndex.C, PathEndMode.InteractionCell);
            yield return Toils_Reserve.Release(TargetIndex.C);
        }
        else
        {
            yield return Toils_Goto.GotoThing(TargetIndex.C, PathEndMode.Touch);
            yield return Toils_HeatMeal.HeatMeal(foodIndex, TargetIndex.C).FailOnDespawnedNullOrForbiddenPlacedThings()
                .FailOnCannotTouch(TargetIndex.C, PathEndMode.Touch);
        }

        yield return clean;
        yield return exit;
    }
}