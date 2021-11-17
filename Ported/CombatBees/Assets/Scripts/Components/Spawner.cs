using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Spawner : IComponentData
{
    public Entity Prefab;
    public float3 SpawnPosition;
    public int Count;    
}
 