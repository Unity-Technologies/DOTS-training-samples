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
        var map = m_MapSettingsQuery.GetSingleton<MapSettingsComponent>();
        return new SteeringJob
        {
            Obstacles = map.Obstacles.Value.Obstacles,
            MapSize = map.MapSize,
        }.Schedule(this, inputDeps);
    }

    struct SteeringJob : IJobForEach<PositionComponent, FacingAngleComponent, WallSteeringComponent>
    {
        public float MapSize;
        public BlobArray<float2> Obstacles;

        public void Execute([ReadOnly] ref PositionComponent position, [ReadOnly] ref FacingAngleComponent facingAngle, ref WallSteeringComponent steering)
        {
            const float distance = 1.5f;
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
                    int value = GetObstacleBucket(testX, testY).Length;
                    if (value > 0)
                    {
                        output -= i;
                    }
                }
            }
            steering.Value = output;
        }
    }
}
