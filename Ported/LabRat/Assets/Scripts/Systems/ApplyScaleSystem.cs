using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class ApplyScaleSystem : SystemBase
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
        
        var rnd = new Random(((uint)UnityEngine.Random.Range(1, 100000)));
        Entities.ForEach((int entityInQueryIndex, Entity entity, in ScaleAuthoring auth) =>
        {
            var value = auth.Enable ? rnd.NextFloat( auth.MinScale,  auth.MaxScale) :  auth.Scale;
            ecb.AddComponent(entityInQueryIndex, entity, new Size(){Value = value});
            ecb.RemoveComponent<ScaleAuthoring>(entityInQueryIndex, entity);
        }).ScheduleParallel();

        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}