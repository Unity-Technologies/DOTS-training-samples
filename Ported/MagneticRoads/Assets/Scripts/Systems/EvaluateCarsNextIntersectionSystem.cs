using Aspects;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems
{
    [BurstCompile]
    public partial struct EvaluateCarsNextIntersectionSystem : ISystem
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
            foreach (var carAspect in SystemAPI.Query<CarAspect>().WithAll<WaitingAtIntersection>())
            {
                var nextIntersection = Entity.Null;
                foreach (var roadSegment in SystemAPI.Query<RefRO<RoadSegment>>())
                {
                    var roadSegmentRO = roadSegment.ValueRO;
                    
                    if (roadSegmentRO.StartIntersection == carAspect.NextIntersection)
                    {
                        nextIntersection = roadSegmentRO.EndIntersection;
                    }
                    else if (roadSegmentRO.EndIntersection == carAspect.NextIntersection)
                    {
                        nextIntersection = roadSegmentRO.StartIntersection;
                    }

                    carAspect.NextIntersection = nextIntersection;
                }
            }
        }
    }
}
