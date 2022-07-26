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
    }
}