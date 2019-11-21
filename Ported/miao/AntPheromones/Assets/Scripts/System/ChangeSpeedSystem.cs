using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(CalculatePheromoneSteeringSystem))]
    [UpdateAfter(typeof(CalculateWallSteeringSystem))]
    public class ChangeSpeedSystem : JobComponentSystem
    {
        private SteeringMovementComponent _steeringMovement;

        protected override void OnCreate()
        {
            base.OnCreate();
            this._steeringMovement = 
                GetEntityQuery(ComponentType.ReadOnly<SteeringMovementComponent>()).GetSingleton<SteeringMovementComponent>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job {
                MaxSpeed = this._steeringMovement.MaxSpeed,
                Acceleration = this._steeringMovement.Acceleration
            }.Schedule(this, inputDeps);
        }
        
        private struct Job : IJobForEach<PheromoneSteeringComponent, WallSteeringComponent, SpeedComponent>
        {
            public float MaxSpeed;
            public float Acceleration;
            
            public void Execute([ReadOnly] ref PheromoneSteeringComponent pheromone, [ReadOnly] ref WallSteeringComponent wall, ref SpeedComponent speedComponent)
            {
                float targetSpeed = this.MaxSpeed;
                targetSpeed *= 1 - (math.abs(pheromone.Value) + math.abs(wall.Value)) / 3;
                
                speedComponent.Value = (targetSpeed - speedComponent.Value) * this.Acceleration;
            }
        }
    }
}