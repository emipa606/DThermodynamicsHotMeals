using RimWorld;
using Verse;

namespace DHotMeals.Comps;

public class CompDSecondLayer : ThingComp
{
    private Graphic graphicInt;

    private CompProperties_DSecondLayer Props => (CompProperties_DSecondLayer)props;

    protected virtual Graphic Graphic
    {
        get
        {
            if (graphicInt != null)
            {
                return graphicInt;
            }

            if (Props.graphicData == null)
            {
                var def = parent.def;
                Log.ErrorOnce(
                    $"{def} has no DSecondLayer graphicData but we are trying to access it.",
                    764533);
                return BaseContent.BadGraphic;
            }

            graphicInt = Props.graphicData.GraphicColoredFor(parent);

            return graphicInt;
        }
    }

    public override void PostDraw()
    {
        base.PostDraw();
        Graphic.Draw(GenThing.TrueCenter(parent.Position, parent.Rotation, parent.def.size, Props.Altitude),
            parent.Rotation, parent);
    }
}