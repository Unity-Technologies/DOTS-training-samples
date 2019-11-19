using System;
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

    struct SteeringJob : IJobForEach<FacingAngleComponent, PositionComponent, PheromoneSteeringComponent>
    {
        public float MapSize;
        public DynamicBuffer<PheromoneBuffer> PheromoneMap;

        public void Execute([ReadOnly] ref FacingAngleComponent facingAngle, [ReadOnly] ref PositionComponent position, ref PheromoneSteeringComponent steering)
        {
            const float distance = 3;
            float output = 0;
            for (int i = -1; i <= 1; i += 2)
            {
                float angle = facingAngle.Value + i * math.PI * .25f;
                math.sincos(angle, out var sin, out var cos);
                float testX = position.Value.x + cos * distance;
                float testY = position.Value.y + sin * distance;

                if (testX < 0 || testY < 0 || testX >= MapSize || testY >= MapSize)
                {

                }
                else
                {
                    int index = (int)(testY * MapSize + testX);
                    output += PheromoneMap[index] * i;
                }
            }
            steering.Value = output;
        }
    }
}
