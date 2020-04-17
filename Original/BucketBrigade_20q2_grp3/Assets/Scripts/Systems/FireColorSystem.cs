using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FireExtinguishSystem))]
public class FireColorSystem : SystemBase
{
    public double UpdateGrowFrequency;

    public JobHandle Deps;

    private double m_LastUpdateGrowTime;

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

        if (m_LastUpdateGrowTime + UpdateGrowFrequency < Time.ElapsedTime)
        {
            m_LastUpdateGrowTime = Time.ElapsedTime;

            var data = GridData.Instance;
            Entities
                .WithName("UpdateFireColor")
                .WithReadOnly(data)
                .ForEach((ref Unity.Rendering.MaterialColor color, in GridCell cell) =>
                {
                    var heat = (float)data.Heat[cell.Index] / byte.MaxValue;
                    var value = math.pow(1 - heat, 2);
                    color.Value = new float4(1 - value, value, 0, 1);
                }).ScheduleParallel();
            Deps = Dependency;
        }
    }
}
