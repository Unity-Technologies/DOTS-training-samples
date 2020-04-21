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
            prefab = prefab
        });
        dstManager.AddComponentData(spawner, new RockBounds()
        {
            range = xKillPlanes
        });

        for (int i = 0; i < initSpawnNumber; i++)
        {
            
            Entity rock = dstManager.Instantiate(prefab);
            dstManager.SetName(spawner, "Init Rock " + i);
            dstManager.AddComponent<RockTag>(rock);
            dstManager.AddComponentData(rock, new RockVelocityComponentData
            {
                value = spawnVelocity
            });
            dstManager.AddComponentData(rock, new RockBounds
            {
                range = xKillPlanes
            });
            dstManager.SetComponentData(rock,new Translation
            {
                Value = new float3(0.8f * convertRNG.NextFloat(-10f,10f),0f,2.0f)
            });
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(meshPrefab);
    }
}
