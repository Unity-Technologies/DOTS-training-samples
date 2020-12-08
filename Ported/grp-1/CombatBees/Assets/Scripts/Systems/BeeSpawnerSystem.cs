using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

public class BeeSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        NativeList<Translation> beeBaseTranslation = new NativeList<Translation>(3, Allocator.TempJob);
        NativeList<Entity> beeBaseEntity = new NativeList<Entity>(3, Allocator.TempJob);
        beeBaseTranslation.Add(new Translation());
        beeBaseTranslation.Add(new Translation());
        beeBaseTranslation.Add(new Translation());
        beeBaseEntity.Add(new Entity());
        beeBaseEntity.Add(new Entity());
        beeBaseEntity.Add(new Entity());
        var ecb0 = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .ForEach((Entity bbaseEntity, in BeeBase bee, in Translation translation ) =>
            {
                if (bee.teamID == 1)
                {
                    beeBaseTranslation[0] = translation;
                    beeBaseEntity[0] = bbaseEntity;
                }
                else if (bee.teamID == 2)
                {
                    beeBaseTranslation[1] = translation;
                    beeBaseEntity[1] = bbaseEntity;
                }
                else
                {
                    beeBaseTranslation[2] = translation;
                    beeBaseEntity[2] = bbaseEntity;
                }
            }).Run();
        ecb0.Playback(EntityManager);
        ecb0.Dispose();

        EntityCommandBufferSystem sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = sys.CreateCommandBuffer();
        var random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
        Entities
            .ForEach((Entity entity, ref BeeSpawnRequest req, in BeeSpawner spawner, in Translation pos) =>
            {
                // spawn bees
                for (int i = 0; i < req.numOfBeesToSpawn; ++i)
                {
                    
                    var bee = ecb.Instantiate(spawner.beePrefab);

                    // give the bee a position near the spawn point
                    float3 randomDistance = random.NextFloat3(-spawner.radius, spawner.radius);
                    var translation = new Translation { Value = (pos.Value + randomDistance) };
                    ecb.SetComponent(bee, translation);

                    //give the bee its team
                    ecb.SetComponent(bee, new URPMaterialPropertyBaseColor
                    {
                        Value = spawner.teamColour
                    });

                    ecb.SetComponent(bee, new Bee
                    {
                        teamID = spawner.teamNumber,
                        currentTargetEntity = Entity.Null,
                        currentTargetTransform = new Translation(),
                        baseTargetTransform = beeBaseTranslation[spawner.teamNumber - 1],
                        CenterTargetTransform = beeBaseTranslation[2],
                        baseEntity = beeBaseEntity[spawner.teamNumber - 1],
                        centerEntity = beeBaseEntity[2],


                    });

                    if (1 == spawner.teamNumber)
                    {
                        ecb.AddComponent<BeeTeam1>(bee, new BeeTeam1());
                    }
                    else
                    {
                        ecb.AddComponent<BeeTeam2>(bee, new BeeTeam2());
                    }

        
                }

                ecb.RemoveComponent<BeeSpawnRequest>(entity);
            }).Run();

        sys.AddJobHandleForProducer(Dependency);
        beeBaseTranslation.Dispose();
        beeBaseEntity.Dispose();
    }
}
