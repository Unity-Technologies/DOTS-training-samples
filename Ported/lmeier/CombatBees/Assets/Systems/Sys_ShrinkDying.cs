using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class Sys_ShrinkDying : JobComponentSystem
{

    [RequireComponentTag(typeof(Tag_IsDying))][ExcludeComponent(typeof(Tag_Particle_Init))]
    struct Sys_ShrinkDyingJob : IJobForEachWithEntity<C_Size, C_DeathTimer>
    {
        [ReadOnly] public float dt;
        public EntityCommandBuffer.Concurrent ecb;

        public void Execute(Entity ent, int index, ref C_Size size, ref C_DeathTimer timer)
        {
            timer.TimeRemaining -= dt;
            if (timer.TimeRemaining > 0)
            {
                if (timer.TimeRemaining < 1.0f)
                {
                    size.Value *= sqrt(timer.TimeRemaining);
                }
            }
            else
            {
                ecb.DestroyEntity(index, ent);
            }
        }
    }

    EntityCommandBufferSystem m_entityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_entityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();

    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new Sys_ShrinkDyingJob()
        {
            dt = UnityEngine.Time.deltaTime,
            ecb = m_entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        }.Schedule(this, inputDependencies);

        m_entityCommandBufferSystem.AddJobHandleForProducer(job);

        return job;
    }
}