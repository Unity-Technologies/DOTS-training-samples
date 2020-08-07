using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(CarInitializeSplineSystem))]
public class CarInitializeSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    Random _random;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        _random = new Random( (uint) 18564584);
    }

    protected override void OnUpdate()
    {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        var random = _random;

        Entities
            .WithName("CarInitSystem")
            .WithAll<Disabled>()
            .ForEach((Entity entity,
                int entityInQueryIndex,
                ref Offset offset,
                ref Size size,
                ref Speed speed,
                ref OriginalSpeed originalSpeed,
                ref Progress progress
            ) =>
            {
                //Initializing car data
                offset.SetOffset(random.NextFloat(-1.0F, 1.0F));
                size.SetSize(random.NextInt());

                originalSpeed.SetSpeed(random.NextFloat(0.0f, 1.0f));
                speed.Value = originalSpeed.GetSpeed();
                progress.Value = random.NextFloat(0.0F, 1.0F);
            }).ScheduleParallel();
        
        //Enable the car after initializing the data
        Entities
            .WithName("CarInitSystem_EnableCar")
            .WithAll<Disabled>()
            .WithAll<BelongToSpline>()
            .ForEach((Entity entity, int entityInQueryIndex) =>
            {
                commandBuffer.RemoveComponent<Disabled>(entityInQueryIndex,entity);

            }).ScheduleParallel();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}