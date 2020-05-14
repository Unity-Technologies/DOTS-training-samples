using Unity.Mathematics;

public static class AnimUtils
{
    public static readonly float gravityStrength = 25f;
    public static float EvauateThrowCurveSmooth(float t)
    {
        float tPeak = 0.28f;

        float peak = 1.4f;
        if (t < tPeak)
            return peak * math.smoothstep(0, tPeak, t);
        return peak * (1 - math.smoothstep(tPeak, 1, t));
    }
};