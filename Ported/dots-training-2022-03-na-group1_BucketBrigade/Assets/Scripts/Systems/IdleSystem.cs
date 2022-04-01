using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using static BucketBrigadeUtility;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class IdleSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        var teamReformBufferEntity = GetSingletonEntity<TeamReformCommand>();
        var bucketDropCommandEntity = GetSingletonEntity<DropBucketCommand>();
        var bucketPickupCommandEntity = GetSingletonEntity<PickupBucketCommand>();
        var dumpBucketCommandEntity = GetSingletonEntity<DumpBucketCommand>();
        var fillBucketCommandEntity = GetSingletonEntity<FillBucketCommand>();

        var waterPoolBufferEntity = GetSingletonEntity<WaterPoolInfo>();
        var waterPoolBuffer = EntityManager.GetBuffer<WaterPoolInfo>(waterPoolBufferEntity);

        var entityManager = EntityManager;

        Entities
            .WithName("DumpBucketOnFire")
            .WithAny<CaptainTag>()
            .ForEach((Entity entity, ref MyWorkerState state, ref BucketHeld bucketHeld, ref Speed speed, in Position position) =>
            {
                if (state.Value == WorkerState.Idle && bucketHeld.Value != Entity.Null && bucketHeld.IsFull)
                {
                    ecb.AppendToBuffer(dumpBucketCommandEntity, new DumpBucketCommand()
                    {
                        Worker = entity, 
                    });
                }
            }).Run();
        
        Entities
            .WithName("DropBucketAtTargetFullOnly")
            .WithAny<FetcherTag, WorkerTag>()
            .ForEach((Entity entity, ref MyWorkerState state, ref RelocatePosition destination, 
                in BucketHeld bucketHeld, in Position position, in DestinationWorker target) =>
            {
                if (state.Value == WorkerState.Idle && bucketHeld.Value != Entity.Null && bucketHeld.IsFull)
                {
                    if (target.Value != Entity.Null)
                    {
                        var targetPosition = entityManager.GetComponentData<Position>(target.Value);

                        if (IsVeryClose(position.Value, targetPosition.Value))
                        {
                            ecb.AppendToBuffer(bucketDropCommandEntity, new DropBucketCommand(entity));
                            ecb.AppendToBuffer(bucketPickupCommandEntity, new PickupBucketCommand(target.Value, bucketHeld.Value, 1));
                        }
                        else
                        {
                            destination.Value = targetPosition.Value;
                        }
                    }
                    else
                    {
                        ecb.AppendToBuffer(bucketDropCommandEntity, new DropBucketCommand(entity));
                    }
                }
            }).Run();

        Entities
            .WithName("DropBucketAtTargetEmptyOnly")
            .WithAny<CaptainTag, WorkerTag>()
            .ForEach((Entity entity, ref MyWorkerState state, ref RelocatePosition destination, 
                in BucketHeld bucketHeld, in Position position, in DestinationWorker target) =>
            {
                if (state.Value == WorkerState.Idle && bucketHeld.Value != Entity.Null &&
                    !bucketHeld.IsFull)
                {
                    if (target.Value != Entity.Null)
                    {
                        var targetPosition = entityManager.GetComponentData<Position>(target.Value);

                        if (IsVeryClose(position.Value, targetPosition.Value))
                        {
                            ecb.AppendToBuffer(bucketDropCommandEntity, new DropBucketCommand(entity));
                            ecb.AppendToBuffer(bucketPickupCommandEntity, new PickupBucketCommand(target.Value, bucketHeld.Value, 1));
                        }
                        else
                        {
                            destination.Value = targetPosition.Value;
                        }
                    }
                    else
                    {
                        ecb.AppendToBuffer(bucketDropCommandEntity, new DropBucketCommand(entity));
                    }
                }
            }).Run();
        
        Entities
            .WithName("FindWaterSourceAndFillBucket")
            .WithAny<FetcherTag, OmniworkerTag>()
            .WithReadOnly(waterPoolBuffer)
            .ForEach((Entity entity, ref MyWorkerState state, ref RelocatePosition destination, ref Home home, 
                in BucketHeld bucketHeld, in Position position, in MyTeam team) =>
            {
                if (state.Value == WorkerState.Idle && bucketHeld.Value != Entity.Null && !bucketHeld.IsFull)
                {
                    // goto water source
                    (var waterPool, var waterPosition) = FindClosestWater(home.Value, waterPoolBuffer);

                    if (waterPool != Entity.Null)
                    {
                        if (!IsVeryClose(home.Value, waterPosition))
                        {
                            home.Value = waterPosition;
                            destination.Value = waterPosition;
                            ecb.AppendToBuffer(teamReformBufferEntity, new TeamReformCommand() { Team = team.Value });
                        }
                        else if (IsVeryClose(position.Value, waterPosition))
                        {
                            ecb.AppendToBuffer(fillBucketCommandEntity, new FillBucketCommand(entity, waterPool));
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
    }
}
