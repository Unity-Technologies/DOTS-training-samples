using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(PerturbFacingSystem))]
public class ComputeWallSteeringSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new System.NotImplementedException();
    }

    struct SteeringJob : IJobForEach<PositionComponent, FacingAngleComponent, WallSteeringComponent>
    {
        public float MapSize;

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
