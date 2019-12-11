using src;
using Unity.Mathematics;

static internal class BezierMath
{
    public static float3 GetPoint(LineSegmentBufferElement segment, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;
        float3 p = uuu * segment.p0;
        p += 3 * uu * t * segment.p1;
        p += 3 * u * tt * segment.p2;
        p += ttt * segment.p3;
        return p;
    }
}