using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class MoveToTargetLocationSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        var ecb = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.ForEach((Entity e, int entityInQueryIndex, ref Translation translation, in TargetDestination target) => {
            var distanceInFrame = 1f * deltaTime; // TODO: Load speed from constants, hinder when bucket is carried.

            var direction = target.Value - translation.Value.xz;
            var length = math.length(direction);

            // TODO: Remove target component when this happens?
            if (length == 0)
            {
                ecb.RemoveComponent<TargetDestination>(entityInQueryIndex, e);
                return;
            }

            distanceInFrame = math.min(distanceInFrame, length);
            var toMove = distanceInFrame * (direction / length);

            translation.Value += new float3(toMove.x, 0, toMove.y);
        }).ScheduleParallel();

        // HACK: Lazy buy functional
        Entities.ForEach((Entity e, int entityInQueryIndex, ref Translation translation, in TargetDestination target, in HoldingBucket holdingBucket) => {
            if (holdingBucket.HeldBucket != Entity.Null)
                ecb.SetComponent(entityInQueryIndex, holdingBucket.HeldBucket, new Translation { Value = translation.Value + math.up() });
        }).ScheduleParallel();

        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
