using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateInGroup(typeof(TransitionSystemGroup))]
class TimerSystem : JobComponentSystem
{
    EntityQuery m_TimerQuery;

    protected override void OnCreate()
    {
        m_TimerQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite<TimerComponent>()
                },
                Any = new[]
                {
                    ComponentType.ReadOnly<DOORS_OPEN>(),
                    ComponentType.ReadOnly<DOORS_CLOSE>()
                }
            });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new TimerJob{dt = Time.deltaTime}.Schedule(m_TimerQuery, inputDeps);
    }

    [BurstCompile(FloatMode = FloatMode.Fast)]
    struct TimerJob : IJobForEach<TimerComponent>
    {
        public float dt;
        public void Execute(ref TimerComponent timer)
        {
            timer.value += dt;
        }
    }
}
