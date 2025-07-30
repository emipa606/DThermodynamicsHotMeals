# .github/copilot-instructions.md

## Mod Overview and Purpose
The "Hot Meals" mod enhances the gameplay experience in RimWorld by introducing a dynamic temperature system that influences the mood and consumption of meals. This mod aims to add depth to food-related mechanics, affecting how players manage their colony's food storage and preparation processes.

## Key Features and Systems

1. **Dynamic Temperature System**: Implements a complex system that allows food to have varying temperatures, affecting colonists' mood depending on whether the food is too hot, too cold, or at an ideal temperature.

2. **Food Temperature Components**:
   - `CompDFoodTemperature`: Determines the type of food and its temperature effects on colonist morale.
   - `CompDTemperatureIngestible`: Extends temperature properties to ingestible items.

3. **Temperature Control Mechanisms**:
   - `CompDTempControl`: Allows manipulation and visualization of temperature settings for food storage.
   - `Building_HeatedStorage`: Provides infrastructure for efficient temperature management of stored food items.

4. **Custom Job Drivers**:
   - `JobDriver_HeatMeal`: Enables colonists to reheat meals to a desirable temperature before consumption.
   - `Toils_HeatMeal`: Supports the implementation of heating tasks within job drivers.

5. **Harmony Patching**: 
   - Integrates seamlessly with existing game systems through careful Harmony patches, ensuring compatibility and minimizing conflicts with other mods.

## Coding Patterns and Conventions

### General C# Practices:
- Follow standard naming conventions for classes (`PascalCase`) and methods (`camelCase`).
- Use access modifiers effectively to encapsulate class internals (`public`, `internal`, or `private` as necessary).
- Implement clean and consistent code structure for maintainability and readability.

### Class Structure and Inheritance:
- Utilize inheritance to extend temperature properties across related component classes, such as `CompDTemperatureIngestible` inheriting from `CompDTemperature`.

### Internal Logic:
- Utilize logical utility methods, e.g., methods like `GetPositiveMoodEffect()` and `GetNegativeMoodEffect()` in `CompDFoodTemperature` for determining mood adjustments based on food temperature.

## XML Integration

### XML Modding Techniques:
- Define components and properties using XML definitions when possible. Ensure that integration between C# classes and XML is clear and well-documented.
- Use `PatchOperation` classes (e.g., `AssignMealTemp`) for XML-based patching to adjust defined properties dynamically without hardcoded values.

## Harmony Patching

### Implementation:
- Use Harmony to patch existing game methods cautiously. Ensure patches are:
  - Non-destructive: Changes should not break base game functionality.
  - Transparent: Thoroughly log changes or potential issues.
  - Isolated: Target narrowly-defined behavior to minimize interference with other mods.

- Implement patches within files like `DHotMealsHarmonyPatches.cs` and `DThermodynamicsCoreHarmonyPatches.cs` to regulate temperature effects and integrate with the game's lifecycle.

## Suggestions for Copilot

1. **Enhance Boilerplate Generation**:
   - Use Copilot to generate method headers and consistent documentation comments for classes and methods.

2. **Refactor Common Patterns**:
   - Identify common code patterns, such as temperature conversion or mood calculation logic, and refactor these into helper methods that can be reused across multiple classes.

3. **Debug and Logging**:
   - Implement comprehensive logging for temperature changes and food interactions to aid in debugging and provide clear feedback to players.

4. **Documentation Assistance**:
   - Assist in generating user-friendly documentation for mod users to understand the impact and interactions of the mod on their gameplay.

5. **Compatibility Checks**:
   - Suggest strategies for ensuring compatibility with the latest versions of RimWorld and other prevalent mods, utilizing Harmony patches and XML configurations.

This file should serve as a guideline for leveraging GitHub Copilot effectively in developing the "Hot Meals" mod for RimWorld. Consistency in coding practices and thorough integration testing will help maintain high-quality modding standards.
