namespace DThermodynamicsCore.Comps;

public class DTemperatureLevels
{
    public float badTemp;
    public float frozenTemp;

    public float goodTemp;
    public float okTemp;
    public float reallyBadTemp;

    public DTemperatureLevels(float good = 37f, float ok = 20f, float bad = 10f, float reallyBad = 0.01f,
        float frozen = 0f)
    {
        goodTemp = good;
        okTemp = ok;
        badTemp = bad;
        reallyBadTemp = reallyBad;
        frozenTemp = frozen;
    }
}