using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

// TODO : add reinitialization system using keyboard controls

public static class Const {
    public const float SpeedSlow = 10f;
    public const float SpeedFast = 25f;
    // TODO: Having trouble adding the bucket colors as a static variable
}

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class SpawnerSystem : SystemBase
{
    protected override void OnCreate()
    {
        // Wait for the specified instanciations
        RequireSingletonForUpdate<Spawner>();
    }

    protected override void OnUpdate()
    {
        var random = new Unity.Mathematics.Random(1234);

        var spawner = GetSingleton<Spawner>();

        var cellInstance = EntityManager.Instantiate(spawner.CellPrefab, spawner.GridSize*spawner.GridSize, Allocator.Temp);
        for (int i = 0; i < spawner.GridSize; ++i)
        {
            for (int j = 0; j < spawner.GridSize; ++j)
            {
                var t = new Translation { Value = new float3(i, 0.0f, j)};
                var cell = cellInstance[i+j*spawner.GridSize];
                EntityManager.SetComponentData(cell, t);

                EntityManager.AddComponentData(cell, new Firelocation());

                EntityManager.AddComponentData(cell, new URPMaterialPropertyBaseColor() { Value = new float4(0, 0.5f, 0, 1.0f) });
                //EntityManager.SetName(cellInstance[i+j*spawner.GridSize], $"GrassFieldCell-{i}_{j}");
            }
        }

        // instantiate fire
        EntityManager.CreateEntity(typeof(Heat));
        var heatSingleton = EntityManager.GetBuffer<Heat>(GetSingletonEntity<Heat>());
        heatSingleton.Length = spawner.GridSize*spawner.GridSize;

        // Initializes Heat map for the grass field
        for (int i = 0; i < heatSingleton.Length; i++)
        {
            heatSingleton[i] = Heat.NonBurningHeat;
            //EntityManager.SetName(heatSingleton[i], $"CellFire-{i}");
        }

        // On start, a few random cells are on fire. Buckets are randomly placed around the field.
        var maxGridIndex = spawner.GridSize * spawner.GridSize - 1;
        for (int iInitialFire = 0; iInitialFire < spawner.StartingFireCount; iInitialFire++)
        {
            var initialFireInstanceIndex = random.NextInt(0, maxGridIndex);
            heatSingleton[initialFireInstanceIndex] = Heat.InitialFireSpotHeat;
        }

        // instantiate water patch
        var waterPatchInstance = EntityManager.Instantiate(spawner.WaterPatchPrefab,
            spawner.GridSize * 4, Allocator.Temp);
        for (int i = 0; i < spawner.GridSize; ++i)
        {
            EntityManager.SetComponentData(waterPatchInstance[i], new Translation
            {
                Value = new float3(i, 0.0f, random.NextInt(-5, -2))
            });
            EntityManager.AddComponent<WaterPatch>(waterPatchInstance[i]);


            EntityManager.SetComponentData(waterPatchInstance[spawner.GridSize + i], new Translation
            {
                Value = new float3(i, 0.0f, random.NextInt(spawner.GridSize + 2, spawner.GridSize + 5))
            });
            EntityManager.AddComponent<WaterPatch>(waterPatchInstance[spawner.GridSize + i]);


            EntityManager.SetComponentData(waterPatchInstance[spawner.GridSize * 2 + i], new Translation
            {
                Value = new float3(random.NextInt(-5, -2), 0.0f, i)
            });
            EntityManager.AddComponent<WaterPatch>(waterPatchInstance[spawner.GridSize * 2 + i]);


            EntityManager.SetComponentData(waterPatchInstance[spawner.GridSize * 3 + i], new Translation
            {
                Value = new float3(random.NextInt(spawner.GridSize + 2, spawner.GridSize + 5), 0.0f, i)
            });
            EntityManager.AddComponent<WaterPatch>(waterPatchInstance[spawner.GridSize * 3 + i]);
        }

        // create team entity
        var teamInstance = EntityManager.CreateEntity(EntityManager.CreateArchetype(typeof(Team)), spawner.TeamCount, Allocator.Temp);
        for (int i = 0; i < spawner.TeamCount; ++i)
        {
            float3 targetWaterPosition;

            // Target water is instantiated at random at the start
            targetWaterPosition = EntityManager.GetComponentData<Translation>(waterPatchInstance[random.NextInt(0, waterPatchInstance.Length)]).Value;
            targetWaterPosition.y = 1;

            // empty bucket workers
            var EmptyBucketWorkerInstance = EntityManager.Instantiate(spawner.EmptyBucketWorkerPrefab, spawner.EmptyBucketWorkerPerTeamCount, Allocator.Temp);
            for (int j = 0; j < spawner.EmptyBucketWorkerPerTeamCount; ++j)
            {
                EntityManager.SetComponentData(EmptyBucketWorkerInstance[j], new Translation
                {
                    Value = new float3(random.NextFloat(0, spawner.GridSize), 1, random.NextFloat(0, spawner.GridSize))
                });
                EntityManager.AddComponent<HeldBucket>(EmptyBucketWorkerInstance[j]);
                EntityManager.AddComponent<TargetOriginalPosition>(EmptyBucketWorkerInstance[j]);
                EntityManager.AddComponent<TargetPosition>(EmptyBucketWorkerInstance[j]);
                EntityManager.AddComponent<EmptyBucketWorker>(EmptyBucketWorkerInstance[j]);

                if (j > 0)
                {
                    EntityManager.SetComponentData<EmptyBucketWorker>(EmptyBucketWorkerInstance[j], new EmptyBucketWorker
                    {
                        nextWorker = EmptyBucketWorkerInstance[j-1]
                    });
                }
                else
                {
                    EntityManager.AddComponentData<LastWorker>(EmptyBucketWorkerInstance[j], new LastWorker{ targetPosition = targetWaterPosition });
                }
            }


            // full buckets workers
            var FullBucketWorkerInstance = EntityManager.Instantiate(spawner.FullBucketWorkerPrefab, spawner.FullBucketWorkerPerTeamCount, Allocator.Temp);
            for (int j = 0; j < spawner.FullBucketWorkerPerTeamCount; ++j)
            {
                EntityManager.SetComponentData(FullBucketWorkerInstance[j], new Translation
                {
                    Value = new float3(random.NextFloat(0, spawner.GridSize), 1, random.NextFloat(0, spawner.GridSize))
                });
                EntityManager.AddComponent<HeldBucket>(FullBucketWorkerInstance[j]);
                EntityManager.AddComponent<TargetOriginalPosition>(FullBucketWorkerInstance[j]);
                EntityManager.AddComponent<TargetPosition>(FullBucketWorkerInstance[j]);
                EntityManager.AddComponent<FullBucketWorker>(FullBucketWorkerInstance[j]);

                if (j > 0)
                {
                    EntityManager.SetComponentData<FullBucketWorker>(FullBucketWorkerInstance[j], new FullBucketWorker
                    {
                        nextWorker = FullBucketWorkerInstance[j-1]
                    });
                }
                else
                {
                    EntityManager.AddComponent<LastWorker>(FullBucketWorkerInstance[j]);
                }
            }

            // Water fetcher worker - only 1 instance per team
            var waterFetcherInstance = EntityManager.Instantiate(spawner.WaterFetcherWorkerPrefab);
            EntityManager.SetComponentData(waterFetcherInstance, new Translation
            {
                Value = new float3(random.NextFloat(0, spawner.GridSize), 1, random.NextFloat(0, spawner.GridSize))
            });
            EntityManager.AddComponent<HeldBucket>(waterFetcherInstance);
            EntityManager.AddComponentData<TargetOriginalPosition>(waterFetcherInstance, new TargetOriginalPosition { Value = targetWaterPosition });
            EntityManager.AddComponent<TargetPosition>(waterFetcherInstance);
            EntityManager.AddComponent<BucketFetcher>(waterFetcherInstance);

            EntityManager.SetComponentData<Team>(teamInstance[i], new Team
            {
                targetFire = Entity.Null,
                targetFirePosition = new int2(-1, -1), // Target fire is invalidated at start (will be set in PickTeamTargetFireSystem)
                targetWaterPosition = targetWaterPosition, 
                emptyBucketWorker = spawner.EmptyBucketWorkerPerTeamCount > 0 ? EmptyBucketWorkerInstance[spawner.EmptyBucketWorkerPerTeamCount - 1] : Entity.Null,
                fullBucketWorker = spawner.FullBucketWorkerPerTeamCount > 0 ? FullBucketWorkerInstance[spawner.FullBucketWorkerPerTeamCount - 1] : Entity.Null,
                bucketFetcherWorker = waterFetcherInstance
            });
        }

        for (int i = 0; i < spawner.OmniworkerCount; ++i)
        {
            // Omni fetcher worker - only 1 instance per team
            var omniWorkerInstance = EntityManager.Instantiate(spawner.OmniWorkerWorkerPrefab);
            EntityManager.SetComponentData(omniWorkerInstance, new Translation
            {
                Value = new float3(random.NextFloat(0, spawner.GridSize), 1, random.NextFloat(0, spawner.GridSize))
            });
            EntityManager.AddComponent<HeldBucket>(omniWorkerInstance);
            EntityManager.AddComponent<TargetPosition>(omniWorkerInstance);
            EntityManager.AddComponent<OmniWorker>(omniWorkerInstance);
        }

        // instantiate buckets
        var bucketInstance = EntityManager.Instantiate(spawner.BucketPrefab,
            spawner.BucketCount, Allocator.Temp);
        for (int j = 0; j < spawner.BucketCount; ++j)
        {
            EntityManager.SetComponentData(bucketInstance[j], new Translation
            {
                Value = new float3(random.NextFloat(0, spawner.GridSize), 0.25f, random.NextFloat(0, spawner.GridSize))
            });
            EntityManager.AddComponent<Bucket>(bucketInstance[j]);
            EntityManager.AddComponentData(bucketInstance[j], new URPMaterialPropertyBaseColor() { Value = new float4(0.2f, 0, 0.6f, 1.0f) });

        }

        

        Enabled = false;
    }
}