using Unity.Entities;
using Unity.Transforms;

public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        var deltaTime = Time.DeltaTime;

        Entities
            .ForEach((ref Translation translation, ref ParabolaTValue tValue, in Parabola parabola) =>
            {
                if (tValue.Value >= 0)
                {
                    float y = JumpTheGun.Parabola.Solve(parabola.A, parabola.B, parabola.C, tValue.Value);
                    translation.Value.y = y;
                    tValue.Value += deltaTime / parabola.Duration;
                    if (tValue.Value > 1.0f)
                    {
                        tValue.Value = -1; // reset and calculate new bounce
                    }

                    // move forward in x/z, i.e. mouse input or cannon target,
                    translation.Value += parabola.Forward / (parabola.Duration / deltaTime); 
                }
            }).ScheduleParallel();
    }
}