using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveTowardBucketSystem : SystemBase
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
            .ForEach((Entity entity, int entityInQueryIndex, in MoveTowardBucket scooper) =>
        {
            var bucketPosition = cdfe[scooper.Target].Value;
            var scooperPosition = cdfe[entity].Value;

            var distanceLeft = bucketPosition - scooperPosition;
            var length = math.length(distanceLeft);

            if (length <= threshold)
            {
                ecb.RemoveComponent<MoveTowardBucket>(entityInQueryIndex, entity);
                ecb.AddComponent<FindWaterCell>(entityInQueryIndex, entity);
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
            .ForEach((Entity entity, int entityInQueryIndex, in MoveTowardWater scooper) =>
            {
                var waterPosition = cdfe[scooper.Target].Value;
                var scooperPosition = cdfe[entity].Value;

                var distanceLeft = waterPosition - scooperPosition;
                var length = math.length(distanceLeft);

                if (length <= threshold)
                {
                    ecb.RemoveComponent<MoveTowardWater>(entityInQueryIndex, entity);
                    ecb.AddComponent<MoveTowardBucket>(entityInQueryIndex, entity);
                }
                else
                {
                    var direction = distanceLeft / length;
                    var newPosition = scooperPosition + speed * deltaTime * direction;
                    cdfe[entity] = new Translation() { Value = newPosition };
                }
        }).ScheduleParallel();


        sys.AddJobHandleForProducer(Dependency);
    }
}
