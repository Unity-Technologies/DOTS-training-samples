using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class PhysicsSolverSystem : SystemBase
{
    private EntityQuery m_Query;
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<SpawnZones>();
        RequireForUpdate(m_Query);
    }

    protected override void OnUpdate()
    {
        var zones = GetSingleton<SpawnZones>();
        var dt = Time.DeltaTime;
        Entities
            .WithName("PhysicsSolverSystem")
            .WithStoreEntityQueryInField(ref m_Query)
            .ForEach((Entity e, ref Translation t, ref PhysicsData d) =>
            {
                d.v += d.a * dt;
                d.v *= (1 - d.damping);
                d.a = 0;
                t.Value += d.v * dt;

                if (t.Value.y < zones.LevelBounds.Min.y)
                {
                    t.Value.y = zones.LevelBounds.Min.y;
                    d.v = d.kineticEnergyPreserved * d.v;
                    d.v.y = -d.v.y;
                }

            }).ScheduleParallel();
    }
}