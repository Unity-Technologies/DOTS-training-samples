using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class ApplySpeedSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    
    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var rnd = new Random(((uint)UnityEngine.Random.Range(1, 100000)));
        
        Entities.ForEach((int entityInQueryIndex, Entity entity, in SpeedAuthoring auth) =>
        {
            var value = rnd.NextFloat( auth.MinSpeed,  auth.MaxSpeed);
            ecb.AddComponent(entityInQueryIndex, entity, new Speed(){Value = value});
            ecb.RemoveComponent<SpeedAuthoring>(entityInQueryIndex, entity);
        }).ScheduleParallel();

        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}