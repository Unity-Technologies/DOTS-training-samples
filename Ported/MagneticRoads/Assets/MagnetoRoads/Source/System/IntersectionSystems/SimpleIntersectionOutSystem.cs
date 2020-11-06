using Unity.Entities;

[UpdateAfter(typeof(SimpleIntersectionActiveCarSystem))]
[UpdateBefore(typeof(DoubleIntersectionSystem))]
public class SimpleIntersectionOutSystem : SystemBase
{
    EndFixedStepSimulationEntityCommandBufferSystem ecbSystem;
    
    protected override void OnCreate()
    {
        ecbSystem = World.GetExistingSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = ecbSystem.CreateCommandBuffer();
        var ecbWriter = ecb.AsParallelWriter();
        
        var positionAccessor = GetComponentDataFromEntity<CarPosition>();
        Entities
            .WithNone<IntersectionNeedsInit>()
            .WithNativeDisableContainerSafetyRestriction(positionAccessor)
            .ForEach((Entity entity, int entityInQueryIndex, ref SimpleIntersection simpleIntersection) =>
            {
                if (simpleIntersection.car == Entity.Null)
                    return;

                if (positionAccessor[simpleIntersection.car].Value < 1)
                    return;
                
                ecbWriter.AppendToBuffer(entityInQueryIndex, simpleIntersection.laneOut0, (CarBufferElement) simpleIntersection.car);
                positionAccessor[simpleIntersection.car] = new CarPosition {Value = 0};
                simpleIntersection.car = Entity.Null;
            }).ScheduleParallel();
        
        ecbSystem.AddJobHandleForProducer(Dependency);
    }
}
