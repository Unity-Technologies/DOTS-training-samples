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

                if (t.Value.x > zones.LevelBounds.Max.x)
                {
                    t.Value.x = zones.LevelBounds.Max.x;
                    d.v = d.kineticEnergyPreserved * d.v;
                    d.v.x = -d.v.x;
                }
                if (t.Value.x < zones.LevelBounds.Min.x)
                {
                    t.Value.x = zones.LevelBounds.Min.x;
                    d.v = d.kineticEnergyPreserved * d.v;
                    d.v.x = -d.v.x;
                }
                if (t.Value.y > zones.LevelBounds.Max.y)
                {
                    t.Value.y = zones.LevelBounds.Max.y;
                    d.v = d.kineticEnergyPreserved * d.v;
                    d.v.y = -d.v.y;
                }
                if (t.Value.y < zones.LevelBounds.Min.y)
                {
                    t.Value.y = zones.LevelBounds.Min.y;
                    d.v = d.kineticEnergyPreserved * d.v;
                    d.v.y = -d.v.y;
                }
                if (t.Value.z > zones.LevelBounds.Max.z)
                {
                    t.Value.z = zones.LevelBounds.Max.z;
                    d.v = d.kineticEnergyPreserved * d.v;
                    d.v.z = -d.v.z;
                }
                if (t.Value.z < zones.LevelBounds.Min.z)
                {
                    t.Value.z = zones.LevelBounds.Min.z;
                    d.v = d.kineticEnergyPreserved * d.v;
                    d.v.z = -d.v.z;
                }
            }).ScheduleParallel();
    }
}