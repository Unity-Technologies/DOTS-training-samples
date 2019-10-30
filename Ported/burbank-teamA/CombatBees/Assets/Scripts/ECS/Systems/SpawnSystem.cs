using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

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

        Random r = new Random((uint)UnityEngine.Random.Range(0, 100));


        var jobHandle = Entities.ForEach((Entity e, ref Spawner spawner, ref Translation t) => 
               {
                   for (int i = 0; i < spawner.Amount; i++)
                   {

                       float3 randV = r.NextFloat3();

                       var instance = commandBuffer.Instantiate(0, spawner.Prefab);
                       commandBuffer.AddComponent(0, instance, new Translation() { Value = t.Value});
                       commandBuffer.SetComponent(0, instance, new Velocity() { Value = randV });
                   }

                   commandBuffer.DestroyEntity(0, e);

               })
            .Schedule(inputDeps);

        bufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}