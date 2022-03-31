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

        Entities.WithAll<TrainComponent>()
            .ForEach((ref SpeedComponent speed, 
                ref TrackPositionComponent trackPosition, 
                in TrainStateComponent trainState) =>
            {
                float acceleration;
                switch (trainState.Value) {
                    case TrainState.Accelerating:
                        acceleration = speed.Acceleration;
                        break;
                    case TrainState.Decelerating:
                        acceleration = speed.Acceleration * -1;
                        break;
                    default:
                        return;
                }
                speed.CurrentSpeed = math.clamp(speed.CurrentSpeed + speed.Acceleration, 0, speed.MaxSpeed);
                
                trackPosition.Value = math.frac(trackPosition.Value + speed.CurrentSpeed * deltaTime);
        }).ScheduleParallel();
    }
}
