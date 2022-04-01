using Unity.Entities;
using Unity.Mathematics;

public partial class TrainNavigationSystem : SystemBase
{
    public enum TrainState
    {
        Accelerating,
        Decelerating,
        Embarking
    }
        
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float embarkingTime = 2;
        var stationPositionsDictionary = GetBufferFromEntity<StationPositionBufferElement>(true);

        Entities.WithAll<TrainComponent>()
            .ForEach((ref SpeedComponent speed, 
                ref TrackPositionComponent trackPosition,
                ref WaypointIndexComponent waypointIndex,
                ref TrainStateComponent trainState,
                ref TimeAtStationComponent timeAtStation,
                in TrainComponent trainComponent) =>
            {
                var stationPositions = stationPositionsDictionary[trainComponent.Line];
                var decelerationDistance = speed.CurrentSpeed / speed.Acceleration;
                var nextStationPosition = stationPositions[waypointIndex.NextWaypoint].positionAlongRail;
                
                switch (trainState.Value) {
                    case TrainState.Accelerating:
                        speed.CurrentSpeed = math.clamp(speed.CurrentSpeed + speed.Acceleration, 0, speed.MaxSpeed);
                
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
        }).Schedule();
    }
}
