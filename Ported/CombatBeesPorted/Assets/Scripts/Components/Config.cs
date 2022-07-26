using Unity.Entities;
using Unity.Mathematics;

public struct Config : IComponentData
{
    public float BloodDuration;
    public float ExplosionDuration;
    public float HitDistance;
    public float GrabDistance;
    public float HiveDepth;
    public float GravityDown;
    public float3 PlayVolume;
    public int StartingBeeCount;
    public int StartingResourceCount;
    
    public Entity BeePrefab;
    public Entity ResourcePrefab;
    public Entity ExplosionPrefab;
    public Entity BloodPrefab;
}