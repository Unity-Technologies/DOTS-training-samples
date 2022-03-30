using Unity.Entities;
using Unity.Mathematics;

public partial class TrainNavigationSystem : SystemBase
{
        
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.WithAll<TrainComponent>()
            .ForEach((ref SpeedComponent speed, ref TrackPositionComponent trackPosition) => {
            trackPosition.Value = math.frac(trackPosition.Value + speed.Value * deltaTime);
        }).ScheduleParallel();
    }
}
