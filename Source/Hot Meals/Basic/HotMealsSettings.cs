using UnityEngine;
using Verse;

namespace DHotMeals;

public class HotMealsSettings : ModSettings
{
    private static bool positiveMoodEnabled = true;
    public static int positiveMoodBuff = 3;
    private static bool negativeMoodEnabled = true;
    public static int negativeMoodDebuff = -3;
    public static float heatSpeedMult = 1;
    public static bool thawIt;
    public static bool useCookingAppliances = true;
    public static bool multipleHeat;
    public static float searchRadius = 48f;
    public static float diffusionModifier = 1f;
    public static bool slowDiffuseWhileCarried = true;
    public static bool warmersSlowRot;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref positiveMoodEnabled, "positiveMoodEnabled", true);
        Scribe_Values.Look(ref positiveMoodBuff, "positiveMoodBuff", 3);
        Scribe_Values.Look(ref negativeMoodEnabled, "negativeMoodEnabled", true);
        Scribe_Values.Look(ref negativeMoodDebuff, "negativeMoodDebuff", -3);
        Scribe_Values.Look(ref heatSpeedMult, "heatSpeedMult", 1);
        Scribe_Values.Look(ref thawIt, "thawIt");
        Scribe_Values.Look(ref useCookingAppliances, "useCookingAppliances", true);
        Scribe_Values.Look(ref multipleHeat, "multipleHeat");
        Scribe_Values.Look(ref searchRadius, "searchRadius", 48f);
        Scribe_Values.Look(ref diffusionModifier, "diffusionModifier", 1f);
        Scribe_Values.Look(ref slowDiffuseWhileCarried, "slowDiffuseWhileCarried", true);
        Scribe_Values.Look(ref warmersSlowRot, "warmersSlowRot");
        base.ExposeData();
    }

    public static void WriteAll() // called when settings window closes
    {
    }

    public static void DrawSettings(Rect rect)
    {
        var ls = new Listing_Standard(GameFont.Small)
        {
            ColumnWidth = rect.width * 2.0f / 3.0f
        };

        ls.Begin(rect);
        ls.Gap();

        ls.CheckboxLabeled("HoMe.multipleHeat".Translate(), ref multipleHeat, "HoMe.multipleHeatTT".Translate());
        ls.Gap();

        var searchRect = ls.GetRect(Text.LineHeight);
        var searchLabelRect = searchRect.LeftPartPixels(300);
        var searchSliderRect = searchRect.RightPartPixels(searchRect.width - 300);
        Widgets.Label(searchLabelRect, "HoMe.searchRadius".Translate());
        searchRadius = Widgets.HorizontalSlider(searchSliderRect, searchRadius, 1f, 100f, false,
            searchRadius.ToString("f0"), null, null, 1f);
        ls.Gap();

        var heatRect = ls.GetRect(Text.LineHeight);
        var heatLabelRect = heatRect.LeftPartPixels(300);
        var heatSliderRect = heatRect.RightPartPixels(heatRect.width - 300);
        Widgets.Label(heatLabelRect, "HoMe.heatSpeedMult".Translate());
        heatSpeedMult = Widgets.HorizontalSlider(heatSliderRect, heatSpeedMult, 0.1f, 5.0f, false,
            heatSpeedMult.ToString("f1"), null, null, 0.1f);
        ls.Gap();
        ls.Gap();
        ls.CheckboxLabeled("HoMe.positiveMoodEnabled".Translate(), ref positiveMoodEnabled);
        ls.Gap();
        if (positiveMoodEnabled)
        {
            var posRect = ls.GetRect(Text.LineHeight);
            var posLabelRect = posRect.LeftPartPixels(300);
            var posSliderRect = posRect.RightPartPixels(posRect.width - 300);
            Widgets.Label(posLabelRect, "HoMe.positiveMoodBuff".Translate());
            positiveMoodBuff = Mathf.RoundToInt(Widgets.HorizontalSlider(posSliderRect, positiveMoodBuff, 1, 20,
                false,
                positiveMoodBuff.ToString(), null, null, 1));
        }

        ls.Gap();
        ls.CheckboxLabeled("HoMe.negativeMoodEnabled".Translate(), ref negativeMoodEnabled);
        if (negativeMoodEnabled)
        {
            var negRect = ls.GetRect(Text.LineHeight);
            var negLabelRect = negRect.LeftPartPixels(300);
            var negSliderRect = negRect.RightPartPixels(negRect.width - 300);
            Widgets.Label(negLabelRect, "HoMe.negativeMoodDebuff".Translate());
            negativeMoodDebuff = Mathf.RoundToInt(Widgets.HorizontalSlider(negSliderRect, negativeMoodDebuff,
                -15, 0,
                false, negativeMoodDebuff.ToString(), null, null, 1));
        }

        ls.Gap();
        ls.Gap();
        ls.CheckboxLabeled("HoMe.thawIt".Translate(), ref thawIt);

        ls.Gap();
        ls.CheckboxLabeled("HoMe.useCookingAppliances".Translate(), ref useCookingAppliances);

        var diffusionRect = ls.GetRect(Text.LineHeight);
        var diffusionLabelRect = diffusionRect.LeftPartPixels(300);
        var diffusionSliderRect = diffusionRect.RightPartPixels(diffusionRect.width - 300);
        Widgets.Label(diffusionLabelRect, "HoMe.diffusionModifier".Translate());
        diffusionModifier = Widgets.HorizontalSlider(diffusionSliderRect, diffusionModifier, 0, 5.0f, false,
            diffusionModifier.ToString("f2"), null, null, 0.05f);
        ls.Gap();

        ls.Gap();
        ls.CheckboxLabeled("HoMe.slowDiffuseWhileCarried".Translate(), ref slowDiffuseWhileCarried);
        ls.Gap();
        ls.CheckboxLabeled("HoMe.warmersSlowRot".Translate(), ref warmersSlowRot,
            "HoMe.warmersSlowRotTT".Translate());

        if (HotMealsMod.currentVersion != null)
        {
            ls.Gap();
            GUI.contentColor = Color.gray;
            ls.Label("HoMe.CurrentModVersion".Translate(HotMealsMod.currentVersion));
            GUI.contentColor = Color.white;
        }

        ls.End();
    }
}