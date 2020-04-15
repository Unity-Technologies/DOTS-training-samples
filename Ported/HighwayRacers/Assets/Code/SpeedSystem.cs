using Unity.Entities;
using UnityEngine;

public class SpeedSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Speed speed, in BlockSpeed blockSpeed) =>
        {
            speed.Value = blockSpeed.Value;
        }).ScheduleParallel();
        
        Entities.ForEach((ref Speed speed, in TargetSpeed targetSpeed, in OvertakeTag overtakeTag, in OvertakeSpeedIncrement overtakeSpeedIncrement) =>
        {
            speed.Value = targetSpeed.Value + overtakeSpeedIncrement.Value;
        }).ScheduleParallel();
        
        Entities.WithNone<BlockSpeed, OvertakeTag>().ForEach((ref Speed speed, in TargetSpeed targetSpeed) =>
        {
            speed.Value = targetSpeed.Value;
        }).ScheduleParallel();
    }
}
