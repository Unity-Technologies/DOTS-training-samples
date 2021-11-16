using System.Collections.Generic;
using System.Numerics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;
using UnityTransform = UnityEngine.Transform;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class SpawnerAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public UnityGameObject BeePrefab;
    public UnityGameObject FoodPrefab;
    public int StartingFoodCount;
    public float Length;
    public float Width;
    public float HiveDepth;

    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BeePrefab);
        referencedPrefabs.Add(FoodPrefab);
    }

    // This function is required by IConvertGameObjectToEntity
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        float hd = HiveDepth / 2.0f;
        float3 max = new float3(Length / 2.0f, Width / 2.0f, Width / 2.0f);
        float3 min = new float3(Length / -2.0f, Width / -2.0f, Width / -2.0f);
        float3 minHive = new float3(min.x + hd, 0.0f, 0.0f);
        float3 maxHive = new float3(max.x - hd, 0.0f, 0.0f);
        
        dstManager.AddComponentData(entity, new Spawner
        {
            BeePrefab = conversionSystem.GetPrimaryEntity(BeePrefab),
            FoodPrefab = conversionSystem.GetPrimaryEntity(FoodPrefab),
            BoundsMax = max,
            BoundsMin = min,
            BlueHiveCenter = minHive,
            YellowHiveCenter = maxHive,
            HiveDepth = HiveDepth,
            StartingFoodCount = StartingFoodCount
        });
    }
}