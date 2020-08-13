using System;
using System.Drawing;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(FireIntializationSystem))]
public class BucketInitializationSystem : SystemBase
{    
    private EntityCommandBufferSystem _commandBufferSystem;
    static private BucketInitializationSystem _instance;
    
    protected override void OnCreate()
    {
        Debug.Log("*** OnCreate BucketInitializationSystem");
        _commandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        
        RequireSingletonForUpdate<BucketSpawning>();
        RequireSingletonForUpdate<FireConfiguration>();
        RequireSingletonForUpdate<WaterInitialization>();

        _instance = this;
    }
    
    protected override void OnUpdate()
    {
        Debug.Log("*** OnUpdate BucketInitializationSystem");

        var random = new Random(1);

        var waterConfigEntity = GetSingletonEntity<WaterInitialization>();
        var waterConfig = EntityManager.GetComponentData<WaterInitialization>(waterConfigEntity);
        var fireConfigEntity = GetSingletonEntity<FireConfiguration>();
        var fireConfig = EntityManager.GetComponentData<FireConfiguration>(fireConfigEntity);
        
        var ecb = _commandBufferSystem.CreateCommandBuffer();
        Entities
            .WithName("BucketInitializationSystem_BucketPlacement")
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity configEntity, 
                //in FireConfiguration config, 
                //in FireInitialization config, 
                //in WaterInitialization waterConfig, 
                in BucketSpawning bucketSpawning) =>
            { 
                Debug.Log("*** - on for each");
                var size = fireConfig.GridHeight * fireConfig.GridHeight;
                var numBuckets = waterConfig.totalBuckets;
 
                var bucketEntries = new NativeArray<Entity>(numBuckets, Allocator.TempJob);

                EntityManager.Instantiate(bucketSpawning.Prefab, bucketEntries);

                // place buckets around grid
                // for that you need the grid size, and cell size
                var cellSize = fireConfig.CellSize;
                for (int i = 0; i < numBuckets; ++i)
                {
                    var instance = bucketEntries[i];
                    //var randomX = random.NextInt(0, (fireConfig.GridWidth - 1) / 2);
                    //var randomZ = random.NextInt(0, (fireConfig.GridHeight -1) / 2);;
                    var randomX = random.NextInt(-(fireConfig.GridWidth - 1) / 2, (fireConfig.GridWidth - 1) / 2);
                    var randomZ = random.NextInt(-(fireConfig.GridHeight -1) / 2, (fireConfig.GridHeight -1) / 2);;
                    var translation = new float3(randomX, 0, randomZ);
                    translation *= cellSize;
                    translation += fireConfig.Origin;

                    // Position
                    ecb.SetComponent(instance, new Translation { Value = translation });
                    ecb.AddComponent(instance, new NonUniformScale { Value = new float3(0.2f, waterConfig.bucketSize_FULL, 0.2f) });

                    bucketEntries[i] = instance;
                }

                ecb.RemoveComponent<BucketSpawning>(configEntity);
                    
                bucketEntries.Dispose(Dependency);

            }).Run();
      
        _commandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
