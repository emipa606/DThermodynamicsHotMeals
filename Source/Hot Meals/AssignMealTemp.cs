using System.Collections.Generic;
using System.Xml;
using DHotMeals;

namespace Verse;

public class AssignMealTemp : PatchOperation
{
    public List<Meal> Meals;

    protected override bool ApplyWorker(XmlDocument xml)
    {
        foreach (var m in Meals)
        {
            MealAssigner.fixedTypes.SetOrAdd(m.name, m.type);
        }

        return true;
    }
}