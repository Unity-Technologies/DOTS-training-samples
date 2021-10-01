using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class DoorMovementSystem : SystemBase
{
    private const float openingDistance = 0.3f;
    private const float closedPos = 0.15f;
    private const float openingDuration = 1.0f;
    
    protected override void OnUpdate()
    {
        ref var settings = ref GetSingleton<Settings>().SettingsBlobRef.Value;
        var settingsTimeAtStation = settings.TimeAtStation;

        var deltaTime = Time.DeltaTime;
        Entities.ForEach((ref DoorMovement doorMovement, ref Translation translation, in TrainReference trainReference) =>
        {

            var trainState = GetComponent<TrainState>(trainReference.Train);
            if (trainState.State == TrainMovementStates.WaitingAtPlatform)
                doorMovement.timeSpentAtPlatform += deltaTime;
            else
            {
                doorMovement.timeSpentAtPlatform = 0;
            }
            
            var openingProgress = math.clamp(doorMovement.timeSpentAtPlatform / openingDuration,0,1);

            var closingProgress = math.clamp((settingsTimeAtStation - doorMovement.timeSpentAtPlatform) / openingDuration, 0,
                1);
            var progress = math.min(openingProgress, closingProgress);
            translation.Value.x = (closedPos + progress * openingDistance) * (doorMovement.leftDoor ? 1.0f : -1.0f); 

        }).ScheduleParallel();
    }
}
