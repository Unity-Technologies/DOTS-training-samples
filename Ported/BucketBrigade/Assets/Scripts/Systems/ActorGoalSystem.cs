using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Random = Unity.Mathematics.Random;

public class ActorGoalSystem : SystemBase
{
    private Random mRand;
    private EntityQuery mAllBucketsNotBeingHeld;
    private EntityQuery mAllActorsWithBucketAndNoDestination;

    private EntityQuery mAllFires;

    private EntityQuery mAllRiversQuery;

    private EndSimulationEntityCommandBufferSystem mEndSimCommandBufferSystem;

    protected override void OnCreate()
    {
        GetEntityQuery(ComponentType.ReadWrite<Actor>(), ComponentType.Exclude<Destination>());

        mAllBucketsNotBeingHeld = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<Bucket>(),
                                                                                ComponentType.Exclude<HeldBy>());

        mAllActorsWithBucketAndNoDestination = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<Actor>(),
                                                                                ComponentType.ReadWrite<HoldingBucket>(),
                                                                                ComponentType.Exclude<Destination>());

        mAllRiversQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<River>(),
                                                                                ComponentType.ReadWrite<Translation>(),
                                                                                ComponentType.ReadWrite<ValueComponent>());

        mAllFires = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Fire>(),
                                                                            ComponentType.ReadOnly<ValueComponent>(),
                                                                            ComponentType.ReadOnly<Translation>());


        mEndSimCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        mRand = new Random(1337);
        base.OnCreate();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        mAllBucketsNotBeingHeld.Dispose();;
        mAllActorsWithBucketAndNoDestination.Dispose();
        mAllRiversQuery.Dispose();

        mAllFires.Dispose();
    }

    protected override void OnUpdate()
    {
        if (!HasSingleton<TuningData>())
        {
            return;
        }
        var tuningData = GetSingleton<TuningData>();

        var rand = mRand;

        var actorPerformActionBuffer = new EntityCommandBuffer(Allocator.TempJob);
        var actorCreateGoalBuffer = mEndSimCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        var getFireComponent = GetComponentDataFromEntity<Fire>(true);
        var getActorComponent = GetComponentDataFromEntity<Actor>(true);
        var getValueComponent = GetComponentDataFromEntity<ValueComponent>();
        var getTranslationComponent = GetComponentDataFromEntity<Translation>(true);
        var getLineComponent = GetComponentDataFromEntity<LineComponent>(true);

        var getHeldBucket = GetComponentDataFromEntity<HoldingBucket>(true);

        //Synchronous, low freq call

        Entities
            .WithName("Any_Pass_Bucket_Action")
            .WithoutBurst()
            .WithNone<Destination, ThrowerTag>() //Thrower required because it handles its own bucket actions
            .WithNativeDisableContainerSafetyRestriction(getHeldBucket)
            .ForEach((Entity actor, in TargetEntity target, in HoldingBucket holdingBucket) =>
            {
                var nextActor = target.target;

                if(!getHeldBucket.Exists(nextActor)) //Wait until the recipient no longer has a bucket
                {
                    var buck = holdingBucket.bucket;
                    actorPerformActionBuffer.RemoveComponent<HoldingBucket>(actor);
                    actorPerformActionBuffer.AddComponent<HoldingBucket>(nextActor, new HoldingBucket() { bucket = buck });
                    actorPerformActionBuffer.RemoveComponent<TargetEntity>(actor);
                    actorPerformActionBuffer.SetComponent(buck, new HeldBy() { holder = nextActor });
                }
            }).Run();

        Entities
            .WithName("Thrower_Perform_Action")
            .WithAll<Actor>()
            .WithNone<Destination>()
            .ForEach((Entity actor, in ThrowerTag thrower, in TargetEntity target, in HoldingBucket bucket) => {

                var targetEntity = target.target;
                if (getFireComponent.Exists(targetEntity))
                {
                    var bucketValue = getValueComponent[bucket.bucket];
                    getValueComponent[bucket.bucket] = new ValueComponent() {Value = 0};

                    var fireValue = getValueComponent[targetEntity];
                    fireValue = new ValueComponent() {Value = (byte) math.max(0, fireValue.Value - bucketValue.Value)};
                    getValueComponent[targetEntity] = fireValue;

                    if (fireValue.Value <= 0f)
                    {
                        actorPerformActionBuffer.RemoveComponent<LineComponent>(thrower.brigade);
                    }
                }
                else if (getActorComponent.Exists(targetEntity))
                {
                    actorPerformActionBuffer.AddComponent<HoldingBucket>(targetEntity, new HoldingBucket() {bucket = bucket.bucket});
                    actorPerformActionBuffer.RemoveComponent<HoldingBucket>(actor);
                    actorPerformActionBuffer.SetComponent(bucket.bucket, new HeldBy() {holder = targetEntity});
                }

                actorPerformActionBuffer.RemoveComponent<TargetEntity>(actor);
            }).Run();

        Entities
            .WithName("Scooper_Pickup_Bucket")
            .WithAll<ScooperTag>()
            .WithNone<Destination, HoldingBucket>()
            .WithReadOnly(getTranslationComponent)
            .ForEach((Entity actor, ref TargetEntity target, in Actor actorData) =>
            {
                var buck = target.target;
                actorPerformActionBuffer.AddComponent<HoldingBucket>(actor, new HoldingBucket() {bucket = buck});
                actorPerformActionBuffer.AddComponent<HeldBy>(buck, new HeldBy(){holder = actor});

                target.target = actorData.neighbor;
                actorPerformActionBuffer.AddComponent<Destination>(actor, new Destination() {position = getTranslationComponent[actorData.neighbor].Value});

            }).Run();

        Entities
            .WithName("Filler_Gets_Water")
            .WithAll<FillerTag>()
            .WithReadOnly(getTranslationComponent)
            .WithNone<Destination, TargetEntity>()
            .ForEach((Entity actor, in HoldingBucket bucket, in Actor actorData) =>
        {
            //Debug.Log("filling water");
            var bucketValue = getValueComponent[bucket.bucket];
            getValueComponent[bucket.bucket] = new ValueComponent() {Value = (byte) (bucketValue.Value + (byte) tuningData.BucketCapacity)};

            actorPerformActionBuffer.AddComponent<Destination>(actor, new Destination() {position = getTranslationComponent[actorData.neighbor].Value});
            actorPerformActionBuffer.AddComponent<TargetEntity>(actor, new TargetEntity() {target = actorData.neighbor});

        }).Run();

        Entities
            .WithName("Thrower_Has_Bucket")
            .WithNone<Destination, TargetEntity>()
            .WithReadOnly(getLineComponent)
            .WithReadOnly(getTranslationComponent)
            .ForEach((Entity actor, in Actor actorData, in ThrowerTag thrower, in HoldingBucket bucket) =>
            {
                var bucketValue = getValueComponent[bucket.bucket];
                if (bucketValue.Value > 0)
                {
                    //Go to fire
                    var lineComp = getLineComponent[thrower.brigade];
                    actorPerformActionBuffer.AddComponent<Destination>(actor, new Destination() {position = getTranslationComponent[lineComp.end].Value});
                    actorPerformActionBuffer.AddComponent<TargetEntity>(actor, new TargetEntity() {target = lineComp.end});
                }
                else
                {
                    //Go to neighbor
                    actorPerformActionBuffer.AddComponent<Destination>(actor, new Destination() {position =getTranslationComponent[actorData.neighbor].Value});
                    actorPerformActionBuffer.AddComponent<TargetEntity>(actor, new TargetEntity() {target = actorData.neighbor});
                }
            }).Run();

        Entities
            .WithName("Actors_Find_Bucket_Target")
            .WithNone<FillerTag, ScooperTag, ThrowerTag>()
            .WithNone<Destination, TargetEntity>()
            .WithAll<HoldingBucket>()
            .ForEach((Entity actor, in Actor actorData) =>
            {
                //Debug.Log("regular folk finding friend");
                actorPerformActionBuffer.AddComponent<Destination>(actor, new Destination() {position = GetComponent<Translation>(actorData.neighbor).Value});
                actorPerformActionBuffer.AddComponent<TargetEntity>(actor, new TargetEntity() {target = actorData.neighbor});

            }).Run();

        actorPerformActionBuffer.Playback(EntityManager);
        actorPerformActionBuffer.Dispose();

        int numAvailableBuckets = mAllBucketsNotBeingHeld.CalculateChunkCount();
        if (numAvailableBuckets > 0)
        {
            var getPositions = GetComponentDataFromEntity<Translation>(true);

            var bucketEntities = mAllBucketsNotBeingHeld.ToEntityArrayAsync(Allocator.TempJob, out var getBucketEntitiesHandle);
            Dependency = JobHandle.CombineDependencies(Dependency, getBucketEntitiesHandle);

            //Async, low freq action
            Entities
                .WithName("Scooper_Find_Bucket")
                .WithAll<Actor, ScooperTag>()
                .WithNone<Destination, HoldingBucket, TargetEntity>()
                .WithReadOnly(getPositions)
                .WithDeallocateOnJobCompletion(bucketEntities)
                .ForEach((int entityInQueryIndex, Entity actor, in Translation currentPos) =>
                {
                    if (entityInQueryIndex >= bucketEntities.Length)
                    {
                        return;
                    }
                    //Debug.Log("in Scooper_Find_Bucket");

                    actorCreateGoalBuffer.AddComponent(entityInQueryIndex, actor, new TargetEntity { target = bucketEntities[entityInQueryIndex] } );
                    actorCreateGoalBuffer.AddComponent(entityInQueryIndex, bucketEntities[entityInQueryIndex], new HeldBy() {holder = actor});
                    actorCreateGoalBuffer.AddComponent(entityInQueryIndex, actor, new Destination() {position = getPositions[bucketEntities[entityInQueryIndex]].Value});
                }).Schedule();
        }

        mRand = rand;
        mEndSimCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
