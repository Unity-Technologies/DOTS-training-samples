using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class DebugParabolaAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    [Range(1, 20)]
    public int SampleCount;

    public GameObject SamplePrefab;
    
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(SamplePrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new DebugParabolaData
        {
            SampleCount = SampleCount,
            SamplePrefab = conversionSystem.GetPrimaryEntity(SamplePrefab),
        });
        dstManager.AddComponent<DebugParabolaSpawnerTag>(entity);
    }
}
