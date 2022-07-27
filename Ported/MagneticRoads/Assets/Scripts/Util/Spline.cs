using Unity.Burst;
using Unity.Mathematics;
using UnityEngine.UIElements;

namespace Util
{
    public static class Spline
    {
        [BurstCompile]
        public struct RoadTerminator
        {
            public float3 Position;
            public float3 Normal;
            public float3 Tangent;
        }
        
        [BurstCompile]
        public static float3 Evaluate(RoadTerminator start, RoadTerminator end, float t)
        {
            var anchor1 = start.Position + start.Tangent;
            var anchor2 = end.Position - end.Tangent;
            return start.Position * (1f - t) * (1f - t) * (1f - t) + 3f * anchor1 * (1f - t) * (1f - t) * t + 3f * anchor2 * (1f - t) * t * t + end.Position * t * t * t;
        }
    }
}