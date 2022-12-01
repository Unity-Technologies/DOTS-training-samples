using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    partial struct CarriageJob : IJobEntity
    {        
        [ReadOnly] public TrainPositions TrainPositions;

        void Execute(ref CarriageAspect carriage)
        {
            var trainPosition = TrainPositions.TrainsPositions[carriage.TrainID];
            var trainRotation = TrainPositions.TrainsRotations[carriage.TrainID];
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

            carriageJob.ScheduleParallel(state.Dependency).Complete();

            foreach (var (seats, passengers,carriage) in SystemAPI.Query<CarriageSeatsPositions,DynamicBuffer<CarriagePassengers>,LocalTransform>())
            {
                for (int i = 0; i < passengers.Length; i++)
                {
                    var seatPosition = carriage.Position + math.rotate(carriage.Rotation,seats.Seats[i]);
                    SystemAPI.SetComponent(passengers[i].Value, LocalTransform.FromPosition(seatPosition));
                }
            }
        }
    }
}