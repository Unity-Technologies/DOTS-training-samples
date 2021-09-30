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
        var settings = GetSingleton<Settings>();

        var deltaTime = Time.DeltaTime;
        Entities.ForEach((ref DoorMovement doorMovement, ref Translation translation, in TrainReference trainReference) =>
        {

            var trainState = GetComponent<TrainState>(trainReference.Train);
            if (trainState.State == TrainMovementStates.WaitingAtPlatform)
                doorMovement.timeSpentAtStation += deltaTime;
            else
            {
                doorMovement.timeSpentAtStation = 0;
            }
            
            var openingProgress = math.clamp(doorMovement.timeSpentAtStation / openingDuration,0,1);

            var closingProgress = math.clamp((settings.TimeAtStation - doorMovement.timeSpentAtStation) / openingDuration, 0,
                1);
            var progress = math.min(openingProgress, closingProgress);
            translation.Value.x = (closedPos + progress * openingDistance) * (doorMovement.leftDoor ? 1.0f : -1.0f); 

        }).ScheduleParallel();
    }
}
