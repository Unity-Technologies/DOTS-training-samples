using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[RequiresEntityConversion]
[AddComponentMenu("ECS Thrower/Projectile Spawner")]
public class ProjectileSpawnerAuthoringComponent: MonoBehaviour, IConvertGameObjectToEntity,IDeclareReferencedPrefabs
{
    public GameObject meshPrefab;
    public float minRadius;
    public float maxRadius;
    public float xBoundsMin;
    public float xBoundsMax;
    public float projectileVelX;
    public float spawnFrequency;
    public int initSpawnNumber;
    [Range(1,1000)]
    public int numBuckets = 1;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        
        ProjectileSpawnerComponentData spawnComponent = new ProjectileSpawnerComponentData()
        {
            radiusRange = new float2(minRadius,maxRadius),
            xRange = new float2(xBoundsMin,xBoundsMax),
            velocityX = projectileVelX,
            spawnFrequency = spawnFrequency,
            spawnRamaining = spawnFrequency,
            initSpawn = initSpawnNumber,
            spawnZ = transform.position.z,
            prefab = conversionSystem.GetPrimaryEntity(meshPrefab),
            numBuckets = numBuckets
        };
        
        //give converted spawner entitiy component data for initial spawn
        dstManager.AddComponentData(entity, spawnComponent);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(meshPrefab);
    }
}
