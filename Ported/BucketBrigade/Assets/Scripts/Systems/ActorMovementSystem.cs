using Unity.Collections;
using  Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ActorMovementSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem mEndSimBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        GetEntityQuery(ComponentType.ReadOnly<Actor>(), ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Destination>());
        mEndSimBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        if (!HasSingleton<TuningData>())
        {
            return;
        }
        var tuningData = GetSingleton<TuningData>();
        var deltaTime = Time.DeltaTime;

        var ecb = mEndSimBufferSystem.CreateCommandBuffer().ToConcurrent();
        Entities
            .WithName("Actor_Movement")
            .WithAll<Actor>()
            .ForEach((int entityInQueryIndex, Entity actorEntity, ref Translation translation, in Destination dest) =>
            {
                float3 delta = dest.position - translation.Value;
                float remainingDistance = math.length(delta);
                float maxTravelDistance = deltaTime * tuningData.ActorSpeed;

                if (maxTravelDistance >= remainingDistance)
                {
                    translation.Value = dest.position;
                    ecb.RemoveComponent<Destination>(entityInQueryIndex, actorEntity);
                }
                else
                {
                    translation.Value = maxTravelDistance * math.normalize(delta) + translation.Value;
                }
            }).ScheduleParallel();

        var getActorPosition = GetComponentDataFromEntity<Translation>(true);
        var getActorHolding = GetComponentDataFromEntity<HoldingBucket>();
        Entities
            .WithAll<Bucket>()
            .WithReadOnly(getActorPosition)
            .WithNativeDisableContainerSafetyRestriction(getActorPosition)
            .ForEach((Entity Bucket, ref Translation bucketTranslation, in HeldBy heldBy) =>
            {
                if (!getActorHolding.Exists(heldBy.holder))
                {
                    return;
                }

                var actorPosition = getActorPosition[heldBy.holder];
                bucketTranslation = new Translation()  {
                    Value = actorPosition.Value + math.up() * .5f
                };
            }).Run();
        mEndSimBufferSystem.AddJobHandleForProducer(Dependency);
    }
}