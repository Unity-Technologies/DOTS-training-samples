using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class CarSpawnerSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithName("CarSpawnerSystem")
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, in CarSpawnerAuthoring spawner) =>
            {
                for (var i = 0; i < spawner.Count; i++)
                {
                    var instance = commandBuffer.Instantiate(entityInQueryIndex, spawner.CarPrefab);
                    commandBuffer.AddComponent<Disabled>(entityInQueryIndex,instance);
                }

                //Disable spawner in case we need it again for game restart
                commandBuffer.AddComponent<Disabled>(entityInQueryIndex,entity);
            }).ScheduleParallel();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}