using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct GameConfig : IComponentData
{
    public Entity BeePrefab;
    public uint BeeSpawnCountOnResourceDrop;
    [Range(0, 1)] public float TeamABeeAggressivity;
    [Range(0, 1)] public float TeamBBeeAggressivity;
    public float NormalSpeed;
    public float CarryingSpeed;
    public float AttackSpeed;
    public float Gravity;
    public Entity ResourcePrefab;
    public float3 PlayingFieldSize;
    public Entity BloodPrefab;
}