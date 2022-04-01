using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class TrainNavigationSystem : SystemBase
{
    public enum TrainState
    {
        Accelerating,
        InTransit,
        Decelerating,
        Embarking
    }
        
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float embarkingTime = 2;
        var stationPositionsDictionary = GetBufferFromEntity<StationPositionBufferElement>(true);

        Entities.WithAll<TrainComponent>()
            .WithReadOnly(stationPositionsDictionary)
            .ForEach((ref SpeedComponent speed, 
                ref TrackPositionComponent trackPosition,
                ref WaypointIndexComponent waypointIndex,
                ref TrainStateComponent trainState,
                ref TimeAtStationComponent timeAtStation,
                in TrainComponent trainComponent) =>
            {
                var stationPositions = stationPositionsDictionary[trainComponent.Line];
                var decelerationDistance = speed.Acceleration / speed.CurrentSpeed;
                var nextStationPosition = stationPositions[waypointIndex.NextWaypoint].positionAlongRail;
                
                switch (trainState.Value) {
                    case TrainState.Accelerating:
                        speed.CurrentSpeed = math.clamp(speed.CurrentSpeed + speed.Acceleration, 0, speed.MaxSpeed);
                
                        trackPosition.Value = math.frac(trackPosition.Value + speed.CurrentSpeed * deltaTime);

                        if (speed.CurrentSpeed == speed.MaxSpeed)
                        {
                            trainState.Value = TrainState.InTransit;
                        }
                            break;
                    case TrainState.InTransit:
                        trackPosition.Value = math.frac(trackPosition.Value + speed.CurrentSpeed * deltaTime);

                        var distanceToStation = math.frac(nextStationPosition - trackPosition.Value);
                        if (distanceToStation < decelerationDistance)
                        {
                            trainState.Value = TrainState.Decelerating;
                        }

                        break;
                    case TrainState.Decelerating:
                        speed.CurrentSpeed = math.clamp(speed.CurrentSpeed - speed.Acceleration, speed.Acceleration, speed.MaxSpeed);
                
                        trackPosition.Value = math.frac(trackPosition.Value + speed.CurrentSpeed * deltaTime);

                        if (trackPosition.Value >= nextStationPosition)
                        {
                            trainState.Value = TrainState.Embarking;
                            timeAtStation.Value = 0;
                            speed.CurrentSpeed = 0;
                        }
                        
                        break;
                    case TrainState.Embarking:
                        timeAtStation.Value += deltaTime;
                        if (timeAtStation.Value > embarkingTime)
                        {
                            trainState.Value = TrainState.Accelerating;
                            waypointIndex.LastWaypoint = waypointIndex.NextWaypoint;
                            waypointIndex.NextWaypoint++;

                            if (waypointIndex.NextWaypoint >= stationPositions.Length - 1)
                                waypointIndex.NextWaypoint = 0;
                        }

                        break;
                    default:
                        trainState.Value = TrainState.Accelerating;
                        return;
                }
        }).ScheduleParallel();
    }
}
