using Aspects;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Util;

namespace Systems
{
    partial struct IntersectionRoadGenerationSystem:ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            
        }

        public void OnDestroy(ref SystemState state)
        {
            
        }

        public void OnUpdate(ref SystemState state)
        {

            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var carAspect in SystemAPI.Query<CarAspect>().WithAll<WaitingAtIntersection>())
            {
                var validIntersection = Entity.Null;
                var nextRoadSegment = new RoadSegment();

                Spline.RoadTerminator tempRoadSegmentStart = new Spline.RoadTerminator();
                Spline.RoadTerminator tempRoadSegmentEnd = new Spline.RoadTerminator();
                
                //even lanes are backwards
                var isBackwards = (carAspect.LaneNumber % 2 == 0);

                tempRoadSegmentStart = isBackwards ? carAspect.RoadSegment.End : carAspect.RoadSegment.Start;

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

                var intersectionTransformAspect = SystemAPI.GetAspect<TransformAspect>(carAspect.NextIntersection);
                
                var rs = ecb.CreateEntity();

                ecb.AddComponent<IntersectionSegment>(rs);
                ecb.AddComponent<RoadSegment>(rs);
                
                var Start = new Spline.RoadTerminator
                {
                    Position = (intersectionTransformAspect.Position + tempRoadSegmentStart.Position)*0.5f,
                    Normal = intersectionTransformAspect.Up,
                    Tangent =  math.round(math.normalize(intersectionTransformAspect.Position - tempRoadSegmentStart.Position))  
                };
                var End = new Spline.RoadTerminator
                {
                    Position = (intersectionTransformAspect.Position + tempRoadSegmentEnd.Position)*0.5f,
                    Normal = intersectionTransformAspect.Up,
                    Tangent =  math.round(math.normalize(intersectionTransformAspect.Position - tempRoadSegmentEnd.Position)) 
                };
                ecb.SetComponent(rs, new RoadSegment
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
