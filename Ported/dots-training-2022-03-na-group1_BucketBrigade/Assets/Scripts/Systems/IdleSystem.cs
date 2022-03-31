using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using static BucketBrigadeUtility;

public partial class IdleSystem : SystemBase
{
    private static int _frame;

    public static int CurrentFrame => _frame;
    
    static float2 CalculateLeftArc(float2 a, float2 b, float t)
    {
        var ab = b - a;

        return a + (ab * t) + (new float2(-ab.y, ab.x) * ((1f - t) * t * 0.3f));
    }

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        var currentFrame = _frame;

        _frame += 1;
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var waterPoolBufferEntity = GetSingletonEntity<WaterPoolInfo>();
        var waterPoolBuffer = EntityManager.GetBuffer<WaterPoolInfo>(waterPoolBufferEntity);
        
        waterPoolBuffer.Clear();
        
        Entities.WithName("WaterInfoCollect")
            .WithNone<BucketTag>()
            .ForEach((in Entity entity, in Volume volume, in Position position, in Scale scale) =>
            {
                // water pool only counts if it has volume.
                if (volume.Value > 0.01)
                {
                    ecb.AppendToBuffer(waterPoolBufferEntity, new WaterPoolInfo() { WaterPool = entity, Position = position.Value, Radius = scale.Value.x / 2f});
                }
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        ecb = new EntityCommandBuffer(Allocator.Temp);
        
        waterPoolBuffer = EntityManager.GetBuffer<WaterPoolInfo>(waterPoolBufferEntity);

        var entityManager = EntityManager;

        Entities
            .WithName("DumpBucketOnFire")
            .WithAny<CaptainTag>()
            .ForEach((ref MyWorkerState state, ref BucketHeld bucketHeld, ref Speed speed, in Position position) =>
            {
                if (state.Value == WorkerState.Idle && bucketHeld.Value != Entity.Null && bucketHeld.IsFull)
                {
                    // splash at position
                    MarkCarriedBucketAsEmpty(ecb, ref bucketHeld, ref speed, currentFrame);
                }
            }).Run();
        
        Entities
            .WithName("DropBucketAtTargetFullOnly")
            .WithAny<FetcherTag, WorkerTag>()
            .ForEach((ref MyWorkerState state, ref RelocatePosition destination, ref BucketHeld bucketHeld, 
                ref Speed speed, in Position position, in DestinationWorker target) =>
            {
                if (state.Value == WorkerState.Idle && bucketHeld.Value != Entity.Null && bucketHeld.IsFull)
                {
                    if (target.Value != Entity.Null)
                    {
                        var targetPosition = entityManager.GetComponentData<Position>(target.Value);

                        if (IsVeryClose(position.Value, targetPosition.Value))
                        {
                            GiveBucketByDrop(ecb, target, ref bucketHeld, ref speed, currentFrame);
                        }
                        else
                        {
                            destination.Value = targetPosition.Value;
                        }
                    }
                    else
                    {
                        DropBucket(ecb, ref bucketHeld, ref speed, currentFrame);
                    }
                }
            }).Run();

        Entities
            .WithName("DropBucketAtTargetEmptyOnly")
            .WithAny<CaptainTag, WorkerTag>()
            .ForEach((ref MyWorkerState state, ref RelocatePosition destination, ref BucketHeld bucketHeld, 
                ref Speed speed, in Position position, in DestinationWorker target) =>
            {
                if (state.Value == WorkerState.Idle && bucketHeld.Value != Entity.Null &&
                    !bucketHeld.IsFull)
                {
                    if (target.Value != Entity.Null)
                    {
                        var targetPosition = entityManager.GetComponentData<Position>(target.Value);

                        if (IsVeryClose(position.Value, targetPosition.Value))
                        {
                            GiveBucketByDrop(ecb, target, ref bucketHeld, ref speed, currentFrame);
                        }
                        else
                        {
                            destination.Value = targetPosition.Value;
                        }
                    }
                    else
                    {
                        DropBucket(ecb, ref bucketHeld, ref speed, currentFrame);
                    }
                }
            }).Run();
        
        Entities
            .WithName("FindWaterSourceAndFillBucket")
            .WithAny<FetcherTag, OmniworkerTag>()
            .WithReadOnly(waterPoolBuffer)
            .ForEach((ref MyWorkerState state, ref RelocatePosition destination, ref BucketHeld bucketHeld, 
                ref MyWaterPool myWaterPool, ref Home home, ref Speed speed, in Position position, in MyTeam team) =>
            {
                if (state.Value == WorkerState.Idle && bucketHeld.Value != Entity.Null && !bucketHeld.IsFull)
                {
                    // goto water source
                    (var waterPool, var waterPosition) = FindClosestWater(position.Value, waterPoolBuffer);

                    if (waterPool != Entity.Null)
                    {
                        if (!IsVeryClose(home.Value, waterPosition))
                        {
                            home.Value = waterPosition;
                            ecb.SetComponent(team.Value, new TeamNeedsReform() { Value = true });
                        }

                        if (IsVeryClose(position.Value, waterPosition))
                        {
                            myWaterPool.Value = waterPool;
                            MarkCarriedBucketAsFull(ecb, ref bucketHeld, ref speed, currentFrame);
                            //state.Value = WorkerState.FillingBucket; // TODO
                        }
                        else
                        {
                            destination.Value = waterPosition;
                        }
                    }
                }
            }).Run();

        Entities
            .WithName("FindBucket")
            .WithAny<FetcherTag, OmniworkerTag>()
            .ForEach((ref MyWorkerState state, in BucketHeld bucketHeld) =>
            {
                if (state.Value == WorkerState.Idle && bucketHeld.Value == Entity.Null)
                {
                    state.Value = WorkerState.BucketDetection;
                }
            }).Run();

        Entities
            .WithName("GoHome")
            .WithAny<WorkerTag, CaptainTag>()
            .ForEach((ref RelocatePosition target, in MyWorkerState state, in Position position, in Home home, in BucketHeld bucketHeld) =>
            {
                if (state.Value == WorkerState.Idle && bucketHeld.Value == Entity.Null && !IsVeryClose(position.Value, home.Value))
                {
                    target.Value = home.Value;
                }
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
        ecb = new EntityCommandBuffer(Allocator.Temp);
        
        Entities
            .WithName("BucketPickUp")
            .ForEach((ref BucketHeld bucketHeld, ref BucketToWant bucketWanted, ref Speed speed, in Position position) =>
            {
                if (bucketWanted.Value != Entity.Null)
                {
                    if (bucketHeld.Value == Entity.Null && IsEntityVeryClose(entityManager, position.Value, bucketWanted.Value))
                    {
                        PickUpBucket(entityManager, ref bucketHeld, ref bucketWanted, ref speed, currentFrame);
                    }
                    
                    bucketWanted.Value = Entity.Null;
                }
            }).Run();

        Entities.WithName("ReformTeam")
            .ForEach((ref TeamNeedsReform teamNeedsReform, in DynamicBuffer<Member> members, in TeamInfo teamInfo) =>
            {
                if (teamNeedsReform.Value)
                {
                    var captainHome = entityManager.GetComponentData<Home>(teamInfo.Captain);
                    var fetcherHome = entityManager.GetComponentData<Home>(teamInfo.Fetcher);

                    var outMembers = members.Length / 2;

                    var deltaT = 1f / (outMembers + 1);
                    var t = deltaT;

                    for (var i = 0; i < outMembers; i++)
                    {
                        var newHome = CalculateLeftArc(fetcherHome.Value, captainHome.Value, t);
                        ecb.SetComponent(members[i], new Home() { Value = newHome });
                        t += deltaT;
                    }

                    deltaT = 1f / (members.Length - outMembers + 1);
                    t = deltaT;

                    for (var i = outMembers; i < members.Length; i++)
                    {
                        var newHome = CalculateLeftArc(captainHome.Value, fetcherHome.Value, t);
                        ecb.SetComponent(members[i], new Home() { Value = newHome });
                        t += deltaT;
                    }

                    teamNeedsReform.Value = false;
                }
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
