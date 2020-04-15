using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Rendering;

public class ActorGoalSystem : SystemBase
{
    private Random mRand;
    private EntityQuery mAllBucketsNotBeingHeld;
    private EntityQuery mAllActorsWithBucketAndNoDestination;

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
    }

    protected override void OnUpdate()
    {
        var tuningData = GetSingleton<TuningData>();

        var rand = mRand;

        var actorPerformActionBuffer = new EntityCommandBuffer(Allocator.TempJob);
        var actorCreateGoalBuffer = mEndSimCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        var getBucketComponent = GetComponentDataFromEntity<Bucket>(true);
        var getRiverComponent = GetComponentDataFromEntity<River>(true);
        var getValueComponent = GetComponentDataFromEntity<ValueComponent>();
        var getHeldBucket = GetComponentDataFromEntity<HoldingBucket>(true);

        Entities
            .WithName("Actor_Perform_Action")
            .WithAll<Actor>()
            .WithNone<Destination>()
            .ForEach((Entity actor, in TargetEntity target) => {

                //TODO: If target is a river, get water, if fire dump water, if another actor, send them a bucket

                if (getBucketComponent.Exists(target.target))
                {
                    actorPerformActionBuffer.AddComponent<HoldingBucket>(actor, new HoldingBucket() {bucket = target.target});
                }
                else if (getRiverComponent.Exists(target.target))
                {
                    var bucket = getHeldBucket[actor];
                    var bucketValue = getValueComponent[bucket.bucket];
                    getValueComponent[bucket.bucket] = new ValueComponent() {Value = (byte) (bucketValue.Value + (byte) tuningData.BucketCapacity)};
                }

                actorPerformActionBuffer.RemoveComponent<TargetEntity>(actor);
            }).Run();

        actorPerformActionBuffer.Playback(EntityManager);
        actorPerformActionBuffer.Dispose();

        int numAvailableBuckets = mAllBucketsNotBeingHeld.CalculateEntityCount();
        var getPositions = GetComponentDataFromEntity<Translation>(true);
        if (numAvailableBuckets > 0)
        {
            var bucketEntities = mAllBucketsNotBeingHeld.ToEntityArrayAsync(Allocator.TempJob, out var getBucketEntitiesHandle);
            Dependency = JobHandle.CombineDependencies(Dependency, getBucketEntitiesHandle);

            Entities
                .WithName("Actor_Find_Bucket")
                .WithAll<Actor>()
                .WithNone<Destination, HoldingBucket, TargetEntity>()
                .WithReadOnly(getPositions)
                .WithDeallocateOnJobCompletion(bucketEntities)
                .ForEach((int entityInQueryIndex, Entity actor, in Translation currentPos) =>
                {
                    if (entityInQueryIndex >= bucketEntities.Length)
                    {
                        return;
                    }

                    actorCreateGoalBuffer.AddComponent(entityInQueryIndex, actor, new TargetEntity { target = bucketEntities[entityInQueryIndex] } );
                    actorCreateGoalBuffer.AddComponent(entityInQueryIndex, bucketEntities[entityInQueryIndex], new HeldBy() {holder = actor});
                    actorCreateGoalBuffer.AddComponent(entityInQueryIndex, actor, new Destination() {position = getPositions[bucketEntities[entityInQueryIndex]].Value});
                }).Schedule();
        }
        else
        {
            // Entities
            //     .WithName("Actor_Wander")
            //     .WithAll<Actor>()
            //     .WithNone<Destination, HoldingBucket>()
            //     .ForEach((int nativeThreadIndex, Entity actor, in Translation currentPos) =>
            //     {
            //         //TODO: Find bucket
            //         var randomCircle = (rand.NextFloat2() - 1) * 2;
            //         randomCircle *= rand.NextFloat();
            //         Destination dest = new Destination()
            //         {
            //             position = currentPos.Value + new float3(randomCircle.x, 0f, randomCircle.y)
            //         };
            //
            //         actorCreateGoalBuffer.AddComponent(nativeThreadIndex, actor, dest);
            //     }).ScheduleParallel();
        }

        if (mAllActorsWithBucketAndNoDestination.CalculateChunkCount() > 0)
        {
            var waterPositions = mAllRiversQuery.ToComponentDataArrayAsync<Translation>(Allocator.TempJob, out var waterPositionHandle);
            var waterEntities = mAllRiversQuery.ToEntityArrayAsync(Allocator.TempJob, out var waterEntityHandle);

            var findWaterJobHandle = JobHandle.CombineDependencies(waterPositionHandle, waterEntityHandle);

            Dependency = JobHandle.CombineDependencies(Dependency, findWaterJobHandle);

            var getBucketValue = GetComponentDataFromEntity<ValueComponent>(true);

            Entities
                .WithName("Actor_Find_Water")
                .WithAll<Actor>()
                .WithNone<Destination, TargetEntity>()
                .WithReadOnly(getBucketValue)
                .WithDeallocateOnJobCompletion(waterPositions)
                .WithDeallocateOnJobCompletion(waterEntities)
                .ForEach((int nativeThreadIndex, Entity actor, in HoldingBucket bucket, in Translation currentPos) =>
                {
                    if (getBucketValue[bucket.bucket].Value > 0)
                    {
                        return;
                    }
                    float closestWater = float.MaxValue;
                    int closestWaterEntity = -1;
                    float3 actorPosition = currentPos.Value;

                    for (int i = 0; i < waterEntities.Length; ++i)
                    {
                        float distance = math.length(actorPosition - waterPositions[i].Value);
                        if (distance < closestWater)
                        {
                            closestWaterEntity = i;
                            closestWater = distance;
                        }
                    }

                    actorCreateGoalBuffer.AddComponent(nativeThreadIndex, actor, new Destination() {position = waterPositions[closestWaterEntity].Value});
                    actorCreateGoalBuffer.AddComponent(nativeThreadIndex, actor, new TargetEntity() {target = waterEntities[closestWaterEntity]});
                }).ScheduleParallel();

            // Entities
            //     .WithName("Actor_Find_Fire")
            //     .WithAll<Actor>()
            //     .WithNone<Destination, TargetEntity>()
            //     .WithReadOnly(getBucketValue)
            //     .WithDeallocateOnJobCompletion(waterPositions)
            //     .WithDeallocateOnJobCompletion(waterEntities)
            //     .ForEach((int nativeThreadIndex, Entity actor, in HoldingBucket bucket, in Translation currentPos) =>
            //     {
            //         if (getBucketValue[bucket.bucket].Value == 0)
            //         {
            //             return;
            //         }
            //
            //         float closestWater = float.MaxValue;
            //         int closestWaterEntity = -1;
            //         float3 actorPosition = currentPos.Value;
            //
            //         for (int i = 0; i < waterEntities.Length; ++i)
            //         {
            //             float distance = math.length(actorPosition - waterPositions[i].Value);
            //             if (distance < closestWater)
            //             {
            //                 closestWaterEntity = i;
            //                 closestWater = distance;
            //             }
            //         }
            //
            //         actorCreateGoalBuffer.AddComponent(nativeThreadIndex, actor, new Destination() {position = waterPositions[closestWaterEntity].Value});
            //         actorCreateGoalBuffer.AddComponent(nativeThreadIndex, actor, new TargetEntity() {target = waterEntities[closestWaterEntity]});
            //     }).ScheduleParallel();
        }

        // Entities
        //     .WithName("Actor_Find_Fire")
        //     .WithAll<Actor, HoldingBucket>()
        //     .WithNone<Destination>()
        //     .ForEach((int nativeThreadIndex, Entity actor, in Translation currentPos) =>
        //     {
        //         //TODO: Find fire destination
        //         var randomCircle = (rand.NextFloat2() - 1) * 2;
        //         randomCircle *= rand.NextFloat() * 5;
        //         Destination dest = new Destination()
        //         {
        //             position = currentPos.Value + new float3(randomCircle.x, 0f, randomCircle.y)
        //         };
        //
        //         actorCreateGoalBuffer.AddComponent(nativeThreadIndex, actor, dest);
        //     }).Schedule();

        mRand = rand;
        mEndSimCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
