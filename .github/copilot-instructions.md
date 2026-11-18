# [D] Thermodynamics - Hot Meals (Continued) GitHub Copilot Instructions

Welcome to the [D] Thermodynamics - Hot Meals (Continued) mod project! This mod enhances the RimWorld experience by making the temperature of meals matter, ensuring colonists enjoy their food at the right temperature. Below are guidelines to help you effectively use GitHub Copilot for this mod project.

## Mod Overview and Purpose

**Mod Name:** [D] Thermodynamics - Hot Meals (Continued)

**Description:**  
Update of Dametris mod based on the 1.3 update by Mercenary. This mod introduces temperature-sensitive food items, categorizes them into various types, and affects colonists' mood based on the temperature of the food they consume. It integrates with the Thermodynamics - Core mod and requires Harmony.

**Compatibility:**  
- RimWorld 1.1 and 1.2
- Compatible with existing saves
- Requires Thermodynamics - Core
- Requires Harmony

## Key Features and Systems

### Food Categories
- **Hot Meals:** Simple Meals, etc.
- **Cold Meals:** Ice Cream from VGP Gourmet, etc.
- **Hot Drinks:** Coffee from multiple mods, etc.
- **Cold Drinks:** Beer, etc.
- **Lukewarm Meals:** Nigiri from VGP Gourmet, etc.
- **Raw Resources:** Potatoes, etc.
- **Nonperishables:** Packaged Survival Meals, etc.
- **Edible Raw:** Agave fruit, etc.

### Temperature Tracking
- Food items keep track of their temperature.
- Temperature gradually diffuses with the surrounding environment.
- Each category has an associated ideal temperature range.

### Mood Effects
- Colonists are happy with food in the right temperature range.
- Mood decreases progressively for food outside the ideal range.

### Heating Appliances
- **Stoves:** Colonists can heat up food at stoves.
- **Microwaves:** Dedicated appliance for reheating meals at super speed.
- **Heat Lamps:** Keep food stored at a set temperature and optionally slow food rot.

## Coding Patterns and Conventions

### C# Files
- **HotMealsMod.cs:** Contains the main mod class with settings handling.
- **DHotMealsHarmonyPatches.cs:** Contains Harmony patches for the mod.
- **Building_HeatedStorage.cs:** Defines the heated storage building with temperature control features.
- **AssignMealTemp.cs:** Handles the assignment of meal temperatures.
- **Base.cs:** Contains base classes and definitions.

### XML Files
- **JobDefs/**: Contains job definitions.
- **Stats/**: Contains stat category and definition files.
- **Patches/**: Contains XML patches for meal temperatures.

### General Coding Guidelines
- Follow the existing coding style and conventions used in the mod.
- Use meaningful variable and method names.
- Write clear and concise comments to explain complex logic.

## XML Integration

### Def Files
- **JobDefs/JobDef.xml:** Defines custom job definitions for meal-related tasks.
- **Stats/StatCategoryDef.xml, StatDef.xml:** Defines new stats and categories for temperature-sensitive food.
- **Patches/**: Contains XML patches to adjust meal temperatures and integrate with other mods.

### Integration Steps
1. **Define New Defs:** Create new XML files for custom job definitions, stats, and patches.
2. **Modify Existing Defs:** Update existing def files to include temperature-sensitive properties.
3. **Integrate Patches:** Apply XML patches to adjust meal temperatures and ensure compatibility with other mods.

## Harmony Patching

### Patch Files
- **DHotMealsHarmonyPatches.cs:** Contains Harmony patches for the mod.

### Patching Steps
1. **Identify Target Methods:** Find the methods in the RimWorld code that need modification.
2. **Create Prefix/Suffix Patches:** Use Harmony to create prefix or suffix patches to modify the behavior of the target methods.
3. **Test Patches:** Ensure that the patches work as expected and do not break the game.

### Example Patch
csharp
[HarmonyPatch(typeof(ThingDef), nameof(ThingDef.CanEat))]
public static class DHotMealsHarmonyPatches
{
    public static void Postfix(ThingDef __instance, ref bool __result)
    {
        // Custom logic to check temperature-sensitive food
        if (__instance.thingClass == typeof(Food))
        {
            __result &= IsTemperatureCorrect(__instance);
        }
    }

    private static bool IsTemperatureCorrect(ThingDef foodDef)
    {
        // Implement temperature check logic
        return true;
    }
}


## Suggestions for Copilot

### C# Code Suggestions
- Use Copilot to generate Harmony patches for modifying RimWorld methods.
- Use Copilot to create new classes and methods for handling temperature-sensitive food.
- Use Copilot to generate XML def files for new job definitions, stats, and patches.

### XML Integration Suggestions
- Use Copilot to generate XML patch files to adjust meal temperatures.
- Use Copilot to create new def files for custom job definitions and stats.
- Use Copilot to modify existing def files to include temperature-sensitive properties.

### General Suggestions
- Use Copilot to generate code snippets for handling temperature-sensitive food.
- Use Copilot to create new methods for temperature tracking and mood effects.
- Use Copilot to generate comments for complex logic in the code.

By following these guidelines and utilizing GitHub Copilot effectively, you can enhance the [D] Thermodynamics - Hot Meals (Continued) mod and provide a better experience for RimWorld players. Happy modding!

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

