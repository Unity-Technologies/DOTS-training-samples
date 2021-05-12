using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class DebugParabolaAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    [Range(1, 40)]
    public int SampleCount = 16;

    public float Duration = 4.0f;

    public bool Enable = false;

    public GameObject SamplePrefab;
    
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        if (!Enable)
            return;

        referencedPrefabs.Add(SamplePrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (!Enable)
            return;

        dstManager.AddComponentData(entity, new DebugParabolaData
        {
            SampleCount = SampleCount,
            Duration = Duration,
            SamplePrefab = conversionSystem.GetPrimaryEntity(SamplePrefab),
        });
        dstManager.AddComponent<DebugParabolaSpawnerTag>(entity);
    }
}
