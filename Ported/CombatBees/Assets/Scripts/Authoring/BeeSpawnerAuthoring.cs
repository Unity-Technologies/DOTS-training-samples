using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BeeSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject beePrefab;
    public int initialBeeCount;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawner = new BeeSpawner
        {
            beePrefab = conversionSystem.GetPrimaryEntity(this.beePrefab),
            initialBeeCount = this.initialBeeCount,
        };

        dstManager.AddComponentData<BeeSpawner>(entity, spawner);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(this.beePrefab);
    }
}

public struct BeeSpawner : IComponentData
{
    public Entity beePrefab;
    public int initialBeeCount;
}