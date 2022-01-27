using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(CarMovementGroup))]
[UpdateAfter(typeof(IntersectionSchedulerSystem))]
public partial class IntersectionToRoadSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Persistent);

        var singleton = GetSingletonEntity<SplineDefArrayElement>();
        var splinesArray = GetBuffer<SplineDefArrayElement>(singleton);

        Entities
            .WithAll<SplinePosition, RoadCompleted, InIntersection>()
            .ForEach((Entity entity, ref SplineDef SplineDef, ref SplinePosition splinePosition, in InIntersection inIntersection) =>
            {
                var intersectionQueue = GetBuffer<CarQueue>(inIntersection.intersection);
                intersectionQueue.RemoveAt(0);

                var nextSpline = splinesArray[inIntersection.nextSpline];
                SplineDef = nextSpline;
                
                splinePosition.position = 0f;
                
                ecb.RemoveComponent<InIntersection>(entity);
                ecb.RemoveComponent<RoadCompleted>(entity);
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
