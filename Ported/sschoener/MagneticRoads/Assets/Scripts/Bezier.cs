using System;
using Unity.Mathematics;

static class Bezier
{
    public static float3 Evaluate(this in CubicBezier bezier, float t) => 
        (1f - t) * (1f - t) * (1f - t) * bezier.start +
        3f * t * (1f - t) * (1f - t) * bezier.anchor1 +
        3f * t * (1f - t) * t * bezier.anchor2 +
        t * t * t * bezier.end;

    public static float MeasureLength(this in CubicBezier bezier, int numSamples)
    {
        float measuredLength = 0f;
        float3 point = bezier.Evaluate(0f);
        for (int i = 1; i <= numSamples; i++)
        {
            float3 newPoint = bezier.Evaluate((float)i / numSamples);
            measuredLength += math.length(newPoint - point);
            point = newPoint;
        }

        return measuredLength;
    }
}