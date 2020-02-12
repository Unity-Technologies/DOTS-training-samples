using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public struct LifeTime : IComponentData
{
    public float Value;
}

// This system updates all entities in the scene with both a RotationSpeed_SpawnAndRemove and Rotation component.
public class LifeTimeSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_Barrier;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDep)
    {
        var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();

        var deltaTime = Time.DeltaTime;
        var outDep = Entities.ForEach((Entity entity, int nativeThreadIndex, ref LifeTime lifetime) =>
        {
            lifetime.Value -= deltaTime;

            if (lifetime.Value < 0.0f)
            {
                commandBuffer.DestroyEntity(nativeThreadIndex, entity);
            }
        }).Schedule(inputDep);
            
        m_Barrier.AddJobHandleForProducer(outDep);
        return outDep;
    }
}
