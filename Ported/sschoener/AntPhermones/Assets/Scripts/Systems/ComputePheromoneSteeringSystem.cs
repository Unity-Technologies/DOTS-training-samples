using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(PerturbFacingSystem))]
public class ComputePheromoneSteeringSystem : JobComponentSystem
{
    EntityQuery m_PheromoneMapQuery;
    EntityQuery m_MapSettingsQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_PheromoneMapQuery = GetEntityQuery(ComponentType.ReadOnly<PheromoneBuffer>());
        m_MapSettingsQuery = GetEntityQuery(ComponentType.ReadOnly<MapSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var pheromoneMap = GetBufferFromEntity<PheromoneBuffer>(true)[m_PheromoneMapQuery.GetSingletonEntity()];
        return new SteeringJob
        {
            PheromoneMap = pheromoneMap,
            MapSize = m_MapSettingsQuery.GetSingleton<MapSettingsComponent>().MapSize
        }.Schedule(this, inputDeps);
    }

    [BurstCompile]
    [ExcludeComponent(typeof(PheromoneBuffer))]
    struct SteeringJob : IJobForEach<FacingAngleComponent, PositionComponent, PheromoneSteeringComponent>
    {
        public int MapSize;
        [ReadOnly]
        public DynamicBuffer<PheromoneBuffer> PheromoneMap;

        public void Execute([ReadOnly] ref FacingAngleComponent facingAngle, [ReadOnly] ref PositionComponent position, ref PheromoneSteeringComponent steering)
        {
            const float distance = 3;
            float output = 0;
            for (int i = -1; i <= 1; i += 2)
            {
                float angle = facingAngle.Value + i * math.PI * .25f;
                math.sincos(angle, out var sin, out var cos);
                
                var test = (int2) (position.Value + distance * new float2(cos, sin));
                if (math.all(test > 0) && math.all(test < MapSize))
                {
                    int index = test.y * MapSize + test.x;
                    output += PheromoneMap[index] * i;
                }
            }
            
            // Mathf.Sign has a weird edge case:
            steering.Value = output >= 0 ? 1 : -1;
        }
    }
}
