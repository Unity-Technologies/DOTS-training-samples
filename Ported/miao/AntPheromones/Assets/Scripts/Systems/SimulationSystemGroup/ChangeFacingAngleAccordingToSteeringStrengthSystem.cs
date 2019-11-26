using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace AntPheromones_ECS
{
    [UpdateAfter(typeof(CalculatePheromoneSteeringSystem))]
    [UpdateAfter(typeof(CalculateWallSteeringSystem))]
    public class ChangeFacingAngleAccordingToSteeringStrengthSystem : JobComponentSystem
    {
        private EntityQuery _steeringStrengthQuery;
        private (bool AreRetrieved, float Wall, float Pheromone) _steeringStrengths;

        protected override void OnCreate()
        {
            base.OnCreate();
            this._steeringStrengthQuery =
                GetEntityQuery(ComponentType.ReadOnly<SteeringStrength>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!this._steeringStrengths.AreRetrieved)
            {
                var steeringStrength = this._steeringStrengthQuery.GetSingleton<SteeringStrength>();
                this._steeringStrengths = 
                    (AreRetrieved: true, 
                    Wall: steeringStrength.Wall,
                    Pheromone: steeringStrength.Pheromone);
            }
            
            return new Job
            {
                PheromoneStrength = this._steeringStrengths.Pheromone,
                WallStrength = this._steeringStrengths.Wall
            }.Schedule(this, inputDeps);
        }

        [BurstCompile]
        private struct Job : IJobForEach<PheromoneSteering, WallSteering, FacingAngle>
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
