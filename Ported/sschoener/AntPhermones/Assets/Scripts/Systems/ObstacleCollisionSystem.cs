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
        return new Job {
            ObstacleRadius = map.ObstacleRadius,
            Obstacles = map.Obstacles
        }.Schedule(this, inputDeps);
    }
    
    //[BurstCompile]
    struct Job : IJobForEach<PositionComponent, VelocityComponent>
    {
        public float ObstacleRadius;
        public BlobAssetReference<ObstacleData> Obstacles;
        
        public void Execute(ref PositionComponent position, ref VelocityComponent velocity)
        {
            Obstacles.Value.TryGetObstacles(position.Value, out int offset, out int length);
            int end = offset + length;
            for (int j = offset; j < end; j++)
            {
                var obstaclePosition = Obstacles.Value.Obstacles[j];
                var delta = position.Value - obstaclePosition;
                float sqrDist = math.lengthsq(delta);
                if (sqrDist < ObstacleRadius * ObstacleRadius)
                {
                    Debug.Log("Collision!");
                    delta /= math.sqrt(sqrDist);
                    position.Value = obstaclePosition + delta * ObstacleRadius;
                    velocity.Value -= 1.5f * delta * math.dot(delta, velocity.Value);
                }
            }
        }
    }
}
