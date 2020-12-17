using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class DebugCreateBucket : MonoBehaviour
{
    public void SpawnBucket()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        int team = (int)(Random.value * FireSimConfig.maxTeams) % FireSimConfig.maxTeams;

        var entity = entityManager.Instantiate(BucketSpawnerSystem.s_BucketPrefabEntity);
        entityManager.AddComponentData(entity, new Bucket      { LinearT = 0.0f });
        entityManager.AddComponentData(entity, BucketOwner.CreateBucketOwner(team) );
        entityManager.AddComponentData(entity, new WaterLevel  { Value = 1.0f });
        entityManager.AddComponentData(entity, new Position());
        entityManager.AddComponentData(entity, new Translation());
    }
};
