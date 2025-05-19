using System.Collections.Generic;
using DHotMeals.Comps;
using RimWorld;
using Verse;
using Verse.AI;

namespace DHotMeals;

internal class JobDriver_HeatMeal : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.GetTarget(TargetIndex.C), job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return ReserveFood();
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch)
            .FailOnDespawnedNullOrForbidden(TargetIndex.A);
        yield return Toils_Ingest.PickupIngestible(TargetIndex.A, pawn);
        var empty = new Toil();
        yield return Toils_Jump.JumpIf(empty, delegate
        {
            var food = job.GetTarget(TargetIndex.A);
            var comp = food.Thing.TryGetComp<CompDFoodTemperature>();
            if (comp == null)
            {
                return true;
            }

            if (comp.PropsTemp.likesHeat)
            {
                return comp.curTemp >= comp.targetCookingTemp;
            }

            return comp.curTemp > 0;
        });
        if (!HotMealsSettings.multipleHeat)
        {
            yield return Toils_Reserve.Reserve(TargetIndex.C);
            yield return Toils_Goto.GotoThing(TargetIndex.C, PathEndMode.InteractionCell)
                .FailOnDestroyedOrNull(TargetIndex.C);
            yield return Toils_HeatMeal.HeatMeal().FailOnDespawnedNullOrForbiddenPlacedThings()
                .FailOnCannotTouch(TargetIndex.C, PathEndMode.InteractionCell);
            yield return Toils_Reserve.Release(TargetIndex.C);
        }
        else
        {
            yield return Toils_Goto.GotoThing(TargetIndex.C, PathEndMode.Touch).FailOnDestroyedOrNull(TargetIndex.C);
            yield return Toils_HeatMeal.HeatMeal().FailOnDespawnedNullOrForbiddenPlacedThings()
                .FailOnCannotTouch(TargetIndex.C, PathEndMode.Touch);
        }

        yield return empty;
    }


    private Toil ReserveFood()
    {
        return new Toil
        {
            initAction = delegate
            {
                if (pawn.Faction == null)
                {
                    return;
                }

                var thing = job.GetTarget(TargetIndex.A).Thing;
                if (pawn.carryTracker.CarriedThing == thing)
                {
                    return;
                }

                var maxAmountToPickup = FoodUtility.GetMaxAmountToPickup(thing, pawn, job.count);
                if (maxAmountToPickup == 0)
                {
                    return;
                }

                if (!pawn.Reserve(thing, job, 10, maxAmountToPickup))
                {
                    Log.Error(string.Concat("Pawn food reservation for ", pawn, " on job ", this,
                        " failed, because it could not register food from ", thing, " - amount: ", maxAmountToPickup));
                    pawn.jobs.EndCurrentJob(JobCondition.Errored);
                }

                job.count = maxAmountToPickup;
            },
            defaultCompleteMode = ToilCompleteMode.Instant,
            atomicWithPrevious = true
        };
    }
}