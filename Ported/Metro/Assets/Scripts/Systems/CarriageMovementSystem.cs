using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [WithNone(typeof(TempCarriageDestination))]
    partial struct CarriageJob : IJobEntity
    {        
        [ReadOnly] public TrainPositions TrainPositions;
        public EntityCommandBuffer.ParallelWriter ECB;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveCarriage(ref CarriageAspect carriage, float3 trainPosition, float3 trainForward, quaternion trainRotation)
        {
            carriage.Position = trainPosition - trainForward * carriage.Width * carriage.Index;
            carriage.Rotation = trainRotation;
        }

        void Execute([ChunkIndexInQuery] int chunkIndex, ref CarriageAspect carriage)
        {
            var trainPosition = TrainPositions.TrainsPositions[carriage.TrainID];
            var trainRotation = TrainPositions.TrainsRotations[carriage.TrainID];
            var trainForward = math.rotate(trainRotation, math.forward());
            if (carriage.Index == 0)
            {
                MoveCarriage(ref carriage, trainPosition, trainForward, trainRotation);
            }
            else
            {
                var carriageForward = carriage.Forward;
                if (math.dot(trainForward, carriageForward) > 0.95f)
                {
                    MoveCarriage(ref carriage, trainPosition, trainForward, trainRotation);
                }
                else
                {
                    ECB.AddComponent(chunkIndex,carriage.Self,new TempCarriageDestination
                    {
                        TempDestination = trainPosition
                    });
                }
            }
        }
    }
    
    [BurstCompile]
    partial struct TempCarriageJob : IJobEntity
    {        
        [ReadOnly] public TrainPositions TrainPositions;
        public EntityCommandBuffer.ParallelWriter ECB;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveCarriage(ref TempCarriageAspect carriage, float3 trainPosition, float3 trainForward, quaternion trainRotation)
        {
            carriage.Position = trainPosition - trainForward * carriage.Width * carriage.Index;
            carriage.Rotation = trainRotation;
        }

        void Execute([ChunkIndexInQuery] int chunkIndex, ref TempCarriageAspect carriage)
        {
            
            var trainMovedDistance = TrainPositions.TrainsDistanceChanged[carriage.TrainID];
            var trainDistance = math.length(trainMovedDistance);

            var destinationDirection = carriage.TempDestination - carriage.Position;
            var distanceToDestination = math.length(destinationDirection);
            if (distanceToDestination < trainDistance)
            {
                ECB.RemoveComponent<TempCarriageDestination>(chunkIndex, carriage.Self);
                var trainPosition = TrainPositions.TrainsPositions[carriage.TrainID];
                var trainRotation = TrainPositions.TrainsRotations[carriage.TrainID];
                var trainForward = math.rotate(trainRotation, math.forward());
                MoveCarriage(ref carriage, trainPosition, trainForward, trainRotation);
            }
            else
            {
                carriage.Position += math.normalize(destinationDirection) * trainDistance;
            }
        }

    }
    
    [BurstCompile]
    partial struct PassengerMoveJob : IJobEntity
    {        
        public EntityCommandBuffer.ParallelWriter ECB;
        
        void Execute([ChunkIndexInQuery] int chunkIndex, ref CarriageMovePassengerAspect carriage)
        {
            var passengers = carriage.Passengers;
            var carriagePosition = carriage.Transform.ValueRO.Position;
            var carriageRotation = carriage.Transform.ValueRO.Rotation;
            var seats = carriage.CarriageSeats.ValueRO;
            for (int i = 0; i < passengers.Length; i++)
            {
                var seatPosition = carriagePosition + math.rotate(carriageRotation,seats.Seats[i]);
                ECB.SetComponent(chunkIndex,passengers[i].Value, LocalTransform.FromPosition(seatPosition));
            }
        }

    }
    
    [BurstCompile]
    [UpdateAfter(typeof(TrainMovementSystem))]
    public partial struct CarriageMovementSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Carriage>();
            state.RequireForUpdate<TrainPositions>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var trainPositions = SystemAPI.GetSingleton<TrainPositions>();
            if(trainPositions.TrainsPositions.Length == 0) return;
            
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var carriageJob = new CarriageJob
            {
                TrainPositions = trainPositions,
                ECB = ecb.AsParallelWriter()
            };
            var tempCarriageJob = new TempCarriageJob
            {
                TrainPositions = trainPositions,
                ECB = ecb.AsParallelWriter()
            };
            var movePassengers = new PassengerMoveJob
            {
                ECB = ecb.AsParallelWriter()
            };

            var carriageHandle = carriageJob.ScheduleParallel(state.Dependency);
            var carriageEdgesJob = tempCarriageJob.ScheduleParallel(carriageHandle);
            movePassengers.ScheduleParallel(carriageEdgesJob).Complete();

            /*foreach (var (seats, passengers,carriage) in SystemAPI.Query<CarriageSeatsPositions,DynamicBuffer<CarriagePassengers>,LocalTransform>())
            {
                for (int i = 0; i < passengers.Length; i++)
                {
                    var seatPosition = carriage.Position + math.rotate(carriage.Rotation,seats.Seats[i]);
                    ecb.SetComponent(passengers[i].Value, LocalTransform.FromPosition(seatPosition));
                }
            }*/
        }
    }
}