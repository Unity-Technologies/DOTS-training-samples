using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class UpdateScaleGrownSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    
    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var dt = UnityEngine.Time.deltaTime;
        
        Entities
            .WithAll<CatTag>()
            .ForEach((int entityInQueryIndex, Entity entity, ref SizeGrown size) =>
            {
                if (size.Grow > 0)
                {
                    size.Grow -= 0.2f * dt;
                }
                else
                {
                    ecb.RemoveComponent<SizeGrown>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();

        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
