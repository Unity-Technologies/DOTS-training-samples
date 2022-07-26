using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;

namespace Components
{
    public struct Track: IComponentData
    {
        [ReadOnly] public float3 StartPos;
        [ReadOnly] public float3 StartNorm;
        [ReadOnly] public float3 StartTang;
        [ReadOnly] public float3 EndPos;
        [ReadOnly] public float3 EndNorm;
        [ReadOnly] public float3 EndTang;

        // This code execution should be a job
        public float3 Evaluate(float t)
        {
            var anchor1 = StartPos + StartTang;
            var anchor2 = EndPos - EndTang;
            return StartPos * (1f - t) * (1f - t) * (1f - t) + 3f * anchor1 * (1f - t) * (1f - t) * t + 3f * anchor2 * (1f - t) * t * t + EndPos * t * t * t;
        }
    }
}