using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class SpawnSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        var jobHandle = Entities
            .WithName("SpawnerSystem")
            .ForEach((Entity entity, ref Spawner spawner) =>
            {
                spawner.Dummy += 1;
                if (spawner.Dummy % 50 == 0)
                {
                    //Debug.Log("Spawn!");
                    //Debug.Log(spawner.Dummy);
                }
                //var instance = commandBuffer.Instantiate(entityInQueryIndex, spawnerFromEntity.Prefab);
                //commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation {Value = position});
                
            }).Schedule(inputDeps);
        
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}
