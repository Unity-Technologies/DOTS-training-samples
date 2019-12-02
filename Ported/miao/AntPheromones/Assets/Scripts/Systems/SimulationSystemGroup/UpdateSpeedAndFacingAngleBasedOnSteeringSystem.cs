using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateAfter(typeof(CalculateSteeringSystem))]
    public class UpdateSpeedAndFacingAngleBasedOnSteeringSystem : JobComponentSystem
    {
        private EntityQuery _steeringMovementQuery;
        private EntityQuery _steeringStrengthQuery;
        
        private (bool IsRetrieved, float MaxSpeed, float Acceleration) _steeringMovement;
        private (bool AreRetrieved, float Wall, float Pheromone) _steeringStrengths;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            this._steeringMovementQuery =
                GetEntityQuery(ComponentType.ReadOnly<SteeringMovement>());
            this._steeringStrengthQuery =
                GetEntityQuery(ComponentType.ReadOnly<SteeringStrength>());
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
            
            if (!this._steeringStrengths.AreRetrieved)
            {
                var steeringStrength = this._steeringStrengthQuery.GetSingleton<SteeringStrength>();
                this._steeringStrengths = 
                    (AreRetrieved: true, 
                        Wall: steeringStrength.Wall,
                        Pheromone: steeringStrength.Pheromone);
            }
            
            JobHandle updateSpeedJob = new UpdateSpeedJob
            {
                MaxSpeed = this._steeringMovement.MaxSpeed,
                Acceleration = this._steeringMovement.Acceleration
            }.Schedule(this, inputDeps);
            
            JobHandle updateFacingAngleJob = new UpdateFacingAngleJob
            {
                PheromoneStrength = this._steeringStrengths.Pheromone,
                WallStrength = this._steeringStrengths.Wall
            }.Schedule(this, inputDeps);
            
            return JobHandle.CombineDependencies(updateSpeedJob, updateFacingAngleJob);
        }
        
        [BurstCompile]
        private struct UpdateSpeedJob : IJobForEach<PheromoneSteering, WallSteering, Speed>
        {
            public float MaxSpeed;
            public float Acceleration;
            
            public void Execute(
                [ReadOnly] ref PheromoneSteering pheromone,
                [ReadOnly] ref WallSteering wall, 
                ref Speed speed)
            {
                speed.Value += 
                    (this.MaxSpeed * (1 - (math.abs(pheromone.Value) + math.abs(wall.Value)) / 3) - speed.Value) * this.Acceleration;
            }
        }
        
        [BurstCompile]
        private struct UpdateFacingAngleJob : IJobForEach<PheromoneSteering, WallSteering, FacingAngle>
        {
            public float PheromoneStrength;
            public float WallStrength;

            public void Execute(
                [ReadOnly] ref PheromoneSteering pheromone,
                [ReadOnly] ref WallSteering wall, 
                [WriteOnly] ref FacingAngle facingAngle)
            {
                facingAngle.Value += pheromone.Value * this.PheromoneStrength + wall.Value * this.WallStrength;
            }
        }
    }
}