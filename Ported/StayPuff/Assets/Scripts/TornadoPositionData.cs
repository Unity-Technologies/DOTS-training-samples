using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TornadoPositionData : IComponentData
{
    public float3 Position;
}