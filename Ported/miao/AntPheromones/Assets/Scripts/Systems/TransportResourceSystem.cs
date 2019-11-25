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
            this._mapQuery = GetEntityQuery(ComponentType.ReadOnly<Map>());
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var map = this._mapQuery.GetSingleton<Map>();
            return new Job
            {
                ColonyPosition = map.ColonyPosition,
                ResourcePosition = map.ResourcePosition
            }.Schedule(this, inputDeps);
        }

        private struct Job : IJobForEach<Position, FacingAngle, ResourceCarrier>
        {
            public float2 ColonyPosition;
            public float2 ResourcePosition;

            public void Execute(
                [ReadOnly] ref Position position,
                [WriteOnly] ref FacingAngle facingAngle, 
                ref ResourceCarrier resourceCarrier)
            {
                if (math.lengthsq(position.Value - (resourceCarrier.IsCarrying ? ColonyPosition : ResourcePosition)) >= 16f)
                {
                    return;
                }

                facingAngle.Value += math.PI;
                resourceCarrier.IsCarrying = !resourceCarrier.IsCarrying;
            }
        }
    }
}