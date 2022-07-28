using Aspects;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems
{
    [BurstCompile]
    public partial struct RedirectCarsToNewRoadSegmentSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var carAspect in SystemAPI.Query<CarAspect>().WithAll<WaitingAtIntersection>())
            {
                var validIntersection = Entity.Null;
                var nextRoadSegment = new RoadSegment();
                
                foreach (var roadSegment in SystemAPI.Query<RefRO<RoadSegment>>())
                {
                    var roadSegmentRO = roadSegment.ValueRO;
                    if (roadSegmentRO.StartIntersection == carAspect.NextIntersection)
                    {
                        validIntersection = roadSegmentRO.EndIntersection;
                        nextRoadSegment = roadSegmentRO;
                    }
                    else if (roadSegmentRO.EndIntersection == carAspect.NextIntersection)
                    {
                        validIntersection = roadSegmentRO.StartIntersection;
                        nextRoadSegment = roadSegmentRO;
                    }
                }
                
                carAspect.NextIntersection = validIntersection;
                ecb.SetComponent(carAspect.RoadSegment, nextRoadSegment);
            }
        }
    }
}
