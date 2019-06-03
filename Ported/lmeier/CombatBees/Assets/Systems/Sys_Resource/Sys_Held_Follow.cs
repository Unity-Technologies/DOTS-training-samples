using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class Sys_Held_Follow : JobComponentSystem
{
    [ExcludeComponent(typeof(C_Stack))][RequireComponentTag(typeof(Tag_IsHeld))]
    struct Sys_Held_FollowJob : IJobForEachWithEntity<C_Held>
    {
        [ReadOnly] public ComponentDataFromEntity<Translation> TranslationType;
        [ReadOnly] public ComponentDataFromEntity<C_Holding> IsHoldingType;
        public float ResourceSize;

        public EntityCommandBuffer.Concurrent ecb;

        public void Execute(Entity ent, int index, [ReadOnly] ref C_Held Held)
        {
            if (IsHoldingType.Exists(Held.Holder))
            {
                float3 pos = TranslationType[Held.Holder].Value;

                pos.y -= ResourceSize;

                Translation newPos;
                newPos.Value = pos;

                ecb.SetComponent(index, ent, newPos);
            }
            else
            {
                ecb.RemoveComponent(index, ent, typeof(Tag_IsHeld));
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
        var job = new Sys_Held_FollowJob()
        {
            TranslationType = GetComponentDataFromEntity<Translation>(true),
            IsHoldingType = GetComponentDataFromEntity<C_Holding>(true),
            ResourceSize = ResourceManager.S.ResourceSize,
            ecb = m_entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        }.Schedule(this, inputDependencies);

        m_entityCommandBufferSystem.AddJobHandleForProducer(job);

        return job;
    }
}