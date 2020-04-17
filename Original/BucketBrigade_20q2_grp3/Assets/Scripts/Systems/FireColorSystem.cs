using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(RenderMeshSystemV2))]
public class FireColorSystem : SystemBase
{
    public double UpdateFrequency;

    public JobHandle Deps;

    private double m_LastUpdateTime;

    private FirePropagateSystem m_FirePropagateSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_FirePropagateSystem = World.GetOrCreateSystem<FirePropagateSystem>();
    }

    protected override void OnUpdate()
    {
        if (!GridData.Instance.Heat.IsCreated)
            return;

        Dependency = JobHandle.CombineDependencies(Dependency, m_FirePropagateSystem.Deps);

        if (m_LastUpdateTime + UpdateFrequency < Time.ElapsedTime)
        {
            m_LastUpdateTime = Time.ElapsedTime;

            var data = GridData.Instance;
            Deps = Entities
                .WithName("UpdateFireColor")
                .WithReadOnly(data)
                .ForEach((ref Unity.Rendering.MaterialColor color, in GridCell cell) =>
                {
                    var heat = (float)data.Heat[cell.Index] / byte.MaxValue;
                    var value = (1 - heat) * (1 - heat);
                    color.Value = new float4(1 - value, value, 0, 1);
                }).ScheduleParallel(Dependency);
            Dependency = Deps;
        }
    }
}
