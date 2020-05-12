using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class CanSpawnerAuthoringComponent: MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    
    public int initSpawnNumber;
    public float spawnFrequency;
    public float3 spawnVelocity;
    public GameObject meshPrefab;
    public Entity prefab;
    public float2 xKillPlanes;
    public float2 xSpawnRanges;
    public float2 ySpawnRanges;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Create entity prefab from the game object hierarchy once
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(meshPrefab, settings);
        
        Random convertRNG = new Random(0x8857921);

        Entity spawner = conversionSystem.CreateAdditionalEntity(gameObject);
        dstManager.SetName(spawner, "Can Spawner");
        dstManager.AddComponentData(spawner, new CanSpawnComponent
        {
            rng = new Random(7),
            spawnTimeRemaining = spawnFrequency,
            spawnTime = spawnFrequency,
            spawnVelocity = spawnVelocity,
            prefab = prefab,
            yRanges = ySpawnRanges,
            xSpawnPos = xSpawnRanges.x,
            zSpawnPos = 5f,
            bounds = xKillPlanes
        });
        dstManager.AddComponentData(spawner, new RockDestroyBounds()
        {
            Value = xKillPlanes
        });
        dstManager.AddComponentData(spawner, new RockSpawnerBounds()
        {
            Value= xSpawnRanges
        });

        for (int i = 0; i < initSpawnNumber; i++)
        {
            
            Entity can = dstManager.Instantiate(prefab);

            dstManager.SetName(can, "Init Can " + i);
            dstManager.AddComponentData(can, new CanVelocity
            {
                Value = spawnVelocity
            });
            dstManager.AddComponentData(can, new CanDestroyBounds
            {
                Value = xKillPlanes
            });
            dstManager.SetComponentData(can,new Translation
            {
                //initial spawn is based across the conveyour; NOT to be confused with the spawnRange for runtime 
                //created rocks
                Value = new float3(0.8f * convertRNG.NextFloat(xKillPlanes.x,xKillPlanes.y),
                    convertRNG.NextFloat(ySpawnRanges.x,ySpawnRanges.y),
                    5f)
            });
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(meshPrefab);
    }
}
