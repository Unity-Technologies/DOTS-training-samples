using Unity.Mathematics;

public static class AnimUtils
{
    public static readonly float gravityStrength = 25f;
    //1.8^2
    public static readonly float reachDistSq = 3.24f;
    public static readonly float reachDuration = 1f;
    public static readonly float windupDuration = 0.7f;
    public static readonly float throwDuration = 1.2f;
    public static float EvauateThrowCurveSmooth(float t)
    {
        float tPeak = 0.28f;

        float peak = 1.4f;
        if (t < tPeak)
            return peak * math.smoothstep(0, tPeak, t);
        return peak * (1 - math.smoothstep(tPeak, 1, t));
    }
};