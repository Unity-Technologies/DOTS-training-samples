using Unity.Entities;
using Unity.Mathematics;

public class GravitySystem : SystemBase
{
    // Gravity value copied from the original.
    const float k_Gravity = -20f;

    protected override void OnUpdate()
    {
        var force = new float3(0, k_Gravity * Time.DeltaTime, 0);

        Entities.WithAll<Gravity>()
            .WithNone<Carried>()
            .ForEach((ref Velocity v) => v.Value += force).ScheduleParallel();
    }
}
