using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems
{
    [BurstCompile]
    partial struct CarriageJob : IJobEntity
    {        
        [ReadOnly] public TrainPositions TrainPositions;
        
        void Execute(ref CarriageAspect carriage)
        {
            var trainPosition = TrainPositions.TrainsPositions[carriage.UniqueTrainID];
            var trainRotation = TrainPositions.TrainsRotations[carriage.UniqueTrainID];
            var direction = math.rotate(trainRotation, math.forward());
            
            carriage.Position = trainPosition - direction * carriage.Width * carriage.Index;
            carriage.Rotation = trainRotation;
        }
    }

    [BurstCompile]
    [UpdateAfter(typeof(TrainMovementSystem))]
    public partial struct CarriageMovementSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var trainPositions = SystemAPI.GetSingleton<TrainPositions>();
            var carriageJob = new CarriageJob
            {
                TrainPositions = trainPositions
            };

            carriageJob.ScheduleParallel();

        }
    }
}