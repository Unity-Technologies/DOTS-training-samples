using System;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Random = Unity.Mathematics.Random;

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
            Seed = 1 + (uint)Time.frameCount,
            Magnitude = m_AntSteeringQuery.GetSingleton<AntSteeringSettingsComponent>().RandomSteerStrength,
        }.Schedule(this, inputDeps);
    }

    struct PerturbationJob : IJobForEachWithEntity<FacingAngleComponent> {
        public uint Seed;
        public float Magnitude;

        public void Execute(Entity entity, int index, ref FacingAngleComponent facing)
        {
            var rng = new Random((uint)index * Seed);
            facing.Value += rng.NextFloat(-Magnitude, Magnitude);
        }
    }
}
