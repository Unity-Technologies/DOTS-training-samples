using Unity.Entities;
using UnityEngine;

public class PercentCompleteSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref PercentComplete percentComplete, in Speed speed) =>
        {
            percentComplete.Value += speed.Value * deltaTime - (int)percentComplete.Value;
        }).ScheduleParallel();
    }
}
