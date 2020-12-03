using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ResourceSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject resourcePrefab;
    public float spawnRate;
    float spawnTimer = 0f;
    public int beesPerResource;
    public int startResourceCount;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawner = new resourceSpawner
        {
            resPrefab = conversionSystem.GetPrimaryEntity(resourcePrefab),
            spawnRate = this.spawnRate,
            spawnTimer = this.spawnTimer,
            beesPerResource = this.beesPerResource,
            count = this.startResourceCount,
            isPosRandom = true
        };

        dstManager.AddComponentData<resourceSpawner>(entity, spawner);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(this.resourcePrefab);
    }
}

public struct resourceSpawner : IComponentData
{
    public Entity resPrefab;
    public float spawnRate;
    public float spawnTimer;
    public int beesPerResource;
    public int count;
    public bool isPosRandom;
}