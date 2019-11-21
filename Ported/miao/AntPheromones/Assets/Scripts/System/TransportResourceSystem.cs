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
        private MapComponent _map;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            this._map = GetEntityQuery(ComponentType.ReadOnly<MapComponent>()).GetSingleton<MapComponent>();
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
                if (math.lengthsq(position.Value - (resourceCarrier.IsCarrying ? ResourcePosition : ColonyPosition)) >= 16f)
                {
                    return;
                }
                
                facingAngleComponent.Value += math.PI;
                resourceCarrier.IsCarrying = !resourceCarrier.IsCarrying;
            }
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job
            {
                ColonyPosition = this._map.ColonyPosition,
                ResourcePosition = this._map.ResourcePosition
            }.Schedule(this, inputDeps);
        }
    }
}