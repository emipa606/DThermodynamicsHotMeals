using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

// basically the same as CompTempControl, but they made it annoying to inherit and modify

namespace DThermodynamicsCore.Comps;

public class CompDTempControl : ThingComp
{
    private const float DefaultTargetTemperature = 21f;

    [Unsaved] public bool operatingAtHighPower;

    public float targetTemperature = -99999f;

    public CompProperties_DTempControl Props => (CompProperties_DTempControl)props;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        if (targetTemperature < -2000f)
        {
            targetTemperature = Props.defaultTargetTemperature;
        }
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref targetTemperature, "targetTemperature");
    }

    private float RoundedToCurrentTempModeOffset(float celsiusTemp)
    {
        return GenTemperature.ConvertTemperatureOffset(
            Mathf.RoundToInt(GenTemperature.CelsiusToOffset(celsiusTemp, Prefs.TemperatureMode)), Prefs.TemperatureMode,
            TemperatureDisplayMode.Celsius);
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (var gizmo in base.CompGetGizmosExtra())
        {
            yield return gizmo;
        }

        var offset2 = RoundedToCurrentTempModeOffset(-10f);
        yield return new Command_Action
        {
            action = delegate { InterfaceChangeTargetTemperature(offset2); },
            defaultLabel = offset2.ToStringTemperatureOffset("F0"),
            defaultDesc = "CommandLowerTempDesc".Translate(),
            hotKey = KeyBindingDefOf.Misc5,
            icon = ContentFinder<Texture2D>.Get("UI/Commands/TempLower")
        };
        var offset3 = RoundedToCurrentTempModeOffset(-1f);
        yield return new Command_Action
        {
            action = delegate { InterfaceChangeTargetTemperature(offset3); },
            defaultLabel = offset3.ToStringTemperatureOffset("F0"),
            defaultDesc = "CommandLowerTempDesc".Translate(),
            hotKey = KeyBindingDefOf.Misc4,
            icon = ContentFinder<Texture2D>.Get("UI/Commands/TempLower")
        };
        yield return new Command_Action
        {
            action = delegate
            {
                targetTemperature = 21f;
                SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                ThrowCurrentTemperatureText();
            },
            defaultLabel = "CommandResetTemp".Translate(),
            defaultDesc = "CommandResetTempDesc".Translate(),
            hotKey = KeyBindingDefOf.Misc1,
            icon = ContentFinder<Texture2D>.Get("UI/Commands/TempReset")
        };
        var offset4 = RoundedToCurrentTempModeOffset(1f);
        yield return new Command_Action
        {
            action = delegate { InterfaceChangeTargetTemperature(offset4); },
            defaultLabel = $"+{offset4.ToStringTemperatureOffset("F0")}",
            defaultDesc = "CommandRaiseTempDesc".Translate(),
            hotKey = KeyBindingDefOf.Misc2,
            icon = ContentFinder<Texture2D>.Get("UI/Commands/TempRaise")
        };
        var offset = RoundedToCurrentTempModeOffset(10f);
        yield return new Command_Action
        {
            action = delegate { InterfaceChangeTargetTemperature(offset); },
            defaultLabel = $"+{offset.ToStringTemperatureOffset("F0")}",
            defaultDesc = "CommandRaiseTempDesc".Translate(),
            hotKey = KeyBindingDefOf.Misc3,
            icon = ContentFinder<Texture2D>.Get("UI/Commands/TempRaise")
        };
    }

    private void InterfaceChangeTargetTemperature(float offset)
    {
        SoundDefOf.DragSlider.PlayOneShotOnCamera();
        targetTemperature += offset;
        targetTemperature = Mathf.Clamp(targetTemperature, Props.minTargetTemperature, Props.maxTargetTemperature);
        ThrowCurrentTemperatureText();
    }

    private void ThrowCurrentTemperatureText()
    {
        MoteMaker.ThrowText(parent.TrueCenter() + new Vector3(0.5f, 0f, 0.5f), parent.Map,
            targetTemperature.ToStringTemperature("F0"), Color.white);
    }

    public override string CompInspectStringExtra()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("TargetTemperature".Translate() + ": ");
        stringBuilder.Append(targetTemperature.ToStringTemperature("F0"));
        return stringBuilder.ToString();
    }
}