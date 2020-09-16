using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SpawnData : IComponentData
{
    public float3 position;
    public int height;
}