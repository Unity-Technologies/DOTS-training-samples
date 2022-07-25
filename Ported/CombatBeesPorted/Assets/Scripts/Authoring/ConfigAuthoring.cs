using Unity.Entities;
using Unity.Mathematics;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public float BloodDuration;
    public float ExplosionDuration;
    public float HitDistance;
    public float GrabDistance;
    public float HiveDepth;
    public float3 PlayVolume;
    public int StartingBeeCount;
    public int StartingResourceCount;
    
    public UnityEngine.GameObject BeePrefab;
    public UnityEngine.GameObject ResourcePrefab;
    public UnityEngine.GameObject ExplosionPrefab;
    public UnityEngine.GameObject BloodPrefab;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new ConfigComponent()
        {
            BloodDuration = authoring.BloodDuration,
            ExplosionDuration = authoring.ExplosionDuration,
            HitDistance = authoring.HitDistance,
            GrabDistance = authoring.GrabDistance,
            HiveDepth = authoring.HiveDepth,
            PlayVolume = authoring.PlayVolume,
            StartingBeeCount = authoring.StartingBeeCount,
            StartingResourceCount = authoring.StartingResourceCount,
            
            BeePrefab = GetEntity(authoring.BeePrefab),
            ResourcePrefab = GetEntity(authoring.ResourcePrefab),
            ExplosionPrefab = GetEntity(authoring.ExplosionPrefab),
            BloodPrefab = GetEntity(authoring.BloodPrefab)
        });
    }
}