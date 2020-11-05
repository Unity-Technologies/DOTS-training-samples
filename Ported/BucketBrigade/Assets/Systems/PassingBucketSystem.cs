using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class PassingBucketSystem : SystemBase
{
    public static EntityQuery buckets;

    protected override void OnCreate()
    {
        base.OnCreate();
        buckets = GetEntityQuery(ComponentType.ReadOnly<EmptyBucket>(), ComponentType.ReadOnly<Translation>());
    }

    protected override void OnUpdate()
    {

        Entities
            .ForEach((Entity entity, ref Translation translation, in FillerBot fillerBot) =>
            {
                var fillerPosition = translation;

                //if (filler)
            }).ScheduleParallel();


        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var cdfe = GetComponentDataFromEntity<Translation>();

        Entities
            .ForEach((Entity entity, ref Translation translation, in PasserBot bot, in HoldingBucket bucket) =>
            {
                var passerPosition = translation;
                var pickupPosition = bot.DropoffPosition;

                var distanceLeft = passerPosition.Value - pickupPosition;
                var length = math.length(distanceLeft);

                var threshold = 2.0f;
                var speed = 5.0f;
                var deltaTime = 0.5f;

                if (length <= threshold)
                {
                    ecb.RemoveComponent<HoldingBucket>(entity);
                }
                else
                {
                    var direction = distanceLeft / length;
                    var newPosition = passerPosition.Value + speed * deltaTime * direction;
                    cdfe[entity] = new Translation() { Value = newPosition };
                    cdfe[bucket.Target] = new Translation() { Value = newPosition };
                }

            }).ScheduleParallel();
    }
}

