using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class RockSpawnerAuthoringComponent: MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    
    public int initSpawnNumber;
    public float spawnFrequency;
    public float3 spawnVelocity;
    public GameObject meshPrefab;
    public Entity prefab;
    public float2 xKillPlanes;
    public float2 xSpawnRanges;
    public float2 radiusRanges;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Create entity prefab from the game object hierarchy once
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(meshPrefab, settings);
        
        Random convertRNG = new Random(0x8675309);

        Entity spawner = conversionSystem.CreateAdditionalEntity(gameObject);
        dstManager.SetName(spawner, "Rock Spawner");
        dstManager.AddComponentData(spawner, new RockSpawnComponent
        {
            rng = new Random(7),
            spawnTimeRemaining = spawnFrequency,
            spawnTime = spawnFrequency,
            spawnVelocity = spawnVelocity,
            prefab = prefab,
            radiusRanges = radiusRanges
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
            
            Entity rock = dstManager.Instantiate(prefab);
            float randRadius = convertRNG.NextFloat(radiusRanges.x, radiusRanges.y);

            dstManager.SetName(rock, "Init Rock " + i);
            dstManager.AddComponentData(rock, new RockVelocityComponentData
            {
                value = spawnVelocity
            });
            dstManager.AddComponentData(rock, new RockDestroyBounds()
            {
                Value = xKillPlanes
            });
            dstManager.AddComponentData(rock, new RockRadiusComponentData
            {
                value = randRadius,
            });
            dstManager.SetComponentData(rock,new Translation
            {
                //initial spawn is based across the conveyour; NOT to be confused with the spawnRange for runtime 
                //created rocks
                Value = new float3(0.8f * convertRNG.NextFloat(xKillPlanes.x,xKillPlanes.y),0f,1.5f)
            });
            dstManager.SetComponentData(rock, new NonUniformScale
            {
                Value = new float3(randRadius,randRadius,randRadius)
            });
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(meshPrefab);
    }
}
