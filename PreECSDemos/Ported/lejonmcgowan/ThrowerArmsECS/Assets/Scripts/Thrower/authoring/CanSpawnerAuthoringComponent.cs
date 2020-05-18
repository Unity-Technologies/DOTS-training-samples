using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class CanSpawnerAuthoringComponent: MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    
    public int spawnNumber;
    public float3 spawnVelocity;
    public GameObject meshPrefab;
    public Entity prefab;
    public float2 xKillPlanes;
    public float2 ySpawnRanges;
    public float zSpawnPos;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Create entity prefab from the game object hierarchy once
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(meshPrefab, settings);
        
        Random convertRNG = new Random(0x8857921);
    
        for (int i = 0; i < spawnNumber; i++)
        {
            
            Entity can = dstManager.Instantiate(prefab);

            float3 initPos = new float3(0.8f * convertRNG.NextFloat(xKillPlanes.x, xKillPlanes.y),
                convertRNG.NextFloat(ySpawnRanges.x, ySpawnRanges.y),
                zSpawnPos);
                
            dstManager.SetName(can, "Can " + i);
            dstManager.AddComponentData(can, new Velocity()
            {
                Value = spawnVelocity
            });
            dstManager.AddComponentData(can, new DestroyBoundsX()
            {
                Value = xKillPlanes
            });
            dstManager.AddComponentData(can, new DestroyBoundsY()
            {
                Value = -10f
            });
            dstManager.AddComponentData(can, new CanInitSpeed
            {
                initPos = initPos,
                initVel = spawnVelocity
            });
            dstManager.AddComponentData(can, new CanTag());
            dstManager.SetComponentData(can,new Translation
            {
                //initial spawn is based across the conveyour; NOT to be confused with the spawnRange for runtime 
                //created rocks
                Value = initPos
            });
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(meshPrefab);
    }
}
