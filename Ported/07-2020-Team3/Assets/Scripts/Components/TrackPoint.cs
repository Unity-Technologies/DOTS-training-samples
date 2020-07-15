using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
[InternalBufferCapacity(8)]
public struct TrackPoint : IBufferElementData
{
    public float3 position;
}
