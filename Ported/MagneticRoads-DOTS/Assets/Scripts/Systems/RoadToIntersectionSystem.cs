using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(CarMovementGroup))]
[UpdateAfter(typeof(CarMovementSystem))]
public partial class RoadToIntersectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Persistent);

        var singleton = GetSingletonEntity<SplineDefArrayElement>();
        var splineToIntersection = GetBuffer<IntersectionArrayElement>(singleton);

        Entities
            .WithAll<SplinePosition, SplineDef, RoadCompleted>()
            .WithNone<InIntersection,WaitingAtIntersection>()
            .ForEach((Entity entity, ref SplineDef SplineDef, ref SplinePosition splinePosition) =>
            {
                var intersectionQueue = GetBuffer<CarQueue>(splineToIntersection[SplineDef.splineId].Value);
                intersectionQueue.Add(entity);
                splinePosition.position = 0f;
                ecb.AddComponent<WaitingAtIntersection>(entity);
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
