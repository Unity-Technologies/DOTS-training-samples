using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public GameObject BotPrefab;
    public GameObject BucketPrefab;
    public GameObject FireCell;
    public GameObject WaterCell;

    public Color ScooperColor;
    public Color FillerColor;
    public Color PasserColor;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Spawner
        {
            BotPrefab = conversionSystem.GetPrimaryEntity(BotPrefab),
            BucketPrefab = conversionSystem.GetPrimaryEntity(BucketPrefab),
            FireCell = conversionSystem.GetPrimaryEntity(FireCell),
            WaterCell = conversionSystem.GetPrimaryEntity(WaterCell),

            ScooperColor = new float4(ScooperColor.r, ScooperColor.g, ScooperColor.b, ScooperColor.a),
            FillerColor = new float4(FillerColor.r, FillerColor.g, FillerColor.b, FillerColor.a),
            PasserColor = new float4(PasserColor.r, PasserColor.g, PasserColor.b, PasserColor.a),
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BotPrefab);
        referencedPrefabs.Add(BucketPrefab);
        referencedPrefabs.Add(FireCell);
        referencedPrefabs.Add(WaterCell);
    }
}