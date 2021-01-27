using Unity.Entities;
using Unity.Transforms;

public class PhysicsSolverSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;
        Entities
            .WithName("PhysicsSolverSystem")
            .ForEach((Entity e, ref Translation t, ref PhysicsData d) =>
            {
                d.v += d.a * dt;
                d.a = 0;
                t.Value += d.v * dt;

                if (t.Value.y < d.floor)
                    t.Value.y = d.floor;
            }).ScheduleParallel();
    }
}