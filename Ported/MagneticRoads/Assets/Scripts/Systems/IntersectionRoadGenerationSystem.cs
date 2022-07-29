using Aspects;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Util;

namespace Systems
{
    [UpdateAfter(typeof(IntersectionOccupiedSystem))]
    [BurstCompile]
    partial struct IntersectionRoadGenerationSystem : ISystem
    {
        ComponentDataFromEntity<RoadSegment> m_RoadSegmentDataFromEntity;
        ComponentDataFromEntity<LocalToWorld> m_LocalToWorld;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_RoadSegmentDataFromEntity = state.GetComponentDataFromEntity<RoadSegment>(true);
            m_LocalToWorld = state.GetComponentDataFromEntity<LocalToWorld>(true);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            m_RoadSegmentDataFromEntity.Update(ref state);
            m_LocalToWorld.Update(ref state);

            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var carAspect in SystemAPI.Query<CarAspect>().WithAll<WaitingAtIntersection>())
            {
                Spline.RoadTerminator tempRoadSegmentStart = new Spline.RoadTerminator();
                Spline.RoadTerminator tempRoadSegmentEnd = new Spline.RoadTerminator();

                //even lanes are backwards
                var isBackwards = (carAspect.LaneNumber % 2 == 0);
                var carRoadSegment = m_RoadSegmentDataFromEntity[carAspect.RoadSegmentEntity];
                tempRoadSegmentStart = isBackwards ? carRoadSegment.End : carRoadSegment.Start;

                foreach (var roadSegment in SystemAPI.Query<RefRO<RoadSegment>>())
                {
                    var roadSegmentRO = roadSegment.ValueRO;

                    tempRoadSegmentEnd = roadSegmentRO.Start;

                    if (roadSegmentRO.StartIntersection == carAspect.NextIntersection)
                    {
                        tempRoadSegmentEnd = roadSegmentRO.Start;

                        //validIntersection = roadSegmentRO.EndIntersection;
                        // nextRoadSegment = roadSegmentRO;
                    }
                    else if (roadSegmentRO.EndIntersection == carAspect.NextIntersection)
                    {
                        // validIntersection = roadSegmentRO.StartIntersection;
                        // nextRoadSegment = roadSegmentRO;

                        tempRoadSegmentEnd = roadSegmentRO.End;
                    }
                }

                var intersectionRS = carAspect.NextIntersection;
                
                var nextIntersection = m_LocalToWorld[carAspect.NextIntersection];
               
                var Start = new Spline.RoadTerminator
                {
                    Position = (nextIntersection.Position + tempRoadSegmentStart.Position) * 0.5f,
                    Normal = nextIntersection.Up,
                    Tangent = math.round(math.normalize(nextIntersection.Position - tempRoadSegmentStart.Position))
                };
                var End = new Spline.RoadTerminator
                {
                    Position = (nextIntersection.Position + tempRoadSegmentEnd.Position) * 0.5f,
                    Normal = nextIntersection.Up,
                    Tangent = math.round(math.normalize(nextIntersection.Position - tempRoadSegmentEnd.Position))
                };
                ecb.SetComponent(intersectionRS, new RoadSegment
                {
                    Start = Start,
                    End = End,
                    Length = Spline.EvaluateLength(Start, End)
                });

                ecb.SetComponentEnabled<TraversingIntersection>(carAspect.Entity, true);
                ecb.SetComponentEnabled<WaitingAtIntersection>(carAspect.Entity, false);

                // carAspect.NextIntersection = validIntersection;
                // ecb.set(carAspect.RoadSegment, rs);
            }

            // now we'll prepare our intersection spline - this lets us
            // create a "temporary lane" inside the current intersection
            /* intersectionSpline.anchor1 = (intersection.position + intersectionSpline.startPoint)*.5f;
             intersectionSpline.startTangent = Vector3Int.RoundToInt((intersection.position - intersectionSpline.startPoint).normalized);
            intersectionSpline.startNormal = intersection.normal;
            
             intersectionSpline.anchor2 = (intersection.position + intersectionSpline.endPoint) * .5f;
             intersectionSpline.endTangent = Vector3Int.RoundToInt((intersection.position - intersectionSpline.endPoint).normalized);
             intersectionSpline.endNormal = intersection.normal;*/
        }
    }
}
