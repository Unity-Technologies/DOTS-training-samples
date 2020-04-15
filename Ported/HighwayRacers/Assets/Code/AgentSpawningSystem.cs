using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[RequiresEntityConversion]
public class AgentSpawningSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities.ForEach((Entity e, in AgentSpawner spawner) =>
        {
            for (int i = 0; i < spawner.NumAgents; i++)
            {
                var spawnedEntity = ecb.Instantiate(spawner.Prefab);

                //Here we could set the agents to specific values along the track.
                Translation translation = new Translation()
                {
                    Value = new float3(0f, 0f, 0f)
                };

                ecb.SetComponent(spawnedEntity, translation);
            }

            ecb.RemoveComponent<AgentSpawner>(e);
        }).Run();

        ecb.Playback(EntityManager);
    }
}
