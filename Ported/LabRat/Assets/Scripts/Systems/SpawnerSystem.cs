using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class SpawnerSystem : SystemBase
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
        var rnd = new Random(((uint)UnityEngine.Random.Range(1, 100000)));
        
        Entities.ForEach((int entityInQueryIndex, Entity spawnerEntity, ref Spawner spawner, 
            in PositionXZ positionXz) =>
        {
            if (spawner.TotalSpawned >= spawner.Max)
                return;
            
            spawner.Counter += dt;
            if (!(spawner.Counter > spawner.Frequency)) return;
            spawner.Counter -= spawner.Frequency;
            
            var instance = ecb.Instantiate(entityInQueryIndex, spawner.Prefab);
            ecb.SetComponent(entityInQueryIndex, instance, new PositionXZ() {Value = positionXz.Value});
            
            // set random scale
            var scale = spawner.Enable ? rnd.NextFloat( spawner.MinScale,  spawner.MaxScale) :  spawner.Scale;
            ecb.AddComponent(entityInQueryIndex, instance, new Size(){Value = scale});

            // set random speed
            var speed = rnd.NextFloat( spawner.MinSpeed,  spawner.MaxSpeed);
            ecb.AddComponent(entityInQueryIndex, instance, new Speed(){Value = speed});
            
            spawner.TotalSpawned++;
        }).ScheduleParallel();

        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}