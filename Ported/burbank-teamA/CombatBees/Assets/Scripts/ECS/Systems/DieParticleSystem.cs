using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public class DieParticlesSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem bufferSystem;


    protected override void OnCreate()
    {
        bufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        var commandBuffer = bufferSystem.CreateCommandBuffer().ToConcurrent();

        Random r = new Random((uint)UnityEngine.Random.Range(1, 100));
        float dt = Time.deltaTime;

        var jobHandle = Entities.ForEach((Entity e, ref DieTime dieTime) =>
               {
                   dieTime.Value -= dt;
                   if (dieTime.Value < 0)
                   {
                       commandBuffer.DestroyEntity(0, e);
                   }
               })
            .Schedule(inputDeps);
               

        bufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}