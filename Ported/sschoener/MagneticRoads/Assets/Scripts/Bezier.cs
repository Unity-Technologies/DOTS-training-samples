using System;
using Unity.Mathematics;

static class Bezier
{
    public static float3 Evaluate(this in CubicBezier bezier, float t) => 
        (1f - t) * (1f - t) * (1f - t) * bezier.start +
        3f * t * (1f - t) * (1f - t) * bezier.anchor1 +
        3f * t * (1f - t) * t * bezier.anchor2 +
        t * t * t * bezier.end;
    
}
