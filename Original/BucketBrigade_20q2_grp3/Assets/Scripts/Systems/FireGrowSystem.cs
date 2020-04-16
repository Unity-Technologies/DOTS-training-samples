using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(ExtinguishSystem))]
public class FireGrowSystem : SystemBase
{
    public int FireGrowStep;
    public double UpdateGrowFrequency;

    public JobHandle Deps;

    private double m_LastUpdateGrowTime;

    private ExtinguishSystem m_ExtinguishSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_ExtinguishSystem = World.GetOrCreateSystem<ExtinguishSystem>();
    }

    protected override void OnUpdate()
    {
        if (!GridData.Instance.Heat.IsCreated)
            return;

        if (m_LastUpdateGrowTime + UpdateGrowFrequency < Time.ElapsedTime)
        {
            m_LastUpdateGrowTime = Time.ElapsedTime;

            Dependency = JobHandle.CombineDependencies(Dependency, m_ExtinguishSystem.Deps);

            GrowFire();
            UpdateColor();

            UpdateFirePosition();
        }

        Deps = Dependency;
    }

    private void GrowFire()
    {
        Dependency = JobHandle.CombineDependencies(Dependency, m_ExtinguishSystem.Deps);
        var data = GridData.Instance;
        var job = new GrowFireJob { Data = data, IncreaseStep = FireGrowStep }.Schedule(data.Width * data.Height, data.Width, Dependency);
        Dependency = JobHandle.CombineDependencies(Dependency, job);
    }

    private void UpdateColor()
    {
        var data = GridData.Instance;
        Entities
            .WithName("UpdateFireColor")
            .WithReadOnly(data)
            .ForEach((ref Unity.Rendering.MaterialColor color, in GridCell cell) =>
            {
                var heat = (float)data.Heat[cell.Index] / byte.MaxValue;
                var value = math.pow(1 - heat, 2);
                color.Value = new float4(1, value, value, 1);
            }).Schedule();
    }

    private void UpdateFirePosition()
    {
        var data = GridData.Instance;
        var time = Time.ElapsedTime;
        Entities
            .WithName("UpdateFirePosition")
            .WithReadOnly(data)
            .ForEach((ref Translation translation, in GridCell cell) =>
        {
            var position = translation.Value;
            position.y = (float)data.Heat[cell.Index] / byte.MaxValue * 4;
            if (position.y > 0)
            {
                var row = cell.Index / data.Width;
                var column = cell.Index % data.Height;
                position.y += (math.sin((float)time + (column) / 0.4f) + math.cos((float)time + (row + column) / 0.4f)) * 0.3f;
            }
            translation.Value = position;
        }).ScheduleParallel();
    }

    [BurstCompile]
    private struct GrowFireJob : IJobParallelFor
    {
        public GridData Data;
        public int IncreaseStep;

        public void Execute(int index)
        {
            if (Data.Heat[index] > 0 && Data.Heat[index] < byte.MaxValue)
            {
                Data.Heat[index] = (byte)math.min(Data.Heat[index] + IncreaseStep, byte.MaxValue);
            }
        }
    }
}
