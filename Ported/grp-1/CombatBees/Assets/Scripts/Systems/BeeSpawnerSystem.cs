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

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, in BeeSpawner spawner, in Translation pos) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                float xPos = pos.Value.x;


                // spawn bees
                for (int i = 0; i < spawner.numBeesToSpawn; ++i)
                {
                    
                    var bee = ecb.Instantiate(spawner.beePrefab);
                    var translation = new Translation { Value = new float3(xPos, i*2, 0) };
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
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
        beeBaseTranslation.Dispose();
        beeBaseEntity.Dispose();
    }
}
