using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(CarMovementGroup))]
[UpdateAfter(typeof(RoadToIntersectionSystem))]
public partial class IntersectionSchedulerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Persistent);
        var random = new Random(1234);

        var singleton = GetSingletonEntity<SplineDefArrayElement>();
        var splinesArray = GetBuffer<SplineDefArrayElement>(singleton);
        var splineIdToRoadArray = GetBuffer<SplineIdToRoad>(singleton);
        var linkArray = GetBuffer<SplineLink>(singleton);
        
//TODO instead of having component acting as tag, use a component with bool or something to represent the states => modify a component instead of add remove comp
        Entities
            .WithAll<CarQueue>().WithNone<CarQueueMaxLength>()//TODO find only intersection but better than that pls!
            .ForEach((Entity intersectionEntity, in Translation translation) =>
            {
                var intersectionQueue = GetBuffer<CarQueue>(intersectionEntity);
                if (intersectionQueue.IsEmpty)
                    return;

                var firstCar = intersectionQueue[0].Value;
                if (HasComponent<WaitingAtIntersection>(firstCar))
                {
                    var currentSpline = GetComponent<SplineDef>(firstCar);
                    var neighbors = linkArray[currentSpline.splineId];
                    var rand = random.NextInt(0, 2);
                    var nextSplineId = rand == 0 || neighbors.Value.y < 0 ? neighbors.Value.x : neighbors.Value.y;
                    
                    //is road full?
                    var nextRoad = splineIdToRoadArray[nextSplineId].Value;
                    var nextRoadQueue = GetBuffer<CarQueue>(nextRoad);
                    var maxLength = GetComponent<CarQueueMaxLength>(nextRoad);
                    if (nextRoadQueue.Length >= maxLength.length)
                        return; //TODO test the other road if any
                    
                    var currentRoadQueue = GetBuffer<CarQueue>(splineIdToRoadArray[currentSpline.splineId].Value);
                    currentRoadQueue.RemoveAt(0);
                    
                    //create dummy spline between current and next spline
                    var nextSpline = splinesArray[nextSplineId].Value;
                    var start = currentSpline.endPoint;
                    var end = nextSpline.startPoint;
                    
                    var isUTurn = Math.Abs(currentSpline.splineId - nextSpline.splineId) == 1;
                    if (isUTurn)
                    {
                        start = CarPositionUpdateSystem.Extrude(currentSpline, new SplinePosition {position = 1f}, out _);
                        end = CarPositionUpdateSystem.Extrude(nextSpline, new SplinePosition {position = 0f}, out _);
                        var plane = translation.Value * currentSpline.endNormal;
                        if (plane.x != 0)
                        {
                            var x = Math.Abs(plane.x);
                            start.x = x;
                            end.x = x;
                        }
                        else if (plane.y != 0)
                        {
                            var y = Math.Abs(plane.y);
                            start.y = y;
                            end.y = y;
                        }
                        else
                        {
                            var z = Math.Abs(plane.z);
                            start.z = z;
                            end.z = z;
                        }
                    }

                    var anchorFactor = isUTurn ? .75f : .5f;
                    var dummySpline = new SplineDef
                    {
                        startPoint = start,
                        endPoint = end,
                        
                        anchor1 = (translation.Value - start)* anchorFactor + start,
                        anchor2 = (translation.Value - end)* anchorFactor + end,
                        startNormal = currentSpline.endNormal,
                        endNormal = currentSpline.startNormal,
                        startTangent = currentSpline.endTangent,
                        endTangent = nextSpline.startTangent,
                        twistMode = -1,
                        offset = isUTurn ? new float2(0f, currentSpline.offset.y) : currentSpline.offset,
                        splineId = -1
                    };
                    
                    SetComponent(firstCar, dummySpline);
                    
                    nextRoadQueue.Add(firstCar);

                    ecb.RemoveComponent<WaitingAtIntersection>(firstCar);
                    ecb.AddComponent(firstCar, new InIntersection {nextSpline = nextSplineId, intersection = intersectionEntity});
                    ecb.RemoveComponent<RoadCompleted>(firstCar);
                }
                
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
