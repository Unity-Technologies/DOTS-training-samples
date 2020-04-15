using Unity.Collections;
using  Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ActorMovementSystem : SystemBase
{
    private EntityQuery mActorsToMove;


    protected override void OnCreate()
    {
        base.OnCreate();
        mActorsToMove = GetEntityQuery(ComponentType.ReadOnly<Actor>(), ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Destination>());
    }

    protected override void OnUpdate()
    {
        var tuningData = GetSingleton<TuningData>();
        var deltaTime = Time.DeltaTime;

        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithName("Actor_Movement")
            .WithAll<Actor>()
            .ForEach((Entity actorEntity, ref Translation translation, in Destination dest) =>
            {
                float3 delta = dest.position - translation.Value;
                float maxTravelDistance = deltaTime * tuningData.ActorSpeed;
                float distance = deltaTime * math.length(delta);
                distance = math.min(distance, maxTravelDistance);

                translation.Value = distance * math.normalize(delta) + translation.Value;
                if (distance < 1e-2)
                {
                    ecb.RemoveComponent<Destination>(actorEntity);
                }
            }).Run();

        var getActorPosition = GetComponentDataFromEntity<Translation>(true);
        Entities
            .WithAll<Bucket>()
            .ForEach((Entity Bucket, ref Translation bucketTranslation, in HeldBy heldBy) =>
            {
                var actorPosition = getActorPosition[heldBy.holder];
                bucketTranslation.Value = actorPosition.Value = math.up() * .5f;
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}