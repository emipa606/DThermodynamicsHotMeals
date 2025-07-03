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

    private static string inIdealRange()
    {
        return "HoMe.Inidealrange".Translate();
    }

    private static string willNeverReach()
    {
        return "HoMe.Neverreach".Translate();
    }

    private static string hoursToIdeal(CompDTemperatureIngestible comp, double idealTemp)
    {
        return
            "HoMe.Willreach".Translate($"{ThermodynamicsBase.HoursToTargetTemp(comp, idealTemp):f1}");
    }

    private static string hoursInIdeal(CompDTemperatureIngestible comp, double changeThreshold)
    {
        return
            "HoMe.Willremain".Translate($"{ThermodynamicsBase.HoursToTargetTemp(comp, changeThreshold):f1}");
    }

    private static string hoursToAmbient(CompDTemperature comp)
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
            s += hoursToAmbient(comp);
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
                        s += hoursInIdeal(comp2, comp2.PropsTemp.tempLevels.goodTemp);
                    }
                    else if (ambient > comp2.PropsTemp.tempLevels.okTemp)
                    {
                        s += hoursInIdeal(comp2, comp2.PropsTemp.tempLevels.okTemp);
                    }
                    else
                    {
                        s += inIdealRange();
                    }
                }
                else if (comp2.curTemp < comp2.PropsTemp.tempLevels.goodTemp &&
                         ambient > comp2.PropsTemp.tempLevels.goodTemp)
                {
                    s += hoursToIdeal(comp2, comp2.PropsTemp.tempLevels.goodTemp);
                }
                else if (comp2.curTemp > comp2.PropsTemp.tempLevels.okTemp &&
                         ambient < comp2.PropsTemp.tempLevels.okTemp)
                {
                    s += hoursToIdeal(comp2, comp2.PropsTemp.tempLevels.okTemp);
                }
                else
                {
                    s += willNeverReach();
                }
            }
            else if (comp2.PropsTemp.likesHeat)
            {
                if (comp2.curTemp > comp2.PropsTemp.tempLevels.goodTemp)
                {
                    if (ambient < comp2.PropsTemp.tempLevels.goodTemp)
                    {
                        s += hoursInIdeal(comp2, comp2.PropsTemp.tempLevels.goodTemp);
                    }
                    else
                    {
                        s += inIdealRange();
                    }
                }
                else if (ambient > comp2.PropsTemp.tempLevels.goodTemp)
                {
                    s += hoursToIdeal(comp2, comp2.PropsTemp.tempLevels.goodTemp);
                }
                else
                {
                    s += willNeverReach();
                }
            }
            else
            {
                if (comp2.curTemp < 0f && !comp2.PropsTemp.okFrozen)
                {
                    switch (comp2.AmbientTemperature)
                    {
                        case > 0f:
                            s += hoursToIdeal(comp2, 0);
                            break;
                        case <= 0f:
                            s += willNeverReach();
                            break;
                    }
                }
                else if (comp2.curTemp < comp2.PropsTemp.tempLevels.goodTemp)
                {
                    if (ambient > comp2.PropsTemp.tempLevels.goodTemp)
                    {
                        s += hoursInIdeal(comp2, comp2.PropsTemp.tempLevels.goodTemp);
                    }
                    else
                    {
                        s += inIdealRange();
                    }
                }
                else if (ambient < comp2.PropsTemp.tempLevels.goodTemp)
                {
                    s += hoursToIdeal(comp2, comp2.PropsTemp.tempLevels.goodTemp);
                }
                else
                {
                    s += willNeverReach();
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