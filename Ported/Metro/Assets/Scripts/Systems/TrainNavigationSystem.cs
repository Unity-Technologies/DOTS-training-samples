using Unity.Entities;
public partial class TrainNavigationSystem : SystemBase
{
        
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.WithAll<TrainComponent>()
            .ForEach((ref SpeedComponent speed, ref TrackPositionComponent trackPosition) => {
            trackPosition.Value = (trackPosition.Value + speed.Value * deltaTime) % 1;
        }).Schedule();
    }
}
