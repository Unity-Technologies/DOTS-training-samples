using System.Transactions;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public static class RandomExtensions
{
    public static float3 NextCarouselPosition(ref this Random random,
        WorldBounds worldBounds, float Depth = 0.0f, float MinHeight = 0.0f, float MaxHeight = 0.0f)
    {
        return new float3(
            random.NextFloat(0.0f, worldBounds.Width),
            random.NextFloat(MinHeight, MaxHeight),
            Depth);
    }
}


public class CarouselRecyclerSystem : SystemBase
{
    static Random s_Random = new Random(1234);

    protected override void OnUpdate()
    {
        var parameters = GetSingleton<SimulationParameters>();
        var worldBounds = GetSingleton<WorldBounds>();
        var random = s_Random;
        EntityCommandBufferSystem sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer ecbRock = sys.CreateCommandBuffer();
        EntityCommandBuffer ecbCan = sys.CreateCommandBuffer();
        var ecbParaWriterRock = ecbRock.AsParallelWriter();
        var ecbParaWriterCan = ecbCan.AsParallelWriter();

        var jobA = Entities
            .WithAll<Rock>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation) =>
            {
                if (Utils.WorldIsOutOfBounds(translation.Value, worldBounds.Width, worldBounds.Ground))
                {
                    //translation.Value = random.NextCarouselPosition(worldBounds, 5.0f);
                    random.InitState((uint)(0x123456 * entity.Index));
                    Translation newTranslation = new Translation
                    {
                        Value = random.NextCarouselPosition(worldBounds, parameters.RockScrollDepth)
                    };
                    ecbParaWriterRock.SetComponent<Translation>(entityInQueryIndex, entity, newTranslation);
                    ecbParaWriterRock.SetComponent(entityInQueryIndex, entity, new Velocity()
                    {
                        Value = new float3(parameters.RockScrollSpeed, 0.0f, 0.0f)
                    });
                    ecbParaWriterRock.RemoveComponent<Falling>(entityInQueryIndex, entity);
                    ecbParaWriterRock.AddComponent<Available>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel(Dependency); 
        
        var jobB = Entities
            .WithAll<Can>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation) =>
            {
                if (Utils.WorldIsOutOfBounds(translation.Value, worldBounds.Width, worldBounds.Ground))
                {
                    //translation.Value = random.NextCarouselPosition(worldBounds, 60.0f, 10.0f, 20.0f);
                    random.InitState((uint) (0x123456 * entity.Index));
                    Translation newTranslation = new Translation
                    {
                        Value = random.NextCarouselPosition(worldBounds, 
                            parameters.CanScrollDepth,
                            parameters.CanScrollMinHeight,
                            parameters.CanScrollMaxHeight)
                    };
                    ecbParaWriterCan.SetComponent(entityInQueryIndex, entity, new Velocity()
                    {
                        Value = new float3(parameters.CanScrollSpeed, 0.0f, 0.0f)
                    });
                    ecbParaWriterCan.SetComponent<Translation>(entityInQueryIndex, entity, newTranslation);
                    ecbParaWriterCan.RemoveComponent<Falling>(entityInQueryIndex, entity);
                    ecbParaWriterCan.AddComponent<Available>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel(Dependency);

        Dependency = Unity.Jobs.JobHandle.CombineDependencies(jobA, jobB);
        sys.AddJobHandleForProducer(Dependency);
        s_Random = random;
    }
}