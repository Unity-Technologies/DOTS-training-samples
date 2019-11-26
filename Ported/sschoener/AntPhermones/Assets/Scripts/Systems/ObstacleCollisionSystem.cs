using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(UpdatePositionSystem))]
public class ObstacleCollisionSystem : JobComponentSystem
{
    EntityQuery m_MapQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_MapQuery = GetEntityQuery(ComponentType.ReadOnly<MapSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var map = m_MapQuery.GetSingleton<MapSettingsComponent>();
        var obstacleRadius = map.ObstacleRadius;
        var obstacles = map.Obstacles;
        return Entities.ForEach((ref PositionComponent position, ref VelocityComponent velocity) =>
        {
            obstacles.Value.TryGetObstacles(position.Value, out int offset, out int length);
            int end = offset + length;
            for (int j = offset; j < end; j++)
            {
                var obstaclePosition = obstacles.Value.Obstacles[j];
                var delta = position.Value - obstaclePosition;
                float sqrDist = math.lengthsq(delta);
                if (sqrDist < obstacleRadius * obstacleRadius)
                {
                    delta /= math.sqrt(sqrDist);
                    position.Value = obstaclePosition + delta * obstacleRadius;
                    velocity.Value -= 1.5f * delta * math.dot(delta, velocity.Value);
                }
            }
        }).Schedule(inputDeps);
    }
}
