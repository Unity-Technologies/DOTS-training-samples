using Unity.Burst;
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
        return new PerturbationJob {
            Magnitude = m_AntSteeringQuery.GetSingleton<AntSteeringSettingsComponent>().RandomSteerStrength,
        }.Schedule(this, inputDeps);
    }

    [BurstCompile]
    struct PerturbationJob : IJobForEach<FacingAngleComponent, RandomSteeringComponent> {
        public float Magnitude;

        public void Execute(ref FacingAngleComponent facingAngle, ref RandomSteeringComponent random)
        {
            facingAngle.Value += random.Rng.NextFloat(-Magnitude, Magnitude);
        }
    }
}
