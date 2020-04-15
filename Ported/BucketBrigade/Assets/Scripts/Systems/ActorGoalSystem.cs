using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ActorGoalSystem : SystemBase
{
    private Random mRand;
    private EntityQuery mAllBucketsNotBeingHeld;
    private EntityQuery mAllActorsWithoutDestination;
    private EndSimulationEntityCommandBufferSystem mEndSimCommandBufferSystem;

    protected override void OnCreate()
    {
        mAllActorsWithoutDestination = GetEntityQuery(ComponentType.ReadWrite<Actor>(), ComponentType.Exclude<Destination>());
        mAllBucketsNotBeingHeld = GetEntityQuery(ComponentType.ReadWrite<Bucket>(), ComponentType.Exclude<HeldBy>());
        mEndSimCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        mRand = new Random(1337);
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        var actorPerformActionBuffer = new EntityCommandBuffer(Allocator.TempJob);
        var actorCreateGoalBuffer = mEndSimCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        var rand = mRand;

        var getBucketComponent = GetComponentDataFromEntity<Bucket>();
        var getRiverComponent = GetComponentDataFromEntity<River>();

        Entities
            .WithName("Actor_Perform_Action")
            .WithAll<Actor>()
            .WithNone<Destination>()
            .ForEach((Entity actor, in TargetEntity target) => {

                //TODO: If target is a river, get water, if fire dump water, if another actor, send them a bucket

                if (getBucketComponent.Exists(target.target))
                {

                }

                actorPerformActionBuffer.RemoveComponent<TargetEntity>(actor);
            }).Run();

        actorPerformActionBuffer.Playback(EntityManager);
        actorPerformActionBuffer.Dispose();

        int numAvailableBuckets = mAllBucketsNotBeingHeld.CalculateEntityCount();
        if (numAvailableBuckets > 0)
        {
            var bucketEntities = mAllBucketsNotBeingHeld.ToEntityArrayAsync(Allocator.TempJob, out var getBucketEntitiesHandle);
            var countOfActorsWithoutDestination = mAllActorsWithoutDestination.CalculateEntityCount();
            var getBucketPositon = GetComponentDataFromEntity<Translation>(true);

            Dependency = JobHandle.CombineDependencies(Dependency, getBucketEntitiesHandle);

            Entities
                .WithName("Actor_Find_Bucket")
                .WithAll<Actor>()
                .WithNone<Destination, HoldingBucket>()
                .WithReadOnly(getBucketPositon)
                .WithDeallocateOnJobCompletion(bucketEntities)
                .ForEach((int entityInQueryIndex, Entity actor, in Translation currentPos) =>
                {
                    if (entityInQueryIndex >= bucketEntities.Length)
                    {
                        return;
                    }

                    actorCreateGoalBuffer.AddComponent(entityInQueryIndex, actor, new TargetEntity { target = bucketEntities[entityInQueryIndex] } );
                    actorCreateGoalBuffer.AddComponent(entityInQueryIndex, bucketEntities[entityInQueryIndex], new HeldBy() {holder = actor});
                    actorCreateGoalBuffer.AddComponent(entityInQueryIndex, actor, new Destination() {position = getBucketPositon[bucketEntities[entityInQueryIndex]].Value});
                }).ScheduleParallel();
        }
        else
        {
            Entities
                .WithName("Actor_Wander")
                .WithAll<Actor>()
                .WithNone<Destination, HoldingBucket>()
                .ForEach((int nativeThreadIndex, Entity actor, in Translation currentPos) =>
                {
                    //TODO: Find bucket
                    var randomCircle = (rand.NextFloat2() - 1) * 2;
                    randomCircle *= rand.NextFloat() * 5;
                    Destination dest = new Destination()
                    {
                        position = currentPos.Value + new float3(randomCircle.x, 0f, randomCircle.y)
                    };

                    actorCreateGoalBuffer.AddComponent(nativeThreadIndex, actor, dest);
                }).ScheduleParallel();
        }

        Entities
            .WithName("Actor_Find_Water")
            .WithAll<Actor, HoldingBucket>()
            .WithNone<Destination>()
            .ForEach((int nativeThreadIndex, Entity actor, in Translation currentPos) =>
            {
                //TODO: Find water destination
                var randomCircle = (rand.NextFloat2() - 1) * 2;
                randomCircle *= rand.NextFloat() * 5;
                Destination dest = new Destination()
                {
                    position = currentPos.Value + new float3(randomCircle.x, 0f, randomCircle.y)
                };

                actorCreateGoalBuffer.AddComponent(nativeThreadIndex, actor, dest);
            }).ScheduleParallel();

        Entities
            .WithName("Actor_Find_Fire")
            .WithAll<Actor, HoldingBucket>()
            .WithNone<Destination>()
            .ForEach((int nativeThreadIndex, Entity actor, in Translation currentPos) =>
            {
                //TODO: Find fire destination
                var randomCircle = (rand.NextFloat2() - 1) * 2;
                randomCircle *= rand.NextFloat() * 5;
                Destination dest = new Destination()
                {
                    position = currentPos.Value + new float3(randomCircle.x, 0f, randomCircle.y)
                };

                actorCreateGoalBuffer.AddComponent(nativeThreadIndex, actor, dest);
            }).ScheduleParallel();

        mRand = rand;
        mEndSimCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
