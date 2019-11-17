using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(PerturbFacingSystem))]
public class ComputePheromoneSteeringSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new SteeringJob
        {
            MapSize = -1,
        }.Schedule(this, inputDeps);
    }

    struct SteeringJob : IJobForEach<FacingAngleComponent, PositionComponent, PheromoneSteeringComponent>
    {
        public float MapSize;

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
                    int index = PheromoneIndex((int)testX, (int)testY);
                    float value = pheromones[index].r;
                    output += value * i;
                }
            }
            steering.Value = output;
        }
    }
}
