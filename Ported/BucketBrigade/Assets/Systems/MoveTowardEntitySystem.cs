using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveTowardEntitySystem : SystemBase
{
    protected override void OnUpdate()
    {
        var speed = 10.0f;
        var threshold = 1.0f;
        var deltaTime = Time.DeltaTime;

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

            if (length <= threshold)
            {
                ecb.AddComponent(entityInQueryIndex, entity, new HoldingBucket() { Target = scooper.Target });

                ecb.RemoveComponent<MoveTowardBucket>(entityInQueryIndex, entity);
                //ecb.AddComponent<FindWaterCell>(entityInQueryIndex, entity);
                ecb.AddComponent(entityInQueryIndex, entity, new MoveTowardFiller()
                {
                    Target = source.ChainStart
                }); 
            }
            else
            {
                var direction = distanceLeft / length;
                var newPosition = scooperPosition + speed * deltaTime * direction;
                cdfe[entity] = new Translation() { Value = newPosition };
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

                if (length <= threshold)
                {
                    ecb.RemoveComponent<MoveTowardFire>(entityInQueryIndex, entity);
                }
                else
                {
                    var direction = distanceLeft / length;
                    var newPosition = botPosition + speed * deltaTime * direction;
                    cdfe[entity] = new Translation() { Value = newPosition };
                    newPosition.y += 1.6f;
                    cdfe[bucket.Target] = new Translation() { Value = newPosition };
                }
            }).ScheduleParallel();

        Entities
            .WithNativeDisableParallelForRestriction(cdfe)
            .ForEach((Entity entity, int entityInQueryIndex, in MoveTowardFiller source, in HoldingBucket bucket) =>
            {
                var bucketPosition = cdfe[source.Target].Value;
                var scooperPosition = cdfe[entity].Value;
                bucketPosition.y = scooperPosition.y;

                var distanceLeft = bucketPosition - scooperPosition;
                var length = math.length(distanceLeft);

                if (length <= threshold)
                {
                    ecb.RemoveComponent<MoveTowardFiller>(entityInQueryIndex, entity);
                    ecb.AddComponent<FindBucket>(entityInQueryIndex, entity);
                }
                else
                {
                    var direction = distanceLeft / length;
                    var newPosition = scooperPosition + speed * deltaTime * direction;
                    cdfe[entity] = new Translation() { Value = newPosition };
                    newPosition.y += 1.6f;
                    cdfe[bucket.Target] = new Translation() { Value = newPosition };
                }
            }).ScheduleParallel();



        sys.AddJobHandleForProducer(Dependency);
    }
}
