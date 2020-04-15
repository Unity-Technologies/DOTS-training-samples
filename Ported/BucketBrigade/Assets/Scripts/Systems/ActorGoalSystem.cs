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
        var tuningData = GetSingleton<TuningData>();

        var rand = mRand;

        var actorPerformActionBuffer = new EntityCommandBuffer(Allocator.TempJob);
        var actorCreateGoalBuffer = mEndSimCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        var getFireComponent = GetComponentDataFromEntity<Fire>(true);
        var getBucketComponent = GetComponentDataFromEntity<Bucket>(true);
        var getRiverComponent = GetComponentDataFromEntity<River>(true);
        var getValueComponent = GetComponentDataFromEntity<ValueComponent>();
        var getHeldBucket = GetComponentDataFromEntity<HoldingBucket>(true);

        //Synchronous, low freq call
        Entities
            .WithName("Actor_Perform_Action")
            .WithAll<Actor>()
            .WithNone<Destination>()
            .ForEach((Entity actor, in TargetEntity target) => {

                //TODO: If target is a river, get water, if fire dump water, if another actor, send them a bucket

                var targetEntity = target.target;
                if (getBucketComponent.Exists(targetEntity))
                {
                    actorPerformActionBuffer.AddComponent<HoldingBucket>(actor, new HoldingBucket() {bucket = target.target});
                }
                else if (getRiverComponent.Exists(targetEntity))
                {
                    var bucket = getHeldBucket[actor];
                    var bucketValue = getValueComponent[bucket.bucket];
                    getValueComponent[bucket.bucket] = new ValueComponent() {Value = (byte) (bucketValue.Value + (byte) tuningData.BucketCapacity)};
                }
                else if (getFireComponent.Exists(targetEntity))
                {
                    var bucket = getHeldBucket[actor];
                    var bucketValue = getValueComponent[bucket.bucket];
                    getValueComponent[bucket.bucket] = new ValueComponent() {Value = 0};

                    var fireValue = getValueComponent[targetEntity];
                    getValueComponent[targetEntity] = new ValueComponent() {Value = (byte)math.max(0, fireValue.Value - bucketValue.Value)};
                }

                actorPerformActionBuffer.RemoveComponent<TargetEntity>(actor);
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
#if false //Wander logic when we can't decide what to do
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
                    randomCircle *= rand.NextFloat();
                    Destination dest = new Destination()
                    {
                        position = currentPos.Value + new float3(randomCircle.x, 0f, randomCircle.y)
                    };

                    actorCreateGoalBuffer.AddComponent(nativeThreadIndex, actor, dest);
                }).ScheduleParallel();
        }
#endif

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
                    if (getBucketValue[bucket.bucket].Value > 0)//TODO: Can we represent this in the query?
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

            var allFireEntities = mAllFires.ToEntityArrayAsync(Allocator.TempJob, out var allFireEntitiesHandle);
            var allFirePositions = mAllFires.ToComponentDataArrayAsync<Translation>(Allocator.TempJob, out var allFirePositionsHandle);
            var allFireValues = mAllFires.ToComponentDataArrayAsync<ValueComponent>(Allocator.TempJob, out var allFireValuesHandle);

            var combinedDeps = JobHandle.CombineDependencies(allFireEntitiesHandle, allFirePositionsHandle);
            combinedDeps = JobHandle.CombineDependencies(combinedDeps, allFireValuesHandle);
            Dependency = JobHandle.CombineDependencies(combinedDeps, Dependency);

            Entities
                .WithName("Actor_Find_Fire")
                .WithAll<Actor>()
                .WithNone<Destination, TargetEntity>()
                .WithReadOnly(getBucketValue)
                .WithDeallocateOnJobCompletion(allFireEntities)
                .WithDeallocateOnJobCompletion(allFirePositions)
                .WithDeallocateOnJobCompletion(allFireValues)
                .ForEach((int nativeThreadIndex, Entity actor, in HoldingBucket bucket, in Translation currentPos) =>
                {
                    if (getBucketValue[bucket.bucket].Value == 0) //TODO: Can we represent this in the query?
                    {
                        return;
                    }

                    float closestFire = float.MaxValue;
                    int closestFireEntity = -1;
                    float3 actorPosition = currentPos.Value;

                    for (int i = 0; i < allFireEntities.Length; ++i)
                    {
                        float distance = math.length(actorPosition - allFirePositions[i].Value);
                        if (distance < closestFire && allFireValues[i].Value > tuningData.ValueThreshold)
                        {
                            closestFireEntity = i;
                            closestFire = distance;
                        }
                    }

                    if (closestFireEntity >= 0)
                    {
                        actorCreateGoalBuffer.AddComponent(nativeThreadIndex, actor, new Destination() {position = allFirePositions[closestFireEntity].Value});
                        actorCreateGoalBuffer.AddComponent(nativeThreadIndex, actor, new TargetEntity() {target = allFireEntities[closestFireEntity]});
                    }
                }).ScheduleParallel();
        }

        mRand = rand;
        mEndSimCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
