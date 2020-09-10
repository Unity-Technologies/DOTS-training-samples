using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[ExecuteAlways]
public class ApplyColorSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    
    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        
        Entities.ForEach((int entityInQueryIndex, Entity entity, in ColorAuthoring auth) =>
        {
            var color = new float4(auth.Color.r, auth.Color.g, auth.Color.b, 1);
            ecb.AddComponent(entityInQueryIndex, entity, new Color(){Value = color});
            ecb.RemoveComponent<ColorAuthoring>(entityInQueryIndex, entity);
        }).ScheduleParallel();

        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}