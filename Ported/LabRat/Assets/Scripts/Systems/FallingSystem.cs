using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class FallingSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    
    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var dt = UnityEngine.Time.deltaTime;

        Entities
            .ForEach((int entityInQueryIndex, Entity entity,ref Falling falling, in Speed speed) =>
        {
            falling.y -= speed.Value * dt;
            if (falling.y <= -15.0)
            {
                ecb.DestroyEntity(entityInQueryIndex, entity);
            }
            
        }).ScheduleParallel();
        
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
