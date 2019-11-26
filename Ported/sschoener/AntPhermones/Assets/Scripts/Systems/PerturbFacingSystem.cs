using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class PerturbFacingSystem : JobComponentSystem
{
    EntityQuery m_AntSteeringQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_AntSteeringQuery = GetEntityQuery(ComponentType.ReadOnly<AntSteeringSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var magnitude = m_AntSteeringQuery.GetSingleton<AntSteeringSettingsComponent>().RandomSteerStrength;
        return Entities.ForEach((ref FacingAngleComponent facingAngle, ref RandomSteeringComponent random) =>
        {
            facingAngle.Value += random.Rng.NextFloat(-magnitude, magnitude);
        }).Schedule(inputDeps);
    }
}
