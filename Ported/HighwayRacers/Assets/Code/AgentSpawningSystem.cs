using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class AgentSpawningSystem : SystemBase
{
    private Random m_Random;
    private RoadInfo m_RoadInfo;

    protected override void OnCreate()
    {
        m_Random = new Random(0x1234567);
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(RoadInfo)));
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        var random = m_Random;
        var roadInfo = GetSingleton<RoadInfo>();//m_LaneInfo;
        Entities.ForEach((Entity e, in AgentSpawner spawner) =>
        {
            for (int i = 0; i < spawner.NumAgents; i++)
            {
                var spawnedEntity = ecb.Instantiate(spawner.Prefab);
                //Need to resolve situation where cars are embeded in each other.
                //Precalculate all the slots that cars can go in and assign those slots here.
                
                Translation translation = new Translation()
                {
                    Value = new float3((int)random.NextFloat(roadInfo.StartXZ.x, roadInfo.EndXZ.x), 0f,
                        random.NextFloat(roadInfo.StartXZ.y, roadInfo.EndXZ.y))
                };

                LaneAssignment laneAssignment = new LaneAssignment()
                {
                    Value = random.NextInt(0, roadInfo.MaxLanes)
                };

                TargetSpeed targetSpeed = new TargetSpeed()
                {
                    Value = random.NextFloat(0.1f, 0.5f)
                };

                ecb.SetComponent(spawnedEntity, targetSpeed);
                ecb.SetComponent(spawnedEntity, laneAssignment);
                ecb.SetComponent(spawnedEntity, translation);
            }

            ecb.RemoveComponent<AgentSpawner>(e);
        }).Run();

        ecb.Playback(EntityManager);
        m_Random = random;
    }
}
