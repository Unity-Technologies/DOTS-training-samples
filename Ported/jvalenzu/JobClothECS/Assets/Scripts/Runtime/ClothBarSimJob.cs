using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

[BurstCompile]
struct ClothBarSimJob : IJob
{
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<float3> vertices;
    [ReadOnly]
    public NativeArray<ClothConstraint> constraints;

    // p0 | p1 | c0 | d0
    // -----------------
    //  0 |  0 |  1 |  1
    //  0 |  1 |  2 |  0
    //  1 |  0 |  0 |  2
    //  1 |  1 |  0 |  0
    readonly static float[] scaleX = {
        1.0f,
        2.0f,
        0.0f,
        0.0f
    };
    readonly static float[] scaleY = {
        1.0f,
        0.0f,
        2.0f,
        0.0f
    };

    public void Execute() {
        for (int i=0,n=constraints.Length; i<n; ++i)
        {
            ClothConstraint constraint = constraints[i];

            float length = constraint.length * 1.953125e-3f; // (1/256) * (1/2)
            float3 p1 = vertices[constraint.x];
            float3 p2 = vertices[constraint.y];
            float3 v0 = p2 - p1;
            float3 v1 = v0 * (0.5f - length / math.length(v0));

            int pinPairIndex = constraint.pinPair;
            p1 += v1 * scaleX[pinPairIndex];
            p2 -= v1 * scaleY[pinPairIndex];

            vertices[constraint.x] = p1;
            vertices[constraint.y] = p2;
        }
    }
}
