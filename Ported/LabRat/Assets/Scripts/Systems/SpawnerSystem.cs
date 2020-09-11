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
        
        Entities.ForEach((int entityInQueryIndex, Entity spawnerEntity, ref Spawner spawner, in PositionXZ positionXz) =>
        {
            if (spawner.TotalSpawned >= spawner.Max)
                return;
            
            spawner.Counter += dt;
            if (!(spawner.Counter > spawner.Frequency)) return;
            spawner.Counter -= spawner.Frequency;
            
            var instance = ecb.Instantiate(entityInQueryIndex, spawner.Prefab);
            ecb.SetComponent(entityInQueryIndex, instance, new PositionXZ() {Value = positionXz.Value});
            ecb.SetComponent(entityInQueryIndex, instance, new RotationY(){Value =  rnd.NextFloat(0, 6.28f)});
            
            
            spawner.TotalSpawned++;
        }).ScheduleParallel();

        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}