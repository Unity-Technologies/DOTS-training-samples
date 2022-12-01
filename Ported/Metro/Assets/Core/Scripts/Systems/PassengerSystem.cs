using Unity.Burst;
using Unity.Collections;
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

        PassengerJob passengerJob = new PassengerJob();
        passengerJob.trainPositions = trainPositions;
        passengerJob.ScheduleParallel();
   /*     foreach (var transform in SystemAPI.Query<TransformAspect>().WithAll<PassengerTag>())
        {
            transform.LocalPosition = new float3(transform.LocalPosition.x, transform.LocalPosition.y, trainPositions[0].positionZ);
        }*/
    }

    [BurstCompile][WithAll(typeof(PassengerTag))]
    public partial struct PassengerJob : IJobEntity
    {
        [ReadOnly]public DynamicBuffer<TrainPositionsBuffer> trainPositions;
        public void Execute(TransformAspect transform)
        {
            transform.LocalPosition = new float3(transform.LocalPosition.x, transform.LocalPosition.y, trainPositions[0].positionZ);
        }
    }
}