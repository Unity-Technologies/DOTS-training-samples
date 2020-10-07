using Unity.Entities;
using Unity.Mathematics;

public class TimerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        Entities.ForEach(
            (ref Timer timer) =>
            {
                timer.Value -= deltaTime;
                timer.Value = math.max(timer.Value, 0f);
            }).ScheduleParallel();
    }
}