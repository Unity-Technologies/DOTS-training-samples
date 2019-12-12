using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class KillSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        float deltaTime = Time.DeltaTime;
        EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        EntityCommandBuffer.Concurrent concurrentBuffer = entityCommandBuffer.ToConcurrent();
        JobHandle jobHandle = Entities
            .WithAll<MarkedForDeath>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Scale scale, in Translation pos) =>
        {
           if (pos.Value.y < -1.0f)
            {
                scale.Value -= deltaTime / 0.3f;
            }
            if (scale.Value <= 0.0f)
            {
                concurrentBuffer.DestroyEntity(entityInQueryIndex, entity);
            }
        }).Schedule(inputDependencies);
        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        // Now that the job is set up, schedule it to be run. 
        return jobHandle;
    }
}