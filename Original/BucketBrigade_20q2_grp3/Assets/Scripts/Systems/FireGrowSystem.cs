using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FireExtinguishSystem))]
public class FireGrowSystem : SystemBase
{
    public int FireGrowStep;
    public double UpdateGrowFrequency;

    public JobHandle Deps;

    private double m_LastUpdateGrowTime;

    private FireExtinguishSystem m_FireExtinguishSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_FireExtinguishSystem = World.GetOrCreateSystem<FireExtinguishSystem>();
    }

    protected override void OnUpdate()
    {
        if (!GridData.Instance.Heat.IsCreated)
            return;

        Dependency = JobHandle.CombineDependencies(Dependency, m_FireExtinguishSystem.Deps);

        if (m_LastUpdateGrowTime + UpdateGrowFrequency < Time.ElapsedTime)
        {
            m_LastUpdateGrowTime = Time.ElapsedTime;

            GrowFire();

            UpdateFirePosition();
        }

        Deps = Dependency;
    }

    private void GrowFire()
    {
        var data = GridData.Instance;
        var job = new GrowFireJob
        {
            Data = data,
            IncreaseStep = FireGrowStep
        }.Schedule(data.Width * data.Height, data.Width, Dependency);
        Dependency = JobHandle.CombineDependencies(Dependency, job);
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
