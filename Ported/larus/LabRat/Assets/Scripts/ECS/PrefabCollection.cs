using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

// These prefabs do not need to be synchronized over the network, so can be spawned
// normally (not using GhostPrefabCollectionComponent
public struct PrefabCollectionComponent : IComponentData
{
    public Entity MouseSpawner;
    public Entity CatSpawner;
}

public class PrefabCollection : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject MouseSpawner;
    public GameObject CatSpawner;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity,
            new PrefabCollectionComponent
            {
                MouseSpawner = conversionSystem.GetPrimaryEntity(MouseSpawner),
                CatSpawner = conversionSystem.GetPrimaryEntity(CatSpawner)
            });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(MouseSpawner);
        referencedPrefabs.Add(CatSpawner);
    }
}
