using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;


// This system updates all entities in the scene with LbDestroy component.
//[UpdateAfter(typeof(EndInitializationEntityCommandBufferSystem))]
public class DestroySystem : JobComponentSystem
{
    EntityCommandBufferSystem m_Barrier;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<LbDestroyBarrier>();
    }

    // Use the [BurstCompile] attribute to compile a job with Burst.
    // You may see significant speed ups, so try it!
    //[BurstCompile]
    struct DestroyJob : IJobForEachWithEntity<LbDestroy>
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity entity, int jobIndex, [ReadOnly] ref LbDestroy destroyTag)
        {
            CommandBuffer.DestroyEntity(jobIndex, entity);
        }
    }

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();

        var job = new DestroyJob
        {
            CommandBuffer = commandBuffer,

        }.Schedule(this, inputDependencies);

        m_Barrier.AddJobHandleForProducer(job);

        return job;
    }
}
