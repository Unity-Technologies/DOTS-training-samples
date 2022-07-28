using System;
using Aspects;
using Components;
using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    [BurstCompile]
    public partial struct RedirectCarRoadSegment : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state) { }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            foreach (var carAspect in SystemAPI.Query<CarAspect>().WithAll<TraversingIntersection>())
            {
                if (Math.Abs(carAspect.T - 1) < 0.05f)
                {
                    foreach (var roadSegmentRO in SystemAPI.Query<RefRO<RoadSegment>>())
                    {
                        var rs = roadSegmentRO.ValueRO;
                        if (rs.EndIntersection == carAspect.NextIntersection || rs.StartIntersection == carAspect.NextIntersection)
                        {
                            ecb.SetComponent(carAspect.RoadSegment, rs);
                        }
                    }
                }
            }

            foreach (var carAspect in SystemAPI.Query<CarAspect>().WithNone<TraversingIntersection>())
            {
                foreach (var roadSegmentRO in SystemAPI.Query<RefRO<RoadSegment>>().WithAll<IntersectionSegment>())
                {
                    var rs = roadSegmentRO.ValueRO;
                    if (rs.StartIntersection == carAspect.RoadSegmentAspect.EndIntersection)
                        ecb.SetComponent(carAspect.RoadSegment, rs);
                }
            }
        }
    }
}
