using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct SpawnComponent : IComponentData
{
    public float3 SpawnPosition;
    public int Count;    
}
