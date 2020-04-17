using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(OvertakeSystem))]
public class SpeedSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithName("Update_Speed_Blocked").ForEach((ref Speed speed, in BlockSpeed blockSpeed) =>
        {
            speed.Value = blockSpeed.Value;
        }).ScheduleParallel();
        
        Entities.WithName("Update_Speed_Overtake").WithNone<BlockSpeed>().WithAll<OvertakeTag>().ForEach((ref Speed speed, in TargetSpeed targetSpeed, in OvertakeSpeedIncrement overtakeSpeedIncrement) =>
        {
            speed.Value = targetSpeed.Value + overtakeSpeedIncrement.Value;
        }).ScheduleParallel();
        
        Entities.WithName("Update_Speed_Default").WithNone<BlockSpeed, OvertakeTag>().ForEach((ref Speed speed, in TargetSpeed targetSpeed) =>
        {
            speed.Value = targetSpeed.Value;
        }).ScheduleParallel();
    }
}
