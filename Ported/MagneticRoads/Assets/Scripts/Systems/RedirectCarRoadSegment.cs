using System;
using Aspects;
using Components;
using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    [UpdateAfter(typeof(IntersectionRoadGenerationSystem))]
    [BurstCompile]
    public partial struct RedirectCarRoadSegment : ISystem
    {
        ComponentDataFromEntity<RoadSegment> m_RoadSegmentDataFromEntity;
        BufferFromEntity<LaneDynamicBuffer> m_LaneDynamicBuffer;
        BufferFromEntity<CarDynamicBuffer> m_CarDynamicBuffer;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_RoadSegmentDataFromEntity = state.GetComponentDataFromEntity<RoadSegment>(true);
            m_LaneDynamicBuffer = state.GetBufferFromEntity<LaneDynamicBuffer>();
            m_CarDynamicBuffer = state.GetBufferFromEntity<CarDynamicBuffer>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            m_RoadSegmentDataFromEntity.Update(ref state);
            m_LaneDynamicBuffer.Update(ref state);
            m_CarDynamicBuffer.Update(ref state);

            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            // This is for the cars that are about to start travelling on the intersection
            foreach (var carAspect in SystemAPI.Query<CarAspect>().WithAll<WaitingAtIntersection>())
            {
                // When this begins we need to remove the car from the buffer associated with the road segment it's leaving
                // var carRoadSegment = m_RoadSegmentDataFromEntity[carAspect.RoadSegmentEntity];
                // var leavingLanes = m_LaneDynamicBuffer[carAspect.RoadSegmentEntity];
                //
                // var leavingLane = leavingLanes[carAspect.LaneNumber - 1].value; // this is the entity storing the car buffer
                // var leavingCarBuffer = m_CarDynamicBuffer[leavingLane];
                //
                // var indexToRemove = 0;
                // if (leavingCarBuffer.Length > 0)
                // {
                //     for (int i = 0; i < leavingCarBuffer.Length; i++)
                //     {
                //         if (leavingCarBuffer[i].value == carAspect.Entity)
                //             indexToRemove = i;
                //     }
                //
                //     leavingCarBuffer.RemoveAt(indexToRemove); // Remove from buffer
                // }
                //
                // // Add the car to the next roadsegment
                // foreach (var rsAspect in SystemAPI.Query<RoadSegmentAspect>().WithNone<IntersectionSegment>())
                // {
                //     if (carRoadSegment.EndIntersection == carAspect.NextIntersection || rsAspect.StartIntersection == carAspect.NextIntersection)
                //     {
                //         var joiningLanes = m_LaneDynamicBuffer[rsAspect.Entity];
                //
                //         var joinLane = joiningLanes[carAspect.LaneNumber - 1].value; // this is the entity storing the car buffer
                //         var carBuffer = m_CarDynamicBuffer[joinLane].Reinterpret<Entity>();
                //
                //         carBuffer.Add(carAspect.Entity);
                //         carAspect.RoadSegmentEntity = rsAspect.Entity;
                //     }
                // }
                ecb.SetComponentEnabled<WaitingAtIntersection>(carAspect.Entity, false);
                carAspect.T = 0;
            }

            // foreach (var carAspect in SystemAPI.Query<CarAspect>().WithAll<TraversingIntersection>().WithAll<WaitingAtIntersection>())
            // {
            //     // Look through all road segments that aren't intersections
            //     foreach (var rsAspect in SystemAPI.Query<RoadSegmentAspect>().WithNone<IntersectionSegment>())
            //     {
            //         // Find the road segment that either has the start or the end marked the same as the cars current road segment
            //         // So we know it's connected
            //         var carRoadSegment = m_RoadSegmentDataFromEntity[carAspect.RoadSegmentEntity];
            //         if (rsAspect.StartPosition.Equals(carRoadSegment.End.Position) || rsAspect.EndPosition.Equals(carRoadSegment.End.Position))
            //         {
            //             // Set the new road segment
            //             ecb.SetComponent(carAspect.RoadSegmentEntity, rsAspect.RoadSegment);
            //
            //             // Add the car to the correct buffer
            //             // var lanes = m_LaneDynamicBuffer[rsAspect.Entity];
            //             // var lane = lanes[carAspect.LaneNumber - 1].value;
            //             // var carBuffer = m_CarDynamicBuffer[lane].Reinterpret<Entity>();
            //             // carBuffer.Add(carAspect.Entity);
            //
            //             ecb.SetComponentEnabled<TraversingIntersection>(carAspect.Entity, false);
            //             ecb.SetComponentEnabled<WaitingAtIntersection>(carAspect.Entity, false);
            //         }
            //     }
        }
    }
}
