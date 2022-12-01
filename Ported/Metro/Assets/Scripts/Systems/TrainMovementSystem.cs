using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems
{
    [BurstCompile]
    partial struct SpeedController : IJobEntity
    {
        [ReadOnly] public TrainPositions TrainPositions;
        public float DeltaTime;

        void Execute(ref TrainSpeedControllerAspect train)
        {
            switch (train.State)
            {
                case TrainState.EnRoute:
                {
                    var nextTrainPosition = TrainPositions.TrainsPositions[train.NextTrainID];
                    var nextTrainRotation = TrainPositions.TrainsRotations[train.NextTrainID];
                    var nextTrainDirection = math.rotate(nextTrainRotation, math.forward());
                    var checkDirection = math.dot(nextTrainDirection, train.Forward);
                    var distance = math.distance(train.Position, nextTrainPosition);
                    if (checkDirection > 0.005f && distance < 35f)
                    {
                        train.Speed = math.min(train.Speed - (0.25f * DeltaTime) * train.MaxSpeed, 0f);
                    }
                    else
                    {
                        var distanceToDestination = math.distance(train.Destination, train.Position);
                        if (distanceToDestination > 20f)
                            train.Speed = math.min(train.Speed + (0.25f * DeltaTime) * train.MaxSpeed, train.MaxSpeed);
                        else if (train.DestinationType == RailwayPointType.Platform)
                        {
                            var minimalSpeed = distanceToDestination < 0.1f ? 0f : 0.1f * train.MaxSpeed;
                            var desiredSpeed = (distanceToDestination / 20f) * train.MaxSpeed;
                            var sign = desiredSpeed > train.Speed ? 1f : -1f;
                            var nextSpeed = train.Speed + sign * 0.05f * train.MaxSpeed;
                            train.Speed = math.max(nextSpeed, minimalSpeed);
                        }
                    }

                    break;
                }
                case TrainState.Departing:
                {
                    train.Speed = 0.05f * train.MaxSpeed;
                    break;
                }
                default:
                    train.Speed = 0;
                    break;
            }
        }
    }

    [BurstCompile]
    partial struct TrainMovementJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<MetroLine> m_MetroLine;

        // Time cannot be directly accessed from a job, so DeltaTime has to be passed in as a parameter.
        public float DeltaTime;

        public void SetNextDestinationPoint(ref TrainAspect train)
        {
            var metroLine = m_MetroLine[train.Train.ValueRO.MetroLine];
            var nextDestination = metroLine.GetNextRailwayPoint(train.DestinationIndex);
            train.Destination = nextDestination.Item1;
            train.DestinationType = nextDestination.Item2;
            train.DestinationIndex = nextDestination.Item3;
        }

        void Execute(ref TrainAspect train)
        {
            switch (train.State)
            {
                case TrainState.EnRoute:
                {
                    var direction = train.Destination - train.Position;
                    //train.Train.ValueRW.DestinationDirection = direction;
                    var trainDirection = train.Forward;
                    //train.Train.ValueRW.Forward = trainDirection;
                    var angle = Utility.SignedAngle(math.forward(), direction, math.up());
                    //train.Train.ValueRW.Angle = angle;
                    if (math.abs(angle) > 0.001f)
                        train.Rotation = quaternion.RotateY(angle);

                    if (train.DestinationType == RailwayPointType.Route)
                    {
                        var nextSuggestedPosition = train.Position + math.normalize(direction) * (DeltaTime * train.CurrentSpeed);
                        var distanceToNextPosition = math.distance(nextSuggestedPosition, train.Position);
                        var distanceToDestination = math.distance(train.Destination, train.Position);
                        if (distanceToNextPosition > distanceToDestination)
                            SetNextDestinationPoint(ref train);

                        train.Position = nextSuggestedPosition;
                    }
                    else
                    {
                        var distanceToThePoint = math.lengthsq(direction);
                        if (distanceToThePoint > 0.01f)
                        {
                            train.Position += math.normalize(direction) * (DeltaTime * train.CurrentSpeed);
                        }
                        else
                        {
                            train.State = TrainState.Arrived;
                        }
                    }

                    break;
                }
                case TrainState.Departing:
                {
                    SetNextDestinationPoint(ref train);
                    train.State = TrainState.EnRoute;
                    break;
                }
            }
        }
    }

    [BurstCompile]
    public partial struct TrainMovementSystem : ISystem
    {
        ComponentLookup<MetroLine> m_MetroLine;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_MetroLine = state.GetComponentLookup<MetroLine>(true);
            state.RequireForUpdate<Train>();
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
            if (trainPositions.TrainsPositions.Length == 0)
                return;

            m_MetroLine.Update(ref state);

            var speedController = new SpeedController
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                TrainPositions = trainPositions
            };
            var trainMovementJob = new TrainMovementJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                m_MetroLine = m_MetroLine
            };

            var speedHandle = speedController.ScheduleParallel(state.Dependency);
            trainMovementJob.ScheduleParallel(speedHandle).Complete();
        }
    }
}