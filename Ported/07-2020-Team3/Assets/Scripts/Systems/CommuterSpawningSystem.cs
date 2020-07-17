using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CommuterSpawningSystem : SystemBase
{
    Random m_AngleRandom;
    Random m_RadiusRandom;
    Random m_SpeedRandom;
    EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_AngleRandom = new Random(0x1234567);
        m_RadiusRandom = new Random(0x7654321);
        m_SpeedRandom = new Random(0x1726354);
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer();
        var angleRandom = m_AngleRandom;
        var radiusRandom = m_RadiusRandom;
        var speedRandom = m_SpeedRandom;
        const float twoPi = 2f * math.PI;

        Entities
            .ForEach((Entity spawnerEntity, in CommuterSpawner spawner,
            in Translation spawnerTranslation, in DynamicBuffer<RoutePlatform> routePlatformsBuffer,
            in DynamicBuffer<CommuterWaypoint> waypointsBuffer) =>
        {
            var maxRadius = spawner.SpawnRadius;
            var minSpeed = spawner.MinCommuterSpeed;
            var maxSpeed = spawner.MaxCommuterSpeed;
            var spawnerPosition = spawnerTranslation.Value;
            var firstPlatform = routePlatformsBuffer[0].Value;
            var firstWaypointPosition = GetComponent<Waypoint>(waypointsBuffer[0].Value).WorldPosition;
            for (int i = 0; i < spawner.SpawnCount; ++i)
            {
                var instance = ecb.Instantiate(spawner.CommuterPrefab);
                var angle = angleRandom.NextFloat(0f, twoPi);
                var radius = radiusRandom.NextFloat(0f, maxRadius);
                var xPos = radius * math.sin(angle);
                var zPos = radius * math.cos(angle);
                var instancePosition = new float3(spawnerPosition.x + xPos, spawnerPosition.y, spawnerPosition.z + zPos);
                ecb.SetComponent(instance, new Translation { Value = instancePosition});
                var buffer = ecb.SetBuffer<CommuterWaypoint>(instance);
                buffer.CopyFrom(waypointsBuffer);
                ecb.SetComponent(instance, new Commuter
                {
                    Direction = math.normalize(firstWaypointPosition - instancePosition),
                    NextPlatform = firstPlatform
                });

                var speed = speedRandom.NextFloat(minSpeed, maxSpeed);
                ecb.SetComponent(instance, new CommuterSpeed { Value = speed });
            }
            ecb.RemoveComponent<CommuterSpawner>(spawnerEntity);
        }).Schedule();

        m_AngleRandom = angleRandom;
        m_RadiusRandom = radiusRandom;
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
