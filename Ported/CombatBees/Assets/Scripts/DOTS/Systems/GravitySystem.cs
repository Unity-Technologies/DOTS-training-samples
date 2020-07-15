using Unity.Entities;
using Unity.Mathematics;

public class GravitySystem : SystemBase
{
    public float Gravity = -9.8f;

    protected override void OnUpdate()
    {
        var force = new float3(0, Gravity * Time.DeltaTime, 0);

        Entities.WithAll<Gravity>()
            .ForEach((ref Velocity v) =>
            {
                v.Value = v.Value + force;
            }).ScheduleParallel();
    }
}
