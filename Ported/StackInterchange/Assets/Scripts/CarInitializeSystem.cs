using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class CarInitializeSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithName("CarInitSystem")
            .WithAll<Disabled>()
            .ForEach((Entity entity, int entityInQueryIndex, 
                ref OffsetAuthoring offset,
                ref SizeAuthoring size,
                ref SpeedAuthoring speed,
                ref BelongToSplineAuthoring belongToSpline,
                ref CurrentSegmentAuthoring currentSegment,
                ref ProgressAuthoring progress
            ) =>
            {
                //Initializing car data

                progress.Value = 0f;

                //Enable the car
                commandBuffer.RemoveComponent<Disabled>(entityInQueryIndex,entity);
            }).ScheduleParallel();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}