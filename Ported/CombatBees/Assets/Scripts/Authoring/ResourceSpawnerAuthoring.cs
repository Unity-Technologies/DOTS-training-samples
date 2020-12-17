using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

//public class ResourceSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
public class ResourceSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    //public GameObject resourcePrefab;
    //public float spawnRate;
    //float spawnTimer = 0f;
    //public int beesPerResource;
    public int startResourceCount;
    public int maxGeneratedByMouseClick;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawner = new ResourceSpawner
        {
            //resPrefab = conversionSystem.GetPrimaryEntity(resourcePrefab),
            //spawnRate = this.spawnRate,
            //spawnTimer = this.spawnTimer,
            //beesPerResource = this.beesPerResource,
            count = this.startResourceCount,
            isPosRandom = true,
            //mouseClicked = false
        };

        dstManager.AddComponentData<ResourceSpawner>(entity, spawner);

        //dstManager.AddBuffer<RayCastPos>(entity);
        /*
        var rayCastPosBuf = dstManager.AddBuffer<RayCastPos>(entity);
        rayCastPosBuf.EnsureCapacity(maxGeneratedByMouseClick);
        for(int i = 0; i < maxGeneratedByMouseClick; i++)
        {
            rayCastPosBuf.Add(new RayCastPos { pos = new float3(0f, 0f, 0f) });
        }
        */
    }

    /*
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(this.resourcePrefab);
    }
    */
}

public struct ResourceSpawner : IComponentData
{
    //public Entity resPrefab;
    //public float spawnRate;
    //public float spawnTimer;
    //public int beesPerResource;
    public int count;
    public bool isPosRandom;
    //public bool mouseClicked;
}

/*
public struct RayCastPos : IBufferElementData
{
    public float3 pos;
}
*/
