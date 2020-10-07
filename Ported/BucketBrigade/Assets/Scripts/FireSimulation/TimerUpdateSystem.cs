using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class TimerUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaT = Time.DeltaTime;
        Entities.
            ForEach((ref Timer timer) =>
        {
            if (timer.elapsedTime > timer.timerValue)
                timer.elapsedTime = 0.0f;

            timer.elapsedTime += deltaT;
        }).ScheduleParallel();
    }
}
