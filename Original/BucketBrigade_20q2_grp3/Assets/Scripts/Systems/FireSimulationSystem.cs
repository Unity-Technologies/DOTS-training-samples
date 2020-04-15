using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireSimulationSystem : SystemBase
{
    public float FireSpreadProbabilityMultiplier = 1;

    private const float m_PropagationChance = 0.3f;
    private const float m_GrowSpeed = 0.02f;

    private const double m_UpdateFrequency = 0.01f;
    private double m_LastUpdateTime;

    private FloatRandom m_Random;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_Random = FloatRandom.Create(0);
        m_LastUpdateTime = Time.ElapsedTime;
    }

    protected override void OnUpdate()
    {
        if (!GridData.Instance.Heat.IsCreated)
            return;

        if (m_LastUpdateTime + m_UpdateFrequency < Time.ElapsedTime)
        {
            m_LastUpdateTime = Time.ElapsedTime;

            GrowFire();

            PropagateFire();

            // TODO: Remove since this will be done by another system?
            UpdateFirePosition();
        }
    }

    private void GrowFire()
    {
        var data = GridData.Instance;
        var job = new GrowFireJob { Data = data }.Schedule(data.Width * data.Height, data.Width);
        job.Complete();
    }

    private void PropagateFire()
    {
        var data = GridData.Instance;
        var job = new PropagateFireJob { Heat = data.Heat, Width = data.Width, Random = m_Random, ProbabilityMultiplier = FireSpreadProbabilityMultiplier}.Schedule(/*data.Width * data.Height, data.Width*/);
        job.Complete();
    }

    private void UpdateFirePosition()
    {
        var data = GridData.Instance;
        var time = Time.ElapsedTime;
        Entities
            .WithName("UpdateFirePosition")
            .ForEach((ref Translation translation, in GridCell cell) =>
        {
            var position = translation.Value;
            position.y = data.Heat[cell.Index] * 4;
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
    private struct PropagateFireJob : IJob
    {
        [NativeDisableParallelForRestriction] public NativeArray<float> Heat;
        public int Width;
        public FloatRandom Random;
        public float ProbabilityMultiplier;

        public void Execute()
        {
            for (int index = 0; index < Heat.Length; index++)
            {
                var row = index / Width;
                if (Heat[index] < float.Epsilon)
                {
                    var up = index + Width;
                    if (up < Heat.Length && Heat[up] > float.Epsilon)
                    {
                        if (Random.NextFloat() < Heat[up] * ProbabilityMultiplier)
                        {
                            Heat[index] = m_GrowSpeed;
                            continue;
                        }
                    }

                    var right = index + 1;
                    if (right < Heat.Length && Heat[right] > float.Epsilon && row == right / Width)
                    {
                        if (Random.NextFloat() < Heat[right] * ProbabilityMultiplier)
                        {
                            Heat[index] = m_GrowSpeed;
                            continue;
                        }
                    }
                    var down = index - Width;
                    if (down >= 0 && Heat[down] > float.Epsilon)
                    {
                        if (Random.NextFloat() < Heat[down] * ProbabilityMultiplier)
                        {
                            Heat[index] = m_GrowSpeed;
                            continue;
                        }
                    }

                    var left = index - 1;
                    if (left >= 0 && Heat[left] > float.Epsilon && row == left / Width)
                    {
                        if (Random.NextFloat() < Heat[left] * ProbabilityMultiplier)
                        {
                            Heat[index] = m_GrowSpeed;
                            continue;
                        }
                    }
                }
            }
        }
    }

    [BurstCompile]
    private struct GrowFireJob : IJobParallelFor
    {
        public GridData Data;

        public void Execute(int index)
        {
            if (Data.Heat[index] > 0 && Data.Heat[index] < 1)
            {
                Data.Heat[index] = math.min(Data.Heat[index] + m_GrowSpeed, 1);
            }
        }
    }
}
