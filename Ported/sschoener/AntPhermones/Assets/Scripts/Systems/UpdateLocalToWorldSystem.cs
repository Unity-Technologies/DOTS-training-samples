using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class UpdateLocalToWorldSystem : JobComponentSystem
{
    EntityQuery m_AntSettingsQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_AntSettingsQuery = GetEntityQuery(ComponentType.ReadOnly<AntRenderSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var ants = m_AntSettingsQuery.GetSingleton<AntRenderSettingsComponent>();
        return new Job
        {
            Scale = ants.Scale
        }.Schedule(this, inputDeps);
    }

    [BurstCompile]
    struct Job : IJobForEach<FacingAngleComponent, PositionComponent, LocalToWorldComponent>
    {
        public float3 Scale;

        public void Execute([ReadOnly] ref FacingAngleComponent angle, [ReadOnly] ref PositionComponent position, [WriteOnly] ref LocalToWorldComponent localToWorld)
        {
            localToWorld.Value = float4x4.TRS(
                new float3(position.Value, 0),
                quaternion.Euler(new float3(0, 0, angle.Value)),
                Scale);
        }
    }
}
