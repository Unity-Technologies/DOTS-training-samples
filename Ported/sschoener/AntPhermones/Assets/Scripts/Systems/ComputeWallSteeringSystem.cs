using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(PerturbFacingSystem))]
public class ComputeWallSteeringSystem : JobComponentSystem
{
    EntityQuery m_MapSettingsQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_MapSettingsQuery = GetEntityQuery(ComponentType.ReadOnly<MapSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return default;
        var map = m_MapSettingsQuery.GetSingleton<MapSettingsComponent>();
        return new SteeringJob
        {
            Obstacles = map.Obstacles,
            MapSize = map.MapSize
        }.Schedule(this, inputDeps);
    }

    [BurstCompile]
    struct SteeringJob : IJobForEach<PositionComponent, FacingAngleComponent, WallSteeringComponent>
    {
        public float MapSize;
        public BlobAssetReference<ObstacleData> Obstacles;

        public void Execute([ReadOnly] ref PositionComponent position, [ReadOnly] ref FacingAngleComponent facingAngle, ref WallSteeringComponent steering)
        {
            const float distance = 1.5f;
            float output = 0;
            for (int i = -1; i <= 1; i += 2)
            {
                float angle = facingAngle.Value + i * math.PI * .25f;
                math.sincos(angle, out var sin, out var cos);
                float2 test = position.Value + distance * new float2(cos, sin);
                if (math.any(test < 0) || math.any(test >= MapSize))
                {

                }
                else if (Obstacles.Value.HasObstacle(test))
                {
                    output -= i;
                }
            }
            steering.Value = output;
        }
    }
}
