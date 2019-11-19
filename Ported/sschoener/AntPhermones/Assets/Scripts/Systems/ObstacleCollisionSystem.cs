using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

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
            Obstacles = map.Obstacles.Value.Obstacles
        }.Schedule(this, inputDeps);
    }

    struct Job : IJobForEach<PositionComponent, VelocityComponent>
    {
        public float ObstacleRadius;
        public BlobArray<float2> Obstacles;
        
        public void Execute(ref PositionComponent position, ref VelocityComponent velocity)
        {
            for (int j = 0; j < Obstacles.Length; j++)
            {
                var obstaclePosition = Obstacles[j];
                var delta = position.Value - obstaclePosition;
                float sqrDist = math.lengthsq(delta);
                if (sqrDist < ObstacleRadius * ObstacleRadius)
                {
                    delta /= math.sqrt(sqrDist);
                    position.Value = obstaclePosition + delta * ObstacleRadius;
                    velocity.Value -= 1.5f * delta * math.dot(delta, velocity.Value);
                }
            }
        }
    }
}
