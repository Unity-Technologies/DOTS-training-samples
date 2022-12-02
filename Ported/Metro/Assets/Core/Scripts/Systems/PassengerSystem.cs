using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct PassengerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<TrainPositionsBuffer>();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var trainPositions = SystemAPI.GetSingletonBuffer<TrainPositionsBuffer>();
        var platformTrainStatus = SystemAPI.GetSingletonBuffer<PlatformTrainStatusBuffer>();
        var trainCapacity = SystemAPI.GetSingletonBuffer<TrainCapacityBuffer>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var config = SystemAPI.GetSingleton<Config>();
        var platformsCount = config.PlatformCountPerStation * config.NumberOfStations;
        NativeArray<Path> paths = CollectionHelper.CreateNativeArray<Path>(platformsCount, state.WorldUpdateAllocator);
        NativeArray<float3> initWaypointTransforms = CollectionHelper.CreateNativeArray<float3>(platformsCount, state.WorldUpdateAllocator);

        CollectPathsJob collectPathsJob = new CollectPathsJob { paths = paths, initWaypointTransforms = initWaypointTransforms };
        var collectPathsHandle = collectPathsJob.ScheduleParallel(state.Dependency);

        PassengerJob passengerJob = new PassengerJob();
        passengerJob.trainPositions = trainPositions;
        passengerJob.platformTrainStatus = platformTrainStatus;
        passengerJob.paths = paths;
        passengerJob.trainCapacity = trainCapacity;
        passengerJob.initWaypointTransforms = initWaypointTransforms;
        passengerJob.ecb = ecb.AsParallelWriter();
        var passengerJobHandle = passengerJob.ScheduleParallel(collectPathsHandle);
        // passengerJobHandle = agentQuery.AddDependency(passengerJobHandle);

        ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        EmbarkJob embarkJob = new EmbarkJob();
        embarkJob.trainPositions = trainPositions;
        embarkJob.platformTrainStatus = platformTrainStatus;
        embarkJob.trainCapacity = trainCapacity;
        embarkJob.deltaTime = UnityEngine.Time.deltaTime;
        embarkJob.ecb = ecb.AsParallelWriter();
        var handle = embarkJob.ScheduleParallel(passengerJobHandle);

        state.Dependency = handle;
    }

    [BurstCompile]
    public partial struct PassengerJob : IJobEntity
    {
        [ReadOnly] public DynamicBuffer<TrainPositionsBuffer> trainPositions;
        [ReadOnly] public DynamicBuffer<PlatformTrainStatusBuffer> platformTrainStatus;
        [ReadOnly] public NativeArray<Path> paths;
        [ReadOnly] public NativeArray<float3> initWaypointTransforms;
        [NativeDisableContainerSafetyRestriction] public DynamicBuffer<TrainCapacityBuffer> trainCapacity;
        public EntityCommandBuffer.ParallelWriter ecb;

        public void Execute([ChunkIndexInQuery]int sortKey, Entity entity, TransformAspect transform, LocationInfo locationInfo, PassengerInfo passengerInfo, ref IdleTime idleTime, ref Agent agent, ref TargetPosition target)
        {
            if (passengerInfo.TrainID != -1)
            {
                transform.LocalPosition = new float3(trainPositions[passengerInfo.TrainID].position.x, trainPositions[passengerInfo.TrainID].position.y, trainPositions[passengerInfo.TrainID].position.z - Globals.TrainHalfLength + passengerInfo.Seat * Globals.TrainSeatSpacing);

                if (locationInfo.CurrentPlatform != passengerInfo.EmbarkedPlatform && platformTrainStatus[locationInfo.CurrentPlatform].TrainID != -1) // possible bug here but I don't think its possible to happen with 1 train
                {
                    ecb.RemoveComponent<PassengerInfo>(sortKey, entity);
                    var trainID = platformTrainStatus[locationInfo.CurrentPlatform].TrainID;
                    trainCapacity[trainID] = new TrainCapacityBuffer() { Capacity = (trainCapacity[trainID].Capacity + 1) };
                    idleTime.Value = Globals.TrainWaitTime * 2;
                    if (locationInfo.CurrentPlatform >= 0)
                    {
                        var path = paths[locationInfo.CurrentPlatform];
                        var defaultWaypointPos = initWaypointTransforms[locationInfo.CurrentPlatform];

                        agent.CurrentWaypoint = path.Default;
                        target.Value = defaultWaypointPos;
                    }
                }
            }
        }
    }

    [BurstCompile][WithNone(typeof(PassengerInfo), typeof(TrainInfo))]
    public partial struct EmbarkJob : IJobEntity
    {
        [ReadOnly] public DynamicBuffer<TrainPositionsBuffer> trainPositions;
        [ReadOnly] public DynamicBuffer<PlatformTrainStatusBuffer> platformTrainStatus;
        [NativeDisableContainerSafetyRestriction] public DynamicBuffer<TrainCapacityBuffer> trainCapacity;
        public EntityCommandBuffer.ParallelWriter ecb;
        [ReadOnly] public float deltaTime;

        public void Execute([ChunkIndexInQuery] int sortKey, Entity entity, TransformAspect transform, LocationInfo locationInfo, ref IdleTime idleTime)
        {
            var trainID = platformTrainStatus[locationInfo.CurrentPlatform].TrainID;
            if (trainID == -1) return;

            if (idleTime.Value > 0)
                idleTime.Value -= deltaTime;

            if (trainCapacity[trainID].Capacity > 0 && idleTime.Value <= 0)
            {
                PassengerInfo passengerInfo = new PassengerInfo() { TrainID = trainID, Seat = trainCapacity[trainID].Capacity + 1, EmbarkedPlatform = locationInfo.CurrentPlatform };
                transform.LocalPosition = new float3(trainPositions[trainID].position.x, trainPositions[trainID].position.y, trainPositions[trainID].position.z - Globals.TrainHalfLength + passengerInfo.Seat * Globals.TrainSeatSpacing);
                trainCapacity[trainID] = new TrainCapacityBuffer() { Capacity = (trainCapacity[trainID].Capacity - 1) };
                ecb.AddComponent(sortKey, entity, passengerInfo);
            }
        }
    }

    [BurstCompile]
    public partial struct CollectPathsJob : IJobEntity
    {
        [NativeDisableContainerSafetyRestriction] public NativeArray<Path> paths;
        [NativeDisableContainerSafetyRestriction] public NativeArray<float3> initWaypointTransforms;
        
        public void Execute(in Path path, in PathID pathID, in WorldTransform transform)
        {
            paths[pathID.Value] = path;
            initWaypointTransforms[pathID.Value] = transform.Position;
        }
    }
    
    [BurstCompile]
    public partial struct DebarkJob : IJobEntity
    {
        [ReadOnly] public NativeArray<Path> paths;
        [ReadOnly] public NativeArray<float3> initWaypointTransforms;

        public void Execute(ref Agent agent, ref TargetPosition target, in LocationInfo info)
        {
            if (info.CurrentPlatform >= 0)
            {
                var path = paths[info.CurrentPlatform];
                var defaultWaypointPos = initWaypointTransforms[info.CurrentPlatform];

                agent.CurrentWaypoint = path.Default;
                target.Value = defaultWaypointPos;
            }
        }
    }
}