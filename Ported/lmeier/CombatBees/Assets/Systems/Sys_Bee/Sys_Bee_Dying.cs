using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class Sys_Bee_Dying : JobComponentSystem
{

    [RequireComponentTag(typeof(Tag_Bee), typeof(Tag_IsDying))]
    struct Sys_Bee_DyingJob : IJobForEachWithEntity<C_DeathTimer, C_Random, Translation>
    {
        public EntityCommandBuffer.Concurrent ecb;
        [ReadOnly] public Entity BloodPrefab;

        public void Execute(Entity ent, int index, [ReadOnly] ref C_DeathTimer Timer, ref C_Random Rand, [ReadOnly] ref Translation Position)
        {
            if (Rand.Generator.NextFloat(0f, 1f) > (Timer.TimeRemaining - .5f) * .5f)
                return;

            //Spawn particles;
            var bloodVel = new C_Velocity()
            {
                Value = float3(0f)
            };

            var blood = ecb.Instantiate(index, BloodPrefab);
            ecb.SetComponent(index, blood, Position);
            ecb.SetComponent(index, blood, bloodVel);
        }
    }

    private EntityCommandBufferSystem m_entityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_entityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new Sys_Bee_DyingJob()
        {
            ecb = m_entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            BloodPrefab = GameConstants.S.BloodEnt
        }.Schedule(this, inputDependencies);

        m_entityCommandBufferSystem.AddJobHandleForProducer(job);

        return job;
    }
}