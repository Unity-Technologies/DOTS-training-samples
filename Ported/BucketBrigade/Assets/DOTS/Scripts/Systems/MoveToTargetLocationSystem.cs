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
        var gameConstants = GetSingleton<GameConstants>();

        var ecb = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.
            WithNone<HoldingBucket>().
            ForEach((Entity e, int entityInQueryIndex, ref Translation translation, in TargetDestination target) =>
        {
            var distanceInFrame = gameConstants.FireFighterMovementSpeedNoBucket * deltaTime;

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

            var hasFullBucket = !HasComponent<EmptyBucket>(holdingBucket.HeldBucket);

            var distanceInFrame = hasFullBucket ? gameConstants.FireFighterMovementSpeedBucket * deltaTime : gameConstants.FireFighterMovementSpeedNoBucket * deltaTime;

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

            if (holdingBucket.HeldBucket != Entity.Null)
                ecb.SetComponent(entityInQueryIndex, holdingBucket.HeldBucket, new Translation { Value = translation.Value + math.up() });
        }).ScheduleParallel();

        CommandBufferSystem.AddJobHandleForProducer(Dependency);

        // TODO : remove when we understand the issue between this and PickClosestLake system
        Dependency.Complete();
    }
}
