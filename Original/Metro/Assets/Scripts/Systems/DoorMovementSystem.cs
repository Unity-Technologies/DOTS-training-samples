using dots_src.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class DoorMovementSystem : SystemBase
{
    private const float openingDistance = 0.3f;
    private const float closedPos = 0.15f;
    
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        Entities.ForEach((ref DoorMovement doorMovement, ref Translation translation, in PlatformRef platform) =>
        {
            if (platform.Platform.Equals(Entity.Null)) return;
            var trainState = GetComponent<BoardingState>(platform.Platform);
            
            if (doorMovement.LeftTrainSide) return;
            
            if (trainState == BoardingStates.Arriving && doorMovement.Progress < 1f)
                doorMovement.Progress += deltaTime*10f;
            else if (trainState == BoardingStates.Leaving && doorMovement.Progress > 0f)
                doorMovement.Progress -= deltaTime*10f;

            doorMovement.Progress = math.saturate(doorMovement.Progress);
            
            translation.Value.x = (closedPos + doorMovement.Progress  * openingDistance) * (doorMovement.LeftDoor ? 1.0f : -1.0f); 

        }).ScheduleParallel();
    }
}
