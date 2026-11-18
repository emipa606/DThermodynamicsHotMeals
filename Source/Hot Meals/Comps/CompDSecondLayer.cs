using RimWorld;
using Verse;

namespace DHotMeals.Comps;

public class CompDSecondLayer : ThingComp
{
    private CompProperties_DSecondLayer Props => (CompProperties_DSecondLayer)props;

    protected virtual Graphic Graphic
    {
        get
        {
            if (field != null)
            {
                return field;
            }

            if (Props.graphicData == null)
            {
                var def = parent.def;
                Log.ErrorOnce(
                    $"{def} has no DSecondLayer graphicData but we are trying to access it.",
                    764533);
                return BaseContent.BadGraphic;
            }

            field = Props.graphicData.GraphicColoredFor(parent);

            return field;
        }
    }

    public override void PostDraw()
    {
        base.PostDraw();
        Graphic.Draw(GenThing.TrueCenter(parent.Position, parent.Rotation, parent.def.size, Props.Altitude),
            parent.Rotation, parent);
    }
}