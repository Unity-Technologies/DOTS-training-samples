using Unity.Collections;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

// This system updates all entities in the scene with LbLifetime component.
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class LifeTimeSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_Barrier;
    private NativeQueue<Entity> m_Queue;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<LbSimulationBarrier>();
        m_Queue = new NativeQueue<Entity>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        m_Queue.Dispose();
    }

    [BurstCompile]
    struct LifeTimeJob : IJobForEachWithEntity<LbLifetime>
    {
        public float DeltaTime;
        public NativeQueue<Entity>.ParallelWriter Queue;
        
        public void Execute(Entity entity, int jobIndex, ref LbLifetime lifeTime)
        {
            lifeTime.Value -= DeltaTime;
            if (lifeTime.Value < 0.0f)
            {
                Queue.Enqueue(entity);
            }
        }
    }

    struct LifeTimeCleanJob : IJob
    {
        public EntityCommandBuffer CommandBuffer;
        public NativeQueue<Entity> Queue;

        public void Execute()
        {
            while (Queue.Count > 0)
                CommandBuffer.AddComponent(Queue.Dequeue(), new LbDestroy());
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var handle = new LifeTimeJob
        {
            DeltaTime = Time.deltaTime,
            Queue = m_Queue.AsParallelWriter()
        }.Schedule(this, inputDependencies);

        handle = new LifeTimeCleanJob()
        {
            Queue = m_Queue,
            CommandBuffer =  m_Barrier.CreateCommandBuffer()
        }.Schedule(handle);
        
        m_Barrier.AddJobHandleForProducer(handle);

        return handle;
    }
}
