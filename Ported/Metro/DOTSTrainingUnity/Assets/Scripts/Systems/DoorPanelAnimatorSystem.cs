using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class DoorPanelAnimatorSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var parentAnimTimers = GetComponentDataFromEntity<DoorCurrAnimTime>();

        Entities.ForEach((ref Translation position, in DoorPanelParentRef doorPanel) => {
            float currTimer = parentAnimTimers[doorPanel.value].value;
            position.Value.x = 1.0f * currTimer;

        }).Schedule();
    }
}
