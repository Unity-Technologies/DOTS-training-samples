using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
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

        //Initializing the data for all the disabled cars
        Entities
            .WithName("CarInitSystem")
            .WithDisposeOnCompletion(splineEntities)
            .WithAll<Disabled>()
            .ForEach((
                ref Offset offset,
                ref Size size,
                ref Speed speed,
                ref BelongToSpline belongToSpline,
                ref CurrentSegment currentSegment,
                ref Progress progress,
                ref URPMaterialPropertyBaseColor color
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

                //Color based on spline destination
                int destinationType = randomSplineId % 4; //repeat the colors for now
                switch(destinationType)
                {
                    case 0: color.Value = new float4(1,0,0,1); break; //Red
                    case 1: color.Value = new float4(0,0,1,1); break; //Blue
                    case 2: color.Value = new float4(0.5f,0,1,1); break; //Purple
                    case 3: color.Value = new float4(1,0.8f,1,1); break; //Pink
                }

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