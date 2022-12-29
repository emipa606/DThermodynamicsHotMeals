using Mlie;
using UnityEngine;
using Verse;

namespace DHotMeals;

public class HotMealsMod : Mod
{
    public static string currentVersion;
    public static HotMealsSettings settings;

    public HotMealsMod(ModContentPack content) : base(content)
    {
        settings = GetSettings<HotMealsSettings>();
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        HotMealsSettings.DrawSettings(inRect);
        base.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "Thermodynamics - Hot Meals";
    }


    public override void WriteSettings() // called when settings window closes
    {
        HotMealsSettings.WriteAll();
        base.WriteSettings();
    }
}