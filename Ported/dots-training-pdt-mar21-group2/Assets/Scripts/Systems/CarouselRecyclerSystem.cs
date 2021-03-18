using System;
using System.Transactions;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

public static class RandomExtensions
{
    public static float3 NextCarouselPosition(ref this Random random,
        WorldBounds worldBounds, float depth = 0.0f, float minHeight = 0.0f, float maxHeight = 0.0f)
    {
        return new float3(
            random.NextFloat(worldBounds.Width),
            random.NextFloat(minHeight, maxHeight),
            depth);
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
        var deltaTime = Time.DeltaTime;
        var seed = random.NextUInt();

        EntityCommandBufferSystem sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer ecbRock = sys.CreateCommandBuffer();
        EntityCommandBuffer ecbCan = sys.CreateCommandBuffer();
        EntityCommandBuffer ecbScaling = sys.CreateCommandBuffer();
        var ecbParaWriterRock = ecbRock.AsParallelWriter();
        var ecbParaWriterCan = ecbCan.AsParallelWriter();
        var ecbParaWriterScaling = ecbScaling.AsParallelWriter();

        var jobA = Entities
            .WithAll<Rock>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation) =>
                Respawn(ref ecbParaWriterRock, entityInQueryIndex, ref random, seed, entity, translation, worldBounds,
                    new float3(parameters.RockScrollSpeed, 0.0f, 0.0f), parameters.RockMinSize, parameters.RockMaxSize,
                    parameters.RockScrollDepth))
            .ScheduleParallel(Dependency);

        var jobB = Entities
            .WithAll<Can>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation) =>
                Respawn(ref ecbParaWriterCan, entityInQueryIndex, ref random, seed, entity, translation, worldBounds,
                    new float3(parameters.CanScrollSpeed, 0.0f, 0.0f), parameters.CanMinSize, parameters.CanMaxSize,
                    parameters.CanScrollDepth, parameters.CanScrollMinHeight, parameters.CanScrollMaxHeight))
            .ScheduleParallel(Dependency);

        var jobC = Entities
            .ForEach((Entity entity, int entityInQueryIndex, ref Scale currentScale, in TargetScale targetScale) =>
                ScaleUp(ref ecbParaWriterScaling, entityInQueryIndex, entity, ref currentScale, targetScale, deltaTime,
                    1.0f / parameters.ScaleUpTime)
            ).ScheduleParallel(Dependency);

        Dependency = Unity.Jobs.JobHandle.CombineDependencies(jobA, jobB, jobC);
        sys.AddJobHandleForProducer(Dependency);
        s_Random = random;
    }

    static void Respawn(ref EntityCommandBuffer.ParallelWriter ecb, int entityInQueryIndex, ref Random random,
        uint seed, Entity entity, in Translation translation, in WorldBounds worldBounds, float3 baseVelocity,
        float minScale, float maxScale, float depth, float minHeight = 0.0f, float maxHeight = 0.0f)
    {
        if (Utils.WorldIsOutOfBounds(translation.Value, worldBounds.Width, worldBounds.Ground))
        {
            random.InitState(seed * ((uint) entity.Index + 0x12345678));

            Translation newTranslation = new Translation
                {Value = random.NextCarouselPosition(worldBounds, depth, minHeight, maxHeight)};
            ecb.SetComponent<Translation>(entityInQueryIndex, entity, newTranslation);
            ecb.SetComponent(entityInQueryIndex, entity, new Velocity() {Value = baseVelocity});
            ecb.RemoveComponent<Falling>(entityInQueryIndex, entity);
            ecb.AddComponent<Available>(entityInQueryIndex, entity);
            ecb.AddComponent(entityInQueryIndex, entity, new Scale {Value = 0.0f});
            ecb.AddComponent(entityInQueryIndex, entity,
                new TargetScale {Value = random.NextFloat(minScale, maxScale)});
        }
    }

    static void ScaleUp(ref EntityCommandBuffer.ParallelWriter ecb, int entityInQueryIndex, Entity entity,
        ref Scale currentScale, in TargetScale targetScale, float deltaTime, float scaleSpeed)
    {
        currentScale = new Scale
        {
            Value = targetScale.Value * math.saturate(currentScale.Value / targetScale.Value + scaleSpeed * deltaTime)
        };

        if (math.abs(currentScale.Value - targetScale.Value) < math.EPSILON)
        {
            ecb.RemoveComponent<TargetScale>(entityInQueryIndex, entity);
        }
    }
}