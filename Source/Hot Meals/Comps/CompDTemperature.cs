using System;
using DHotMeals;
using Verse;

namespace DThermodynamicsCore.Comps;

public class CompDTemperature : ThingComp
{
    public const double minStep = 10.0 / 2500.0; // 3 degrees celsius = 5.4 degrees fahrenheit per hour at minimum

    public double curTemp;

    private int tickCount;

    public virtual CompProperties_DTemperature PropsTemp => (CompProperties_DTemperature)props;

    protected virtual DTemperatureLevels Levels => PropsTemp.tempLevels;

    public virtual double AmbientTemperature
    {
        get
        {
            if (parent.Spawned)
            {
                if (!Base.RimFridgeRunning && !Base.AdaptiveStorageRunning)
                {
                    return GenTemperature.GetTemperatureForCell(parent.Position, parent.Map);
                }

                var thingList = parent.PositionHeld.GetThingList(parent.MapHeld);
                foreach (var thing in thingList)
                {
                    if (Base.AdaptiveStorageRunning)
                    {
                        if (thing.def.modExtensions?.Any(extension =>
                                extension.GetType().Namespace == "AdaptiveStorage") == true)
                        {
                            return parent.AmbientTemperature;
                        }
                    }

                    if (!Base.RimFridgeRunning)
                    {
                        continue;
                    }

                    if (thing is not ThingWithComps thingWithComps)
                    {
                        continue;
                    }

                    var compRefrigerator =
                        thingWithComps.AllComps.FirstOrDefault(comp => comp.GetType() == Base.CompRefrigeratorType);
                    if (compRefrigerator == null)
                    {
                        continue;
                    }

                    var fridgeTemp = (float)Base.RimFridgeTempField.GetValue(compRefrigerator);
                    return fridgeTemp;
                }

                return GenTemperature.GetTemperatureForCell(parent.Position, parent.Map);
            }


            if (parent.ParentHolder != null)
            {
                for (var parentHolder = parent.ParentHolder;
                     parentHolder != null;
                     parentHolder = parentHolder.ParentHolder)
                {
                    if (ThingOwnerUtility.TryGetFixedTemperature(parentHolder, parent, out var result))
                    {
                        return result;
                    }
                }
            }

            if (parent.SpawnedOrAnyParentSpawned)
            {
                return GenTemperature.GetTemperatureForCell(parent.PositionHeld, parent.MapHeld);
            }

            return parent.Tile >= 0 ? GenTemperature.GetTemperatureAtTile(parent.Tile) : 21f;
        }
    }

    public virtual float GetCurTemp()
    {
        return (float)curTemp;
    }

    public virtual string GetState(double temp)
    {
        return "";
    }

    protected virtual bool PawnIsCarrying()
    {
        if (parent.ParentHolder == null)
        {
            return false;
        }

        for (var parentHolder = parent.ParentHolder; parentHolder != null; parentHolder = parentHolder.ParentHolder)
        {
            if (parentHolder is Pawn_InventoryTracker)
            {
                return true;
            }
        }

        return false;
    }


    public virtual void Diffuse(double diffuseTo, int interval)
    {
        var diffuseTime = PropsTemp.diffusionTime;
        if (HotMealsSettings.slowDiffuseWhileCarried && PawnIsCarrying())
        {
            diffuseTime *= 10;
        }

        var minStepScaled = minStep * interval;
        var shift = diffuseTo - curTemp;
        var changeMag = Math.Abs(interval * shift / diffuseTime);
        var step = Math.Abs(shift) < minStepScaled || changeMag > minStep ? changeMag : minStepScaled;
        curTemp += Math.Sign(shift) * step * HotMealsSettings.diffusionModifier;
    }


    protected virtual void tempTick(int numTicks)
    {
        Diffuse(AmbientTemperature, numTicks);
    }

    public override void CompTick()
    {
        tickCount++;
        if (tickCount < PropsTemp.tickRate)
        {
            return;
        }

        tickCount = 0;
        tempTick(PropsTemp.tickRate);
    }

    public override void CompTickRare()
    {
        tempTick(250);
    }


    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
    }

    public override string CompInspectStringExtra()
    {
        return ((float)curTemp).ToStringTemperature();
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref curTemp, "curTemp", PropsTemp.initialTemp);
    }

    public override void PostPostMake()
    {
        base.PostPostMake();
        curTemp = PropsTemp.initialTemp;
    }

    public override void PostSplitOff(Thing piece)
    {
        var newComp = piece.TryGetComp<CompDTemperature>();
        if (newComp != null)
        {
            newComp.curTemp = curTemp;
        }

        base.PostSplitOff(piece);
    }

    public override void PreAbsorbStack(Thing otherStack, int count)
    {
        var newComp = otherStack.TryGetComp<CompDTemperature>();
        if (newComp != null)
        {
            curTemp = ((curTemp * parent.stackCount) + (newComp.curTemp * count)) / (float)(parent.stackCount + count);
        }

        base.PreAbsorbStack(otherStack, count);
    }
}