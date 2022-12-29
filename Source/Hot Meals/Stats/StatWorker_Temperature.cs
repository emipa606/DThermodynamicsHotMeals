using System;
using DHotMeals;
using DThermodynamicsCore.Comps;
using RimWorld;
using Verse;

namespace DThermodynamicsCore.Stats;

internal class StatWorker_Temperature : StatWorker
{
    public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
    {
        if (!req.HasThing)
        {
            return -9999;
        }

        var comp = req.Thing.TryGetComp<CompDTemperature>();
        if (comp != null)
        {
            return (float)comp.curTemp;
        }

        return -9999;
    }

    public override string GetExplanationFinalizePart(StatRequest req, ToStringNumberSense numberSense, float finalVal)
    {
        return "";
    }

    public static string InIdealRange()
    {
        return "HoMe.Inidealrange".Translate();
    }

    public static string WillNeverReach()
    {
        return "HoMe.Neverreach".Translate();
    }

    public static string HoursToIdeal(CompDTemperatureIngestible comp, double idealTemp)
    {
        return
            "HoMe.Willreach".Translate($"{ThermodynamicsBase.HoursToTargetTemp(comp, idealTemp):f1}");
    }

    public static string HoursInIdeal(CompDTemperatureIngestible comp, double changeThreshold)
    {
        return
            "HoMe.Willremain".Translate($"{ThermodynamicsBase.HoursToTargetTemp(comp, changeThreshold):f1}");
    }

    public static string HoursToAmbient(CompDTemperature comp)
    {
        return
            "HoMe.Willreachambient".Translate(((float)comp.AmbientTemperature).ToStringTemperature(),
                $"{ThermodynamicsBase.HoursToTargetTemp(comp, comp.AmbientTemperature, 2):f1}");
    }

    public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
    {
        var s = "";
        if (!req.HasThing)
        {
            return s;
        }

        var comp = req.Thing.TryGetComp<CompDTemperature>();
        if (comp == null)
        {
            return s;
        }

        double interval = 2500;
        var ambient = comp.AmbientTemperature;
        var shift = ambient - comp.curTemp;
        var changeMag = Math.Abs(interval * shift / comp.PropsTemp.diffusionTime);
        var minStepScaled = CompDTemperature.minStep * interval;
        var step = Math.Abs(shift) < minStepScaled || changeMag > CompDTemperature.minStep
            ? changeMag
            : minStepScaled;
        if (step == minStepScaled)
        {
            s += "HoMe.equilibrium".Translate(((float)ambient).ToStringTemperatureOffset());
        }
        else
        {
            var result = Math.Sign(shift) * step * HotMealsSettings.diffusionModifier;
            s +=
                "HoMe.diffusing".Translate(((float)result).ToStringTemperatureOffset());
            s += HoursToAmbient(comp);
            if (comp is not CompDTemperatureIngestible comp2)
            {
                return s;
            }

            s += "\n";

            if (comp2.PropsTemp.roomTemperature)
            {
                if (comp2.curTemp > comp2.PropsTemp.tempLevels.goodTemp &&
                    comp2.curTemp < comp2.PropsTemp.tempLevels.okTemp)
                {
                    if (ambient < comp2.PropsTemp.tempLevels.goodTemp)
                    {
                        s += HoursInIdeal(comp2, comp2.PropsTemp.tempLevels.goodTemp);
                    }
                    else if (ambient > comp2.PropsTemp.tempLevels.okTemp)
                    {
                        s += HoursInIdeal(comp2, comp2.PropsTemp.tempLevels.okTemp);
                    }
                    else
                    {
                        s += InIdealRange();
                    }
                }
                else if (comp2.curTemp < comp2.PropsTemp.tempLevels.goodTemp &&
                         ambient > comp2.PropsTemp.tempLevels.goodTemp)
                {
                    s += HoursToIdeal(comp2, comp2.PropsTemp.tempLevels.goodTemp);
                }
                else if (comp2.curTemp > comp2.PropsTemp.tempLevels.okTemp &&
                         ambient < comp2.PropsTemp.tempLevels.okTemp)
                {
                    s += HoursToIdeal(comp2, comp2.PropsTemp.tempLevels.okTemp);
                }
                else
                {
                    s += WillNeverReach();
                }
            }
            else if (comp2.PropsTemp.likesHeat)
            {
                if (comp2.curTemp > comp2.PropsTemp.tempLevels.goodTemp)
                {
                    if (ambient < comp2.PropsTemp.tempLevels.goodTemp)
                    {
                        s += HoursInIdeal(comp2, comp2.PropsTemp.tempLevels.goodTemp);
                    }
                    else
                    {
                        s += InIdealRange();
                    }
                }
                else if (ambient > comp2.PropsTemp.tempLevels.goodTemp)
                {
                    s += HoursToIdeal(comp2, comp2.PropsTemp.tempLevels.goodTemp);
                }
                else
                {
                    s += WillNeverReach();
                }
            }
            else
            {
                if (comp2.curTemp < 0f && !comp2.PropsTemp.okFrozen)
                {
                    switch (comp2.AmbientTemperature)
                    {
                        case > 0f:
                            s += HoursToIdeal(comp2, 0);
                            break;
                        case <= 0f:
                            s += WillNeverReach();
                            break;
                    }
                }
                else if (comp2.curTemp < comp2.PropsTemp.tempLevels.goodTemp)
                {
                    if (ambient > comp2.PropsTemp.tempLevels.goodTemp)
                    {
                        s += HoursInIdeal(comp2, comp2.PropsTemp.tempLevels.goodTemp);
                    }
                    else
                    {
                        s += InIdealRange();
                    }
                }
                else if (ambient < comp2.PropsTemp.tempLevels.goodTemp)
                {
                    s += HoursToIdeal(comp2, comp2.PropsTemp.tempLevels.goodTemp);
                }
                else
                {
                    s += WillNeverReach();
                }
            }
        }

        return s;
    }

    public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense,
        StatRequest optionalReq, bool finalized = true)
    {
        var s = value.ToStringTemperature();
        if (!optionalReq.HasThing)
        {
            return s;
        }

        var comp = optionalReq.Thing.TryGetComp<CompDTemperature>();
        if (comp != null)
        {
            s += $", {comp.GetState(comp.curTemp)}";
        }

        return s;
    }
}