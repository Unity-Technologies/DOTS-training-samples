using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(FireInitSystem))]
public class SpawningInitSystem : SystemBase
{


    protected override void OnUpdate()
    {
        if (!HasSingleton<TuningData>())
            return;
        Random random = new Random(1000);
        Entity tuningDataEntity = GetSingletonEntity<TuningData>();
        TuningData tuningData = EntityManager.GetComponentData<TuningData>(tuningDataEntity);

        int gridDimensionX = (int)(tuningData.GridSize.x * 0.5f * tuningData.FireCellSize);
        int gridDimensionY = (int)(tuningData.GridSize.y * 0.5f * tuningData.FireCellSize);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities.ForEach((Entity entity, in InitDataActors actorTunning) =>
        {

            int actorCount = actorTunning.BrigadeCount;
            for (int brigade = 0; brigade < actorCount; brigade++)
            {
                
                Entity brigadeEntity = ecb.CreateEntity();
                ecb.AddComponent<Brigade>(brigadeEntity);
                var buffer = ecb.AddBuffer<ActorElement>(brigadeEntity);

                Entity previous = Entity.Null;
                Entity filler = Entity.Null;
                for (int actor = 0; actor < actorTunning.ActorCountPerBrigade; actor++)
                {
                    Entity e = ecb.Instantiate(actorTunning.ActorPrefab);
                    int x = random.NextInt(-gridDimensionX, gridDimensionX);
                    int z = random.NextInt(-gridDimensionY, gridDimensionY);
                    Translation position = new Translation()
                    {
                        Value = new float3(x, 1, z)
                    };
                    ecb.SetComponent(e, position);

                    if (actor == 0)
                        ecb.AddComponent<ScooperTag>(e);
                    else if (actor == 1)
                    {
                        ecb.AddComponent<FillerTag>(e);
                        filler = e;
                    }
                    else if (actor == actorTunning.ActorCountPerBrigade / 2)
                        ecb.AddComponent(e, new ThrowerTag() { brigade = brigadeEntity });

                    if (previous != Entity.Null)
                    {
                        ecb.SetComponent(previous, new Actor(){neighbor = e});
                    }
                    previous = e;
                    if(actor == actorTunning.ActorCountPerBrigade-1)
                        ecb.SetComponent(e, new Actor(){neighbor = filler});

                    //some way to add e to brigadeEntity...
                    buffer.Add(new ActorElement(){actor = e});
                }

            }

            ecb.RemoveComponent<InitDataActors>(entity);
        }).Run();

        ecb.Playback(EntityManager);
        ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((Entity entity, in InitBucketData bucketTunning) =>
        {
            for (int i = 0; i < bucketTunning.BucketCount; i++)
            {
                Entity e = ecb.Instantiate(bucketTunning.BucketPrefab);
                int x = random.NextInt(-gridDimensionX, gridDimensionX);
                int z = random.NextInt(-gridDimensionY, gridDimensionY);
                Translation position = new Translation()
                {
                    Value = new float3(x, 1, z)
                };
                ecb.SetComponent(e, position);
            }
            ecb.RemoveComponent<InitBucketData>(entity);
        }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}

