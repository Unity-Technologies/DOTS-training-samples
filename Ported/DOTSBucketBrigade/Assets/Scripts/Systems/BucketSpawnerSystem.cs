using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class BucketSpawnerSystem : SystemBase
{
    EntityCommandBufferSystem m_EntityCommandBufferSystem;
    
    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
	
    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = m_EntityCommandBufferSystem.CreateCommandBuffer();

        float2 dim = new float2(FireSimConfig.xDim, FireSimConfig.yDim) - 1;
        int nBuckets = BucketConfig.nBuckets;
        Random random = new Random((uint) Time.ElapsedTime + 1);

        Entities.ForEach((Entity entity, in BucketSpawner bucketSpawner) =>
        {
            ecb.DestroyEntity(entity);
			
            for (int i=0; i<nBuckets; ++i)
            {
                float2 bucketCoord = random.NextFloat2(dim);
                Entity bucketEntity = ecb.Instantiate(bucketSpawner.Prefab);
                ecb.AddComponent<Position>(bucketEntity, new Position
                {
                    coord = bucketCoord
                });
                
                // TODO: Reevaluate whether it is necessary to add the translation to position
                // once we have the bucket system in place
                // TODO: adjust the y-value as the prefabs origins/pivot points change (bucket, ground prefabs)
                var newTranslation = new Translation {Value = new float3(bucketCoord.x, 0.5f, bucketCoord.y)};
                ecb.SetComponent(bucketEntity, newTranslation);

            }
        }).Schedule();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }

}