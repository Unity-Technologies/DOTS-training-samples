using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(CalculatePheromoneSteeringSystem))]
    [UpdateAfter(typeof(CalculateWallSteeringSystem))]
    public class ChangeFacingAngleSystem : JobComponentSystem
    {
        private SteeringStrengthComponent _steeringStrength;

        protected override void OnCreate()
        {
            base.OnCreate();
            this._steeringStrength =
                GetEntityQuery(ComponentType.ReadOnly<SteeringStrengthComponent>()).GetSingleton<SteeringStrengthComponent>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job
            {
                PheromoneStrength = this._steeringStrength.Pheromone,
                WallStrength = this._steeringStrength.Wall
            }.Schedule(this, inputDeps);
        }

        private struct Job : IJobForEach<PheromoneSteeringComponent, WallSteeringComponent, FacingAngleComponent>
        {
            public float PheromoneStrength;
            public float WallStrength;

            public void Execute(
                [ReadOnly] ref PheromoneSteeringComponent pheromone,
                [ReadOnly] ref WallSteeringComponent wall, 
                ref FacingAngleComponent facingAngle)
            {
                facingAngle.Value += pheromone.Value * this.PheromoneStrength;
                facingAngle.Value += wall.Value * this.WallStrength;
            }
        }
    }
}
