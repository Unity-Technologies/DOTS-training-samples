using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class SpawnSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem bufferSystem;

    protected override void OnCreate()
    {
        bufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        var commandBuffer = bufferSystem.CreateCommandBuffer().ToConcurrent();


        var jobHandle = Entities.ForEach((Entity e, ref Spawner spawner) => 
               {
                   for (int i = 0; i < spawner.Amount; i++)
                   {
                       var instance = commandBuffer.Instantiate(0, spawner.Prefab);
                       commandBuffer.AddComponent(0, instance, new Translation() { Value = spawner.Position});
                   }

                   commandBuffer.DestroyEntity(0, e);

               })
            .Schedule(inputDeps);

        bufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}