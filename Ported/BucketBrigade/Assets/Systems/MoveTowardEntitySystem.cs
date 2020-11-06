using System.Diagnostics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveTowardEntitySystem : SystemBase
{
    public static readonly float Speed = 10.0f;
    public static readonly float Threshold = 1.0f;

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        FireSim fireSim = GetSingleton<FireSim>();

        EntityCommandBufferSystem sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = sys.CreateCommandBuffer().AsParallelWriter();
        var cdfe = GetComponentDataFromEntity<Translation>();

        Entities
            .WithNativeDisableParallelForRestriction(cdfe)
            .ForEach((Entity entity, int entityInQueryIndex, in MoveTowardBucket scooper, in FillerBot source) =>
        {
            var bucketPosition = cdfe[scooper.Target].Value;
            var scooperPosition = cdfe[entity].Value;
            bucketPosition.y = scooperPosition.y;

            var distanceLeft = bucketPosition - scooperPosition;
            var length = math.length(distanceLeft);

            if (length <= Threshold)
            {
                cdfe[entity] = new Translation() { Value = bucketPosition };

                ecb.AddComponent(entityInQueryIndex, entity, new HoldingBucket() { Target = scooper.Target });

                ecb.RemoveComponent<MoveTowardBucket>(entityInQueryIndex, entity);
                ecb.AddComponent(entityInQueryIndex, entity, new MoveTowardFiller()
                {
                    Target = source.ChainStart
                }); 
            }
            else
            {
                var direction = distanceLeft / length;
                var newPosition = scooperPosition + Speed * deltaTime * direction;
                cdfe[entity] = new Translation() { Value = newPosition };
            }
        }).ScheduleParallel();


        Entities
            .WithNativeDisableParallelForRestriction(cdfe)
            .ForEach((Entity entity, int entityInQueryIndex, in MoveTowardFiller source, in HoldingBucket bucket) =>
            {
                var fillerPosition = cdfe[source.Target].Value;
                var scooperPosition = cdfe[entity].Value;
                fillerPosition.y = scooperPosition.y;

                var distanceLeft = fillerPosition - scooperPosition;
                var length = math.length(distanceLeft);

                if (length <= Threshold)
                {
                    cdfe[entity] = new Translation() { Value = fillerPosition };
                    fillerPosition.y += 1.6f;
                    cdfe[bucket.Target] = new Translation() { Value = fillerPosition };

                    ecb.RemoveComponent<MoveTowardFiller>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<HoldingBucket>(entityInQueryIndex, entity);
                    ecb.AddComponent<FindBucket>(entityInQueryIndex, entity);

                    ecb.AddComponent(entityInQueryIndex, bucket.Target, new BucketReadyFor() { Index = 0 });
                }
                else
                {
                    var direction = distanceLeft / length;
                    var newPosition = scooperPosition + Speed * deltaTime * direction;
                    cdfe[entity] = new Translation() { Value = newPosition };
                    newPosition.y += 1.6f;
                    cdfe[bucket.Target] = new Translation() { Value = newPosition };
                }
            }).ScheduleParallel();


        Entities
            .WithNativeDisableParallelForRestriction(cdfe)
            .ForEach((Entity entity, int entityInQueryIndex, in MoveTowardFire bot, in HoldingBucket bucket) =>
            {
                var firePosition = cdfe[bot.Target].Value;
                var botPosition = cdfe[entity].Value;
                firePosition.y = botPosition.y;

                var distanceLeft = firePosition - botPosition;
                var length = math.length(distanceLeft);

                if (length <= Threshold)
                {
                    cdfe[entity] = new Translation() { Value = firePosition };
                    firePosition.y += 1.6f;
                    cdfe[bucket.Target] = new Translation() { Value = firePosition };

                    ecb.RemoveComponent<MoveTowardFire>(entityInQueryIndex, entity);
                }
                else
                {
                    var direction = distanceLeft / length;
                    var newPosition = botPosition + Speed * deltaTime * direction;
                    cdfe[entity] = new Translation() { Value = newPosition };
                    newPosition.y += 1.6f;
                    cdfe[bucket.Target] = new Translation() { Value = newPosition };
                }
            }).ScheduleParallel();

        Entities
            .WithNativeDisableParallelForRestriction(cdfe)
            .ForEach((Entity entity, int entityInQueryIndex, in GotoDropoffLocation tag, in PasserBot chain, in HoldingBucket bucket, in Bot bot) =>
            {
                var distanceLeft = chain.DropoffPosition - cdfe[entity].Value;
                var length = math.length(distanceLeft);

                if (length <= Threshold)
                {
                    var newPosition = chain.DropoffPosition;
                    cdfe[entity] = new Translation() { Value = newPosition };
                    newPosition.y += 1.6f;
                    cdfe[bucket.Target] = new Translation() { Value = newPosition };

                    ecb.RemoveComponent<GotoDropoffLocation>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<HoldingBucket>(entityInQueryIndex, entity);

                    var next = (bot.Index == fireSim.NumBotsPerChain * 2 - 1) ? 0 : bot.Index + 1;
                    ecb.AddComponent<GotoPickupLocation>(entityInQueryIndex, entity);
                    ecb.AddComponent(entityInQueryIndex, bucket.Target, new BucketReadyFor() { Index = next });
                }
                else
                {
                    var direction = distanceLeft / length;
                    var newPosition = cdfe[entity].Value + Speed * deltaTime * direction;
                    cdfe[entity] = new Translation() { Value = newPosition };
                    newPosition.y += 1.6f;
                    cdfe[bucket.Target] = new Translation() { Value = newPosition };
                }
            }).ScheduleParallel();

        sys.AddJobHandleForProducer(Dependency);
    }
}
