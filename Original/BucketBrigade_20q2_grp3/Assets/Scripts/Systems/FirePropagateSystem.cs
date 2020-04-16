using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FireGrowSystem))]
public class FirePropagateSystem : SystemBase
{
    public float PropagationChance = 1;
    public double UpdatePropagationFrequency;

    public JobHandle Deps;

    private double m_LastUpdatePropagationTime;
    private FloatRandom m_Random;

    private FireGrowSystem m_FireGrowSystem;
    private EntityCommandBufferSystem m_EcbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_Random = FloatRandom.Create(0);
        m_FireGrowSystem = World.GetOrCreateSystem<FireGrowSystem>();
        m_EcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_Random.Dispose();
    }

    protected override void OnUpdate()
    {
        if (!GridData.Instance.Heat.IsCreated)
            return;

        Dependency = JobHandle.CombineDependencies(Dependency, m_FireGrowSystem.Deps);

        if (m_LastUpdatePropagationTime + UpdatePropagationFrequency < Time.ElapsedTime)
        {
            m_LastUpdatePropagationTime = Time.ElapsedTime;

            var data = GridData.Instance;
            var job = new PropagateFireJob {
                Heat = data.Heat,
                Width = data.Width,
                Random = m_Random,
                PropagationChance = PropagationChance,
                IncreaseStep = 1
            }.Schedule(Dependency);
            Dependency = JobHandle.CombineDependencies(Dependency, job);
        }

        Deps = Dependency;
        m_EcbSystem.AddJobHandleForProducer(Dependency);
    }

    [BurstCompile]
    private struct PropagateFireJob : IJob
    {
        [NativeDisableParallelForRestriction] public NativeArray<byte> Heat;
        public int Width;
        public FloatRandom Random;
        public float PropagationChance;
        public int IncreaseStep;

        public void Execute()
        {
            for (int index = 0; index < Heat.Length; index++)
            {
                var row = index / Width;
                if (Heat[index] == 0)
                {
                    var up = index + Width;
                    if (up < Heat.Length && Heat[up] != 0)
                    {
                        float heatRatio = (float)Heat[up] / byte.MaxValue;
                        if (Random.NextFloat() < heatRatio * PropagationChance)
                        {
                            Heat[index] = (byte)IncreaseStep;
                            continue;
                        }
                    }

                    var right = index + 1;
                    if (right < Heat.Length && Heat[right] != 0 && row == right / Width)
                    {
                        float heatRatio = (float)Heat[right] / byte.MaxValue;
                        if (Random.NextFloat() < heatRatio * PropagationChance)
                        {
                            Heat[index] = (byte)IncreaseStep;
                            continue;
                        }
                    }

                    var down = index - Width;
                    if (down >= 0 && Heat[down] != 0)
                    {
                        float heatRatio = (float)Heat[down] / byte.MaxValue;
                        if (Random.NextFloat() < heatRatio * PropagationChance)
                        {
                            Heat[index] = (byte)IncreaseStep;
                            continue;
                        }
                    }

                    var left = index - 1;
                    if (left >= 0 && Heat[left] != 0 && row == left / Width)
                    {
                        float heatRatio = (float)Heat[left] / byte.MaxValue;
                        if (Random.NextFloat() < heatRatio * PropagationChance)
                        {
                            Heat[index] = (byte)IncreaseStep;
                            continue;
                        }
                    }
                }
            }
        }
    }
}
