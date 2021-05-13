using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlatformSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject PlatformPrefab;
    
    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(PlatformPrefab);
    }
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Entity platformPrefabEntity = conversionSystem.GetPrimaryEntity(PlatformPrefab);

        dstManager.AddComponentData(entity, new PlatformSpawnerComponent()
        {
            PlatformPrefab = platformPrefabEntity
        });
    }
}
