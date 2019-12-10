using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class SpawnerSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref SpawnerComponent spawner) =>
        {
            spawner.timeToNextSpawn -= deltaTime;
            while (spawner.timeToNextSpawn < 0)
            {
                var x = spawner.random.NextFloat(-spawner.extend.x, spawner.extend.x) / 2f;
                var y = spawner.random.NextFloat(-spawner.extend.y, spawner.extend.y) / 2f;
                var z = spawner.random.NextFloat(-spawner.extend.z, spawner.extend.z) / 2f;
                //var y = spawner.random.NextInt(0, rowCount - 1);
                //var z = spawner.random.NextInt(0, rowCount - 1);
                var entity = commandBuffer.Instantiate(spawner.spawnEntity);
                var c = spawner.center;
                commandBuffer.SetComponent(entity, new Translation { Value = c + new float3(x, y, z) });

                spawner.timeToNextSpawn += 1f / spawner.frequency;
            }

        }).Run();

        return inputDependencies;
    }

}
