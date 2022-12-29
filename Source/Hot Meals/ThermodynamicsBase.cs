using System;
using DHotMeals;
using DThermodynamicsCore.Comps;
using Verse;

namespace DThermodynamicsCore;

[StaticConstructorOnStartup]
public static class ThermodynamicsBase
{
    public static float HoursToTargetTemp(CompDTemperature comp, double target, double tolerance = 0.5)
    {
        const double interval = 250;
        var ambient = comp.AmbientTemperature;
        var curtemp = comp.curTemp;
        var goUp = curtemp < target;
        var minStepScaled = CompDTemperature.minStep * interval;
        var diffusionTime = comp.PropsTemp.diffusionTime;
        var count = 0;
        while (goUp ? curtemp + tolerance < target : curtemp > target + tolerance && count < 1000)
        {
            var shift = ambient - curtemp;
            var changeMag = Math.Abs(interval * shift / diffusionTime);
            var step = Math.Abs(shift) < minStepScaled || changeMag > CompDTemperature.minStep
                ? changeMag
                : minStepScaled;
            curtemp += Math.Sign(shift) * step * HotMealsSettings.diffusionModifier;
            //curtemp += CompDTemperature.CalcTempChange(ambient, curtemp, interval, diffusionTime, minStepScaled);
            count++;
        }

        return count / 10f;
    }
}