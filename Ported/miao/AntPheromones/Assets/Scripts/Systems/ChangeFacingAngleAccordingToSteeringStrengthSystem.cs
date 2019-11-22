using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
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
                GetEntityQuery(ComponentType.ReadOnly<SteeringStrengthComponent>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!this._steeringStrengths.AreRetrieved)
            {
                var steeringStrength = this._steeringStrengthQuery.GetSingleton<SteeringStrengthComponent>();
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
        private struct Job : IJobForEach<PheromoneSteeringComponent, WallSteeringComponent, FacingAngleComponent>
        {
            public float PheromoneStrength;
            public float WallStrength;

            public void Execute(
                [ReadOnly] ref PheromoneSteeringComponent pheromone,
                [ReadOnly] ref WallSteeringComponent wall, 
                [WriteOnly] ref FacingAngleComponent facingAngle)
            {
                facingAngle.Value += pheromone.Value * this.PheromoneStrength;
                facingAngle.Value += wall.Value * this.WallStrength;
            }
        }
    }
}
