using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public struct SpawnTrains : IComponentData
{
    public Entity prefab;
}

public class SpawnTrainsSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_Barrier;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new SpawnTrainsJob{ cmdBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent()}.Schedule(this, inputDeps);
        m_Barrier.AddJobHandleForProducer(job);
        return job;
    }

    struct SpawnTrainsJob : IJobForEachWithEntity<SpawnTrains>
    {
        [WriteOnly] public EntityCommandBuffer.Concurrent cmdBuffer;
        public void Execute(Entity entity, int index, ref SpawnTrains c0)
        {
            var train = cmdBuffer.Instantiate(index, c0.prefab);
            cmdBuffer.SetComponent(index, train, new Translation{ Value = new float3(0, 0, 0)});
            cmdBuffer.RemoveComponent(index, entity, typeof(SpawnTrains));
        }
    }
}