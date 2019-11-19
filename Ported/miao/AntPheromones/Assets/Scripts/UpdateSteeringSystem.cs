using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    public class UpdateSteeringSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job {
                MaxSpeed = 0,
                Acceleration = 0
            }.Schedule(this, inputDeps);
        }
        
        private struct Job : IJobForEach<PheromoneSteering, WallSteering, SpeedComponent>
        {
            public float MaxSpeed;
            public float Acceleration;
            
            public void Execute([ReadOnly] ref PheromoneSteering pheromone, [ReadOnly] ref WallSteering wall, ref SpeedComponent speed)
            {
                float targetSpeed = MaxSpeed;
                targetSpeed *= 1 - (math.abs(pheromone.Value) + math.abs(wall.Value)) / 3;
                speed.Value = (targetSpeed - speed.Value) * Acceleration;
            }
        }
    }
}