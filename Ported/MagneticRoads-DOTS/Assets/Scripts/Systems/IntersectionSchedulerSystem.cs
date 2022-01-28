using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

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
            .ForEach((Entity entity) =>
            {
                var intersectionQueue = GetBuffer<CarQueue>(entity);
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
                    var dummySpline = new SplineDef
                    {
                        startPoint = currentSpline.endPoint,
                        endPoint = nextSpline.startPoint,
                        anchor1 = currentSpline.endPoint,
                        anchor2 = nextSpline.startPoint,
                        startNormal = currentSpline.endNormal,
                        endNormal = currentSpline.startNormal,
                        startTangent = currentSpline.endTangent,
                        endTangent = nextSpline.startTangent,
                        twistMode = -1,
                        offset = currentSpline.offset,
                        splineId = -1
                    };
                    
                    SetComponent(firstCar, dummySpline);
                    
                    nextRoadQueue.Add(firstCar);

                    ecb.RemoveComponent<WaitingAtIntersection>(firstCar);
                    ecb.AddComponent(firstCar, new InIntersection {nextSpline = nextSplineId, intersection = entity});
                    ecb.RemoveComponent<RoadCompleted>(firstCar);
                }
                
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
