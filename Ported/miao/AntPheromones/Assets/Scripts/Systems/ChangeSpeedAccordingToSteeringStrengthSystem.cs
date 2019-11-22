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
                GetEntityQuery(ComponentType.ReadOnly<SteeringMovementComponent>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!this._steeringMovement.IsRetrieved)
            {
                var steeringMovement = this._steeringMovementQuery.GetSingleton<SteeringMovementComponent>();
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
        private struct Job : IJobForEach<PheromoneSteeringComponent, WallSteeringComponent, SpeedComponent>
        {
            public float MaxSpeed;
            public float Acceleration;
            
            public void Execute(
                [ReadOnly] ref PheromoneSteeringComponent pheromone,
                [ReadOnly] ref WallSteeringComponent wall, 
                ref SpeedComponent speedComponent)
            {
                float targetSpeed = this.MaxSpeed;
                targetSpeed *= 1 - (math.abs(pheromone.Value) + math.abs(wall.Value)) / 3;
                
                speedComponent.Value += (targetSpeed - speedComponent.Value) * this.Acceleration;
            }
        }
    }
}