using Unity.Entities;
using Unity.Mathematics;

public struct Config : IComponentData
{
    public float Aggressiveness;
    public float BloodDuration;
    public float ExplosionDuration;
    public float InteractionDistance;
    public float HiveDepth;
    public float GravityDown;
    public float3 PlayVolume;
    public int StartingBeeCount;
    public int StartingResourceCount;
    public float JitterTimeMin;
    public float JitterTimeMax;
    public float JitterDistanceMax;
    public float BeeMoveSpeed;
    public int BeesPerResource;
    
    public Entity BeePrefab;
    public Entity ResourcePrefab;
    public Entity ExplosionPrefab;
    public Entity BloodPrefab;
}