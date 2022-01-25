using Unity.Mathematics;

public static class FloatRangeExtensions
{
    public static float RandomInRange(this FloatRange range, Random rnd)
    {
        return rnd.NextFloat(range.Min, range.Max);
    }
}