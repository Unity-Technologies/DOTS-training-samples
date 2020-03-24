using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[RequiresEntityConversion]
[AddComponentMenu("ECS Thrower/Target Spawner")]
public class TargetSpawnerAuthoringComponent: MonoBehaviour, IConvertGameObjectToEntity,IDeclareReferencedPrefabs
{
    public GameObject meshPrefab;
    public float yBoundsMin;
    public float yBoundsMax;
    public float xBoundsMin;
    public float xBoundsMax;
    public float targetVelX;
    public float spawnFrequency;
    public int initSpawnNumber;
    public float reach = 1f;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        
        TargetSpawnerComponentData spawnComponent = new TargetSpawnerComponentData()
        {
            yRange = new float2(yBoundsMin,yBoundsMax),
            xRange = new float2(xBoundsMin,xBoundsMax),
            velocityX = targetVelX,
            spawnFrequency = spawnFrequency,
            initSpawn = initSpawnNumber,
            spawnZ = transform.position.z,
            prefab = conversionSystem.GetPrimaryEntity(meshPrefab),
        };
        
        //give converted spawner entitiy component data for initial spawn
        dstManager.AddComponentData(entity, spawnComponent);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(meshPrefab);
    }
}
