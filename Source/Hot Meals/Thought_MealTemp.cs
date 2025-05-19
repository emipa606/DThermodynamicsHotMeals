using System;
using DHotMeals.Comps;
using RimWorld;
using Verse;

namespace DHotMeals;

internal class Thought_MealTemp : Thought_Memory
{
    public CompDFoodTemperature comp = null;

    public double createdTemp = 27;

    public override string Description =>
        comp == null
            ? "HoMe.Consumedsomething".Translate()
            : "HoMe.ConsumedsomethingTT".Translate(comp.parent.Label, comp.GetState(createdTemp));

    public override string LabelCap => comp == null
        ? "HoMe.Consumedsomething".Translate()
        : "HoMe.ConsumedsomethingTTShort".Translate(comp.GetFoodType(), comp.GetState(createdTemp));

    public override float MoodOffset()
    {
        return GetMoodValue();
    }

    public override bool GroupsWith(Thought other)
    {
        if (comp == null)
        {
            return false;
        }

        if (other is not Thought_MealTemp otherMeal)
        {
            return false;
        }

        if (comp.PropsTemp.mealType == MealTempTypes.None)
        {
            return false;
        }

        return comp.PropsTemp.mealType == otherMeal.comp.PropsTemp.mealType;
    }

    public override bool TryMergeWithExistingMemory(out bool showBubble)
    {
        var thoughts = pawn.needs.mood.thoughts;
        if (thoughts.memories.NumMemoriesInGroup(this) >= 1)
        {
            if (thoughts.memories.OldestMemoryInGroup(this) is Thought_MealTemp thought_Memory)
            {
                var moodVal = GetMoodValue();
                if (moodVal == 0) // this has 0 value, don't add it
                {
                    showBubble = false;
                }
                else if (thought_Memory.GetMoodValue() == moodVal) // other value is not 0 and equals this value
                {
                    showBubble = thought_Memory.age > thought_Memory.def.DurationTicks / 2;
                    thought_Memory.Renew();
                }
                else // different values, this one is non-zero
                {
                    thoughts.memories.RemoveMemory(thought_Memory);
                    thoughts.memories.Memories.Add(this);
                    showBubble = true;
                }

                return true;
            }
        }

        showBubble = true;
        return false;
    }

    private int GetMoodValue()
    {
        if (comp == null)
        {
            return 0;
        }

        if (comp.PropsTemp.noHeat)
        {
            if (createdTemp <= 0)
            {
                return 3 * HotMealsSettings.negativeMoodDebuff;
            }

            return 0;
        }

        var val = comp.GetPositiveMoodEffect(createdTemp);
        if (val > 0)
        {
            return val * HotMealsSettings.positiveMoodBuff;
        }

        val = comp.GetNegativeMoodEffect(createdTemp);
        return Math.Abs(val) * HotMealsSettings.negativeMoodDebuff;
    }
}