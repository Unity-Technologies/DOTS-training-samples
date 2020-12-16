using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class WaterSpawnerSystem : SystemBase
{

    EntityCommandBufferSystem mEntityCommmandBufferSystem;

    protected override void OnCreate()
    {
        mEntityCommmandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = mEntityCommmandBufferSystem.CreateCommandBuffer();

        //float2 dim = new float2(FireSimConfig.xDim, FireSimConfig.yDim) - 1;
        int xDim = FireSimConfig.xDim;
        int yDim = FireSimConfig.yDim;

        int nWaterSources = WaterConfig.nWaterSources;
        Random random = new Random(1234); // (uint)Time.DeltaTime + 1);

        Entities.ForEach((Entity entity, in WaterSpawner waterSpawner) =>
        {
            ecb.DestroyEntity(entity);

            for (int i=0; i < nWaterSources; ++i)
            {
                float2 waterCoord; // = random.NextFloat2(dim);
                Entity waterEntity = ecb.Instantiate(waterSpawner.Prefab);

                // water sources will appear outside the grid, so we first need to determine the dimensions of the grid so we know the edges
                // water edge will be adjacent, but in a random location. 
                // Get a side for the water to be on - random(4)
                // Get the adjacent X/Y based on side
                // Random the other X/Y based on dim length
                int nSide = random.NextInt(4);
                if (nSide > 1) //nSide = 2/3 = top/bottom
                {
                    waterCoord.x = random.NextInt(xDim);
                    waterCoord.y = (nSide == 2) ? -5.5f : yDim + 4.5f;

                }
                else //nSide = 0/1 = left/right
                {
                    waterCoord.x = (nSide == 0) ? -5.5f : xDim + 4.5f;
                    waterCoord.y = random.NextInt(yDim);
                }

                ecb.AddComponent<Position>(waterEntity, new Position { coord = waterCoord });

                var newTranslation = new Translation { Value = new float3(waterCoord.x, -1.0f, waterCoord.y) };
                ecb.SetComponent(waterEntity, newTranslation);
            }

        }).Schedule();

        mEntityCommmandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
