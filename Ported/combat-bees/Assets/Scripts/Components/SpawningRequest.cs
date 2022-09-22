using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SpawningRequest : IComponentData
{
    public Entity Prefab;
    public int Faction;
    public float3 InitVelocity;
    public AABB Aabb;
    public int Count;
    public Color Color;
}
