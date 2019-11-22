using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(OrientTowardsGoalSystem))]
    public class TransportResourceSystem : JobComponentSystem
    {
        private EntityQuery _mapQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            this._mapQuery = GetEntityQuery(ComponentType.ReadOnly<MapComponent>());
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var map = this._mapQuery.GetSingleton<MapComponent>();
            return new Job
            {
                ColonyPosition = map.ColonyPosition,
                ResourcePosition = map.ResourcePosition
            }.Schedule(this, inputDeps);
        }

        private struct Job : IJobForEach<PositionComponent, FacingAngleComponent, ResourceCarrierComponent>
        {
            public float2 ColonyPosition;
            public float2 ResourcePosition;

            public void Execute(
                [ReadOnly] ref PositionComponent position,
                [WriteOnly] ref FacingAngleComponent facingAngleComponent, 
                ref ResourceCarrierComponent resourceCarrier)
            {
                if (math.lengthsq(position.Value - (resourceCarrier.IsCarrying ? ColonyPosition : ResourcePosition)) >= 16f)
                {
                    return;
                }

                facingAngleComponent.Value += math.PI;
                resourceCarrier.IsCarrying = !resourceCarrier.IsCarrying;
            }
        }
    }
}