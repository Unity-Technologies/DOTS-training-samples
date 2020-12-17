using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class DebugCreateBucket : MonoBehaviour
{
    public enum BucketState
    {
        kEmpty,
        kFull
    };

    public void SpawnBucketInternal(BucketState bucketState)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        int team = (int)(Random.value * FireSimConfig.maxTeams) % FireSimConfig.maxTeams;

        var entity = entityManager.Instantiate(BucketSpawnerSystem.s_BucketPrefabEntity);
        entityManager.AddComponentData(entity, new Bucket      { LinearT = bucketState == BucketState.kFull ? 0.0f : 1.0f });
        entityManager.AddComponentData(entity, BucketOwner.CreateBucketOwner(team) );
        entityManager.AddComponentData(entity, new WaterLevel  { Value = bucketState == BucketState.kFull ? 1.0f : 0.0f });
        entityManager.AddComponentData(entity, new Position());
        entityManager.AddComponentData(entity, new Translation());
    }

    public void SpawnBucketFull()
    {
        SpawnBucketInternal(BucketState.kFull);
    }

    public void SpawnBucketEmpty()
    {
        SpawnBucketInternal(BucketState.kEmpty);
    }
};
