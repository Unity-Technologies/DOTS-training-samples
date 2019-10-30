using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Spawner : IComponentData
{
    public Entity Prefab;
    public float3 Position;
    public int Amount;
}
