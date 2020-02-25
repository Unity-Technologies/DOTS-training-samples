
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[RequiresEntityConversion]
[AddComponentMenu("ESC Thrower/Projectile Spawner")]
public class ProjectileSpawnerAuthoringComponent: MonoBehaviour,IConvertGameObjectToEntity,IDeclareReferencedPrefabs
{
    public float2 projeciltLifetimeRange;
    public float2 velRange;
    public BoxCollider boxCollider;
    public float frequency;
    public GameObject projectilePrefab;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var boxTransform = boxCollider.transform;
        ProjectileSpawnerComponentData data = new ProjectileSpawnerComponentData
        {
            lifetimeRange = projeciltLifetimeRange,
            velocityRange = velRange,
            velocityDirection = boxTransform.forward,
            timeUntilSpawn = frequency,
            spawnTime = frequency,
            projectilePrefab = conversionSystem.GetPrimaryEntity(projectilePrefab)
        };

        dstManager.AddComponentData(entity, data);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(projectilePrefab);
    }
}
