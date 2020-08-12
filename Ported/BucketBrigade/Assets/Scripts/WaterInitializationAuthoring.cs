using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public struct WaterInitialization : IComponentData
{
    public int   splashRadius; 
    public float coolingStrength; 
    public float coolingStrength_falloff; 
    public float refillRate; 
    public int   totalBuckets;
    public float bucketCapacity; 
    public float bucketFillRate; 
    public float bucketSize_EMPTY;
    public float bucketSize_FULL; 
}

public class WaterInitializationAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int   SplashRadius = 3;
    public float CoolingStrength = 1;
    public float CoolingStrength_falloff = 0.75f;
    public float RefillRate = 0.0001f;
    public int   TotalBuckets = 3;
    public float BucketCapacity = 3f;
    public float BucketFillRate = 0.01f;
    public float BucketSize_EMPTY = 0.2f;
    public float BucketSize_FULL = 0.4f;

    public void Convert(Entity entity, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new WaterInitialization
        {
            splashRadius = SplashRadius,
            coolingStrength = CoolingStrength,
            coolingStrength_falloff = CoolingStrength_falloff,
            refillRate = RefillRate,
            totalBuckets = TotalBuckets,
            bucketCapacity = BucketCapacity,
            bucketFillRate = BucketFillRate,
            bucketSize_EMPTY = BucketSize_EMPTY,
            bucketSize_FULL = BucketSize_FULL,
        });
    }
}