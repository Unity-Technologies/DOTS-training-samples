using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;


[UpdateInGroup(typeof(SimulationSystemGroup))]
public class Sys_Spawn : JobComponentSystem
{

    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }
    struct SpawnJob : IJobForEachWithEntity<C_Spawner, LocalToWorld>
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref C_Spawner spawner,
            [ReadOnly] ref LocalToWorld location)
        {
            for (int i = 0; i < spawner.Count; i++)
            {
                var instance = CommandBuffer.Instantiate(index, spawner.Prefab);

                // Place the instantiated in a grid with some noise
                var position = location.Position;
                CommandBuffer.SetComponent(index, instance, new Translation { Value = position });
            }

            CommandBuffer.DestroyEntity(index, entity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        var job = new SpawnJob
        {
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()

        }.Schedule(this, inputDeps);

        m_EntityCommandBufferSystem.AddJobHandleForProducer(job);

        return job;
    }
}