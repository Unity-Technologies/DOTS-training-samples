using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(TransitionSystemGroup))]
class DoorsOpenTransitionSystem : JobComponentSystem
{
    EntityQuery m_TimerQuery;

    protected override void OnCreate()
    {
        m_TimerQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite<Timer>()
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

    struct TimerJob : IJobForEachWithEntity<Timer>
    {
        public float dt;
        public void Execute(ref Timer timer)
        {
            timer.value += dt;
        }
    }
}
