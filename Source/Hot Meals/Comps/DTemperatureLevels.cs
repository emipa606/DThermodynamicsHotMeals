namespace DThermodynamicsCore.Comps;

public class DTemperatureLevels(
    float good = 37f,
    float ok = 20f,
    float bad = 10f,
    float reallyBad = 0.01f,
    float frozen = 0f)
{
    public readonly float badTemp = bad;

    public readonly float goodTemp = good;
    public readonly float okTemp = ok;
    public readonly float reallyBadTemp = reallyBad;
    public float frozenTemp = frozen;
}