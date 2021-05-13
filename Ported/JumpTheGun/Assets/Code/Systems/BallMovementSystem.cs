using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class BallMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (TryGetSingleton<IsPaused>(out _))
            return;

        float currentTime = (float)Time.ElapsedTime;

        Entities
            .WithAll<Player, Arc, Time>()
            .WithAll<Translation, BallTrajectory, TimeOffset>().ForEach(
            (ref Translation translation, in Player p, in Time t, in BallTrajectory trajectory, in Arc arc, in TimeOffset timeOffset) =>
            {
                float elapsedTime = currentTime - t.StartTime;
                if (elapsedTime < p.CooldownTime)
                    return;

                elapsedTime -= p.CooldownTime;

                var timeInParabola = math.clamp((elapsedTime - timeOffset.Value) / p.BounceTime, 0.0f, 1.0f);
                float yInParabola = ParabolaUtil.Solve(arc.Value.x, arc.Value.y, arc.Value.z, timeInParabola);
                float3 position = math.lerp(trajectory.Source, trajectory.Destination, timeInParabola);
                position.y = yInParabola;
                translation.Value = position;
            }).ScheduleParallel();
    }
}
