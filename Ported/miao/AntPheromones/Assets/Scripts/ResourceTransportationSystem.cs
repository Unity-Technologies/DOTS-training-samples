using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    public class ResourceTransportationSystem : JobComponentSystem
    {
        private Map _map;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            
            EntityQuery entityQuery = GetEntityQuery(ComponentType.ReadOnly<Map>());
            this._map = entityQuery.GetSingleton<Map>();
        }

        private struct Job : IJobForEach<Position, FacingAngle, ResourceCarrier>
        {
            public float2 ColonyPosition;
            public float2 ResourcePosition;
            
            public void Execute([ReadOnly] ref Position position, [WriteOnly] ref FacingAngle facingAngle, ref ResourceCarrier resourceCarrier)
            {
                float2 targetPosition = resourceCarrier.IsCarrying ? ResourcePosition : ColonyPosition;
                
                if (!(math.lengthsq(position.Value - targetPosition) < 4f * 4f))
                {
                    return;
                }
                
                facingAngle.Value += math.PI;
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