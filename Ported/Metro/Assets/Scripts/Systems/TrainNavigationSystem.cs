using Unity.Entities;
using Unity.Mathematics;

public partial class TrainNavigationSystem : SystemBase
{
        
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.WithAll<TrainComponent>()
            .ForEach((ref SpeedComponent speed, ref TrackPositionComponent trackPosition) =>
            {
                speed.CurrentSpeed = math.clamp(speed.CurrentSpeed + speed.Acceleration, 0, speed.MaxSpeed);
                
                trackPosition.Value = math.frac(trackPosition.Value + speed.CurrentSpeed * deltaTime);
        }).ScheduleParallel();
    }
}
