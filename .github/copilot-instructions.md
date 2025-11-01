# GitHub Copilot Instructions for RimWorld Mod: [D] Thermodynamics - Hot Meals (Continued)

## Mod Overview and Purpose

[D] Thermodynamics - Hot Meals (Continued) is a RimWorld mod designed to enhance the in-game meal system by introducing a temperature component to food consumption. This mod aims to provide a more immersive experience by making food temperature a factor in colonists' happiness. Originally created for RimWorld versions 1.1 and 1.2, the mod updates the meal system so that colonists prefer their meals at appropriate temperatures, enhancing their mood when food and drinks are served hot or cold as per their natural categories.

## Key Features and Systems

- **Food Temperature Tracking**: Each food item tracks its temperature and interacts thermodynamically with its surroundings.
- **Temperature Categories**: Foods are categorized into specific types such as Hot Meals, Cold Meals, Hot Drinks, Cold Drinks, etc., each with its own ideal consumption temperature.
- **Mood Modifiers**: Eating food at the ideal temperature provides mood boosts, while deviations lead to mood penalties.
- **Appliances for Heating**: Adds appliances like microwaves and heat lamps to help maintain or alter food temperatures to desired levels.
- **Modular and Configurable**: Extensive configuration options allow users to adjust settings according to their preferences.
- **Compatibility**: Supports various modded foods from other mods like Vanilla Cooking Expanded, VGP Vegetable Garden, and more.

## Coding Patterns and Conventions

- **OOP Principles**: Uses consistent object-oriented programming practices, encapsulating logic within classes such as `CompDFoodTemperature` and `CompDTempControl`.
- **Design Patterns**: Implements component-based architecture for food temperature features, using well-defined interfaces and compositional relationships.
- **C# Naming Conventions**: Follows standard C# naming conventions with `PascalCase` for class and method names (`DHotMealsHarmonyPatches`, `GetPositiveMoodEffect`).

## XML Integration

- XML is primarily used for defining game data properties and extensions, adhering to RimWorld's XML-based modding format.
- Implementing new foods or altering existing ones can be done via XML patch files, such as category assignments and property changes.

## Harmony Patching

- **Harmony Library**: Utilizes Harmony for runtime patching, allowing for seamless integration and modification of RimWorld's base game behaviors.
- **Patch Classes**: Contains dedicated patch classes such as `DHotMealsHarmonyPatches` and `DThermodynamicsCoreHarmonyPatches` to manage different hooks and alterations.
- **Method Transpiling and Prefixes**: Employs both transpilers and prefix methods to adjust existing methods for new temperature dynamics.

## Suggestions for Copilot

When using GitHub Copilot to extend this mod or develop new features, consider the following:

- **Pattern Recognition**: Leverage existing patterns within the codebase. For instance, use `CompDTemperature` as a base when adding new temperature-related components.
- **Harmony Practices**: For patching, follow the existing approach seen in `DThermodynamicsCoreHarmonyPatches` to ensure consistent and error-free modifications.
- **Code Comments**: Maintain comprehensive comments for complex algorithms, especially when dealing with temperature diffusion and mood impact calculations.
- **XML and C# Sync**: Ensure coherent integration between XML data definitions and C# implementations, keeping data-driven designs flexible.
- **Modular Architecture**: Add features as separate components or classes to keep the mod extensible and maintainable.

By adhering to these guidelines and utilizing GitHub Copilot effectively, developers can efficiently modify or extend the [D] Thermodynamics - Hot Meals mod to add additional features or support new content.
