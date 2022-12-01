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

        PassengerJob passengerJob = new PassengerJob();
        passengerJob.trainPositions = trainPositions;
        passengerJob.ScheduleParallel();

        EmbarkJob embarkJob = new EmbarkJob();
        embarkJob.trainPositions = trainPositions;
        embarkJob.platformTrainStatus = platformTrainStatus;
        embarkJob.trainCapacity = trainCapacity;
        embarkJob.ecb = ecb;
        embarkJob.ScheduleParallel();
        /*     foreach (var transform in SystemAPI.Query<TransformAspect>().WithAll<PassengerTag>())
             {
                 transform.LocalPosition = new float3(transform.LocalPosition.x, transform.LocalPosition.y, trainPositions[0].positionZ);
             }*/
    }

    [BurstCompile]
    public partial struct PassengerJob : IJobEntity
    {
        [ReadOnly]public DynamicBuffer<TrainPositionsBuffer> trainPositions;

        public void Execute(TransformAspect transform, LocationInfo locationInfo, PassengerInfo passengerInfo)
        {
            if (passengerInfo.TrainID != -1)
                transform.LocalPosition = new float3(trainPositions[passengerInfo.TrainID].position.x, trainPositions[passengerInfo.TrainID].position.y, trainPositions[passengerInfo.TrainID].position.z);
        }
    }

    [BurstCompile][WithNone(typeof(PassengerInfo))]
    public partial struct EmbarkJob : IJobEntity
    {
        [ReadOnly] public DynamicBuffer<TrainPositionsBuffer> trainPositions;
        [ReadOnly] public DynamicBuffer<PlatformTrainStatusBuffer> platformTrainStatus;
        [NativeDisableContainerSafetyRestriction] public DynamicBuffer<TrainCapacityBuffer> trainCapacity;
        [NativeDisableContainerSafetyRestriction] public EntityCommandBuffer ecb;

        public void Execute(Entity entity, TransformAspect transform, LocationInfo locationInfo)
        {
            var trainID = platformTrainStatus[locationInfo.CurrentPlatform].TrainID;
            if (trainID == -1) return;

            if (trainCapacity[trainID].Capacity > 0)
            {
                transform.LocalPosition = new float3(trainPositions[platformTrainStatus[locationInfo.CurrentPlatform].TrainID].position.x, trainPositions[platformTrainStatus[locationInfo.CurrentPlatform].TrainID].position.y, trainPositions[platformTrainStatus[locationInfo.CurrentPlatform].TrainID].position.z);
                trainCapacity[trainID] = new TrainCapacityBuffer() { Capacity = trainCapacity[trainID].Capacity - 1 };
                PassengerInfo info = new PassengerInfo() { TrainID = trainID };
                ecb.AddComponent(entity, info);
            }
        }
    }


}