using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(CalculatePheromoneSteeringSystem))]
    [UpdateAfter(typeof(CalculateWallSteeringSystem))]
    public class ChangeSpeedAccordingToSteeringStrengthSystem : JobComponentSystem
    {
        private EntityQuery _steeringMovementQuery;
        private (bool IsRetrieved, float MaxSpeed, float Acceleration) _steeringMovement;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            this._steeringMovementQuery =
                GetEntityQuery(ComponentType.ReadOnly<SteeringMovement>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!this._steeringMovement.IsRetrieved)
            {
                var steeringMovement = this._steeringMovementQuery.GetSingleton<SteeringMovement>();
                this._steeringMovement =
                    (IsRetrieved: true,
                    MaxSpeed: steeringMovement.MaxSpeed,
                    Acceleration: steeringMovement.Acceleration);
            }
            
            return new Job {
                MaxSpeed = this._steeringMovement.MaxSpeed,
                Acceleration = this._steeringMovement.Acceleration
            }.Schedule(this, inputDeps);
        }
        
        [BurstCompile]
        private struct Job : IJobForEach<PheromoneSteering, WallSteering, Speed>
        {
            public float MaxSpeed;
            public float Acceleration;
            
            public void Execute(
                [ReadOnly] ref PheromoneSteering pheromone,
                [ReadOnly] ref WallSteering wall, 
                ref Speed speed)
            {
                speed.Value += (this.MaxSpeed * (1 - (math.abs(pheromone.Value) + math.abs(wall.Value)) / 3) - speed.Value) * this.Acceleration;
            }
        }
    }
}