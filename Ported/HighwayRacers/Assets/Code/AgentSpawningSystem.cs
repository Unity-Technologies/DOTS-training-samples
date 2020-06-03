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

    public struct SpawnPosition : IBufferElementData
    {
        public float2 Position;
    }

    protected override void OnCreate()
    {
        m_Random = new Random(0x1234567);
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(RoadInfo)));
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        var random = m_Random;
        var roadInfo = GetSingleton<RoadInfo>(); 
        
        float roadLength = math.abs(roadInfo.EndXZ.y - roadInfo.StartXZ.y);
        float validYDistance = roadInfo.CarSpawningDistancePercent * roadLength;

        // can't use Burst if we set a shared component (even using ecb)
        // (not a big loss for spwaning on init)
        Entities.WithoutBurst().ForEach((Entity e, in AgentSpawner spawner) =>
        {
            var bufferEntity = ecb.CreateEntity();
            var buffer = ecb.AddBuffer<SpawnPosition>(bufferEntity);

            for (float x = roadInfo.StartXZ.x; x < roadInfo.EndXZ.x; x += roadInfo.LaneWidth)
            {
                for (float y = roadInfo.StartXZ.y; y < roadInfo.EndXZ.y; y += validYDistance)
                {
                    buffer.Add(new SpawnPosition()
                    {
                        Position = new float2(x, y)
                    });
                }
            }

            for (int i = 0; i < spawner.NumAgents; i++)
            {
                var spawnedEntity = ecb.Instantiate(spawner.Prefab);

                int randomIndex = random.NextInt(buffer.Length - 1);
                var pos = buffer[randomIndex];
                buffer.RemoveAt(randomIndex);

                Translation translation = new Translation()
                {
                    Value = new float3(pos.Position.x, 0f, pos.Position.y)
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
                ecb.AddComponent(spawnedEntity, laneAssignment);
                ecb.SetComponent(spawnedEntity, translation);
            }

            ecb.RemoveComponent<AgentSpawner>(e);
        }).Run();

        ecb.Playback(EntityManager);
        m_Random = random;
    }
}
