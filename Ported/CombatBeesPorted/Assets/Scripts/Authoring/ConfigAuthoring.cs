using Unity.Entities;
using UnityEngine;

class ConfigAuthoring : MonoBehaviour
{
    [Header("Setup")]
    public int StartingBeeCount = 10;
    public int StartingResourceCount = 100;
    
    [Header("Animation")]
    public float BloodDuration = 3f;
    public float ExplosionDuration = 3f;
    
    [Header("Bees")]
    public float InteractionDistance = 1f;
    public float JitterTimeMin = 0.1f;
    public float JitterTimeMax = 0.9f;
    public float JitterDistanceMax = 3f;
    public float BeeMoveSpeed = 6f;
    public int BeesPerResource = 8;
    [Range(0f, 1f)]
    public float Aggressiveness;
    
    [Header("World")]
    public Vector3 PlayVolume;
    public float HiveDepth = 10f;
    public float GravityDown = 20f;
    
    [Header("Prefabs")]
    public UnityEngine.GameObject BeePrefab;
    public UnityEngine.GameObject ResourcePrefab;
    public UnityEngine.GameObject ExplosionPrefab;
    public UnityEngine.GameObject BloodPrefab;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config()
        {
            Aggressiveness = authoring.Aggressiveness,
            BloodDuration = authoring.BloodDuration,
            ExplosionDuration = authoring.ExplosionDuration,
            InteractionDistance = authoring.InteractionDistance,
            HiveDepth = authoring.HiveDepth,
            GravityDown = authoring.GravityDown,
            PlayVolume = authoring.PlayVolume,
            StartingBeeCount = authoring.StartingBeeCount,
            StartingResourceCount = authoring.StartingResourceCount,
            JitterTimeMin = authoring.JitterTimeMin,
            JitterTimeMax = authoring.JitterTimeMax,
            JitterDistanceMax = authoring.JitterDistanceMax,
            BeeMoveSpeed = authoring.BeeMoveSpeed,
            BeesPerResource = authoring.BeesPerResource,
            
            BeePrefab = GetEntity(authoring.BeePrefab),
            ResourcePrefab = GetEntity(authoring.ResourcePrefab),
            ExplosionPrefab = GetEntity(authoring.ExplosionPrefab),
            BloodPrefab = GetEntity(authoring.BloodPrefab)
        });
    }
}