using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(CarSpawnerSystem))]
public class CarInitializeSystem : SystemBase
{
    private EntityQuery splineQuery;
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        splineQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Spline>()
            }
        });

        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        var random = new Random( (uint) Time.ElapsedTime + 18564584);

        var splineEntities = splineQuery.ToEntityArrayAsync(Allocator.TempJob, out var splineEntitiesHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, splineEntitiesHandle);

        Entities
            .WithName("CarInitSystem")
            .WithDisposeOnCompletion(splineEntities)
            .WithAll<Disabled>()
            .ForEach((Entity entity, int entityInQueryIndex, 
                ref Offset offset,
                ref Size size,
                ref Speed speed,
                ref BelongToSpline belongToSpline,
                ref CurrentSegment currentSegment,
                ref Progress progress
            ) =>
            {
                //Initializing car data
                offset.Value = random.NextFloat(-1.0F, 1.0F);
                var newSize = new float3(1f,1f,1f);
                newSize.x = random.NextFloat(1.0F, 2.0F);
                newSize.y = random.NextFloat(1.0F, 2.0F);
                newSize.z = random.NextFloat(1.0F, 2.0F);
                size.Value = newSize;
                speed.Value = random.NextFloat(1.0F, 2.0F);
                progress.Value = 0f;

                //Spline and segment
                int randomSplineId = random.NextInt(0,splineEntities.Length);
                belongToSpline.Value = splineEntities[randomSplineId];
                var splineData = GetComponent<Spline>(splineEntities[randomSplineId]);
                currentSegment.Value = splineData.Value.Value.Segments[0];

                //Enable the car
                commandBuffer.RemoveComponent<Disabled>(entityInQueryIndex,entity);
            }).ScheduleParallel();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}