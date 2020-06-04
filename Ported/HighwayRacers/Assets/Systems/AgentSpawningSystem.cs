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
        public int Lane;
        public float Percent;
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
        
        // can't use Burst if we set a shared component (even using ecb)
        // (not a big loss for spwaning on init)
        Entities.WithoutBurst().ForEach((Entity e, in AgentSpawner spawner) =>
        {
            var bufferEntity = ecb.CreateEntity();
            var buffer = ecb.AddBuffer<SpawnPosition>(bufferEntity);

            for (int i = 0; i < roadInfo.MaxLanes; i++)
            {
                for (float j = 0; j < 1f; j += roadInfo.CarSpawningDistancePercent)
                {
                    buffer.Add(new SpawnPosition
                    {
                        Lane = i,
                        Percent = j
                    });
                }
            }

            for (int i = 0; i < spawner.NumAgents; i++)
            {
                var spawnedEntity = ecb.Instantiate(spawner.Prefab);

                if (buffer.Length == 0)
                    break;

                int randomIndex = random.NextInt(buffer.Length - 1);
                var pos = buffer[randomIndex];
                buffer.RemoveAt(randomIndex);

                LaneAssignment laneAssignment = new LaneAssignment()
                {
                    Value = pos.Lane
                };

                TargetSpeed targetSpeed = new TargetSpeed()
                {
                    Value = random.NextFloat(spawner.MinSpeed, spawner.MaxSpeed)
                };

                PercentComplete percentComplete = new PercentComplete()
                {
                    Value = pos.Percent
                };

                OvertakeSpeedIncrement osi = new OvertakeSpeedIncrement()
                {
                    Value = spawner.OvertakeIncrement
                };

                ecb.SetComponent(spawnedEntity, osi);
                ecb.SetComponent(spawnedEntity, percentComplete);
                ecb.SetComponent(spawnedEntity, targetSpeed);
                ecb.AddComponent(spawnedEntity, laneAssignment);
                //ecb.SetComponent(spawnedEntity, translation);
            }

            ecb.RemoveComponent<AgentSpawner>(e);
        }).Run();

        ecb.Playback(EntityManager);
        m_Random = random;
    }
}
