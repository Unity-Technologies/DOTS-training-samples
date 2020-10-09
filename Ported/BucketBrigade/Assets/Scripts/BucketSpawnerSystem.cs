using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;

//*
// BoardSpawnerSystem removes the entity that has a BoardSpawner component after it initializes itself.
// Update before that system to ensure we can read the board settings before then.
[UpdateBefore(typeof(BoardSpawnerSystem))]
/*/
 // for testing / validation purposes. 
 // force this system to update after BoardSpawner (by default it updates before BoardSpawner, so this is needed)
[UpdateAfter(typeof(BoardSpawnerSystem))] 
/**/
public class BucketSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Vector2Int boardDimensions = Vector2Int.zero;
        float boardYOffset = 0.0f;
       
        // read Board properties (on main thread at the moment though - needs dependencies to run before board, as that gets destroyed.)
        // also, this is pointless as a loop - the code doesn't support multiple boards.
        Entities.ForEach((Entity e, in BoardSpawner boardSettings) =>
        {
            boardYOffset = boardSettings.RandomYOffset; // ensure agents aren't z-fighting with the board. (might need some terrain-following stuff instead if we want more fidelity)
            boardDimensions = new Vector2Int(boardSettings.SizeX, boardSettings.SizeZ);
        }).Run();
        
        Entities
            .WithStructuralChanges() // we will destroy ourselves at the end of the loop
            .ForEach((Entity bucketSpawnSettings, in BucketSpawner spawner, in Translation t) =>
        {
            NonUniformScale prefabScale = EntityManager.GetComponentData<NonUniformScale>(spawner.BucketPrefab);
            float yOffset = (prefabScale.Value.y * 0.5f) + boardYOffset;

            // reserve a list of entities to clone prefab data into.
            NativeArray<Entity> clonedBuckets = new NativeArray<Entity>(spawner.MaxBuckets, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            // clone agents
            EntityManager.Instantiate(spawner.BucketPrefab, clonedBuckets);
            
            for (int i = 0; i < spawner.MaxBuckets; ++i)
            {
                Entity bucket = clonedBuckets[i];
                EntityManager.AddComponent<Bucket>(bucket);
                EntityManager.AddComponentData<CarryableObject>(bucket, new CarryableObject{ CarryingEntity = Entity.Null });
                // bucket prefab should already have an Intensity component to store current volume
                if (!EntityManager.HasComponent<Intensity>(bucket))
                {
                    EntityManager.AddComponentData<Intensity>(bucket, new Intensity { Value = 0.0f }); // start empty.
                }
                EntityManager.SetComponentData<Translation>(bucket, new Translation{ Value = new float3(Random.Range(0, boardDimensions.x), yOffset, Random.Range(0, boardDimensions.y))} );
            }
            
            EntityManager.DestroyEntity(bucketSpawnSettings);
        }).Run();
    }
}
