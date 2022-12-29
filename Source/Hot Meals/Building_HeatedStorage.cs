using DHotMeals;
using DThermodynamicsCore.Comps;
using RimWorld;
using Verse;

namespace DThermodynamicsCore;

internal class Building_HeatedStorage : Building_Storage
{
    public static float slowRotRateAmount = -99999f;
    public static float unrefrigeratedRotRate = -99999f;
    public CompDTempControl compDTempControl;
    public CompPowerTrader compPowerTrader;
    public CompRefuelable compRefuelable;

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        compPowerTrader = GetComp<CompPowerTrader>();
        compDTempControl = GetComp<CompDTempControl>();
        compRefuelable = GetComp<CompRefuelable>();
        if (unrefrigeratedRotRate < 0)
        {
            unrefrigeratedRotRate = GenTemperature.RotRateAtTemperature(999f);
        }

        if (slowRotRateAmount < 0)
        {
            slowRotRateAmount = unrefrigeratedRotRate - GenTemperature.RotRateAtTemperature(1f);
        }
    }

    public virtual float GetTargetTemp()
    {
        if (compDTempControl != null)
        {
            return compDTempControl.targetTemperature;
        }

        Log.Error(
            $"Thermodynamics: No CompTempControl in Building_HeatedStorage, defName is {def.defName}");
        return 21f;
    }

    public virtual bool IsActive()
    {
        return compPowerTrader is { PowerOn: true } || compRefuelable is { HasFuel: true } ||
               compPowerTrader == null && compRefuelable == null;
    }

    public override void Notify_ReceivedThing(Thing newItem)
    {
        base.Notify_ReceivedThing(newItem);
        newItem.Rotation = Rotation;
    }

    public virtual void ChangeHeat(int ticks)
    {
        if (!IsActive())
        {
            return;
        }

        var targetTemp = GetTargetTemp();

        foreach (var thing in slotGroup.HeldThings)
        {
            var comp = thing.TryGetComp<CompDTemperature>();
            if (comp != null && comp.curTemp < targetTemp)
            {
                comp.Diffuse(targetTemp * 2f, ticks);
            }

            if (!HotMealsSettings.warmersSlowRot)
            {
                continue;
            }

            var rot = thing.TryGetComp<CompRottable>();
            if (rot != null && GenTemperature.RotRateAtTemperature(thing.AmbientTemperature) ==
                unrefrigeratedRotRate)
            {
                rot.RotProgress -= ticks * slowRotRateAmount;
            }
        }
    }

    public override void Tick()
    {
        base.Tick();
        ChangeHeat(1);
    }

    public override void TickRare()
    {
        base.TickRare();
        ChangeHeat(250);
    }
}