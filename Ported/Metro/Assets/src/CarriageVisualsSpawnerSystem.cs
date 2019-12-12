using src;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class CarriagesVisualsSpawnerSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    EntityQuery m_CarriageQuery;
    
    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        m_CarriageQuery = GetEntityQuery(
            ComponentType.ReadOnly(typeof(LinePosition)),
            ComponentType.ReadOnly(typeof(Translation)),
            ComponentType.ReadOnly(typeof(Rotation)),
            ComponentType.ReadOnly(typeof(LocalToWorld))
            );
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        var carriages = m_CarriageQuery.ToEntityArray(Allocator.TempJob);
        var carriagePositions = m_CarriageQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var carriageRotation = m_CarriageQuery.ToComponentDataArray<Rotation>(Allocator.TempJob);
        
        var jobHandle = Entities
            .WithName("TrainSpawnerSystem")
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, in CarriageVisualsSpawner spawnerFromEntity) =>
        {
            for (int i = 0; i < carriages.Length; i++)
            {
                var carriage = commandBuffer.Instantiate(entityInQueryIndex, spawnerFromEntity.Prefab);
                commandBuffer.SetComponent(entityInQueryIndex, carriage, carriagePositions[i]);
                commandBuffer.SetComponent(entityInQueryIndex, carriage, carriageRotation[i]);
                commandBuffer.SetComponent(entityInQueryIndex, carriage, new CarriageVisuals{LogicalEntity = carriages[i] });
            }
            
            // destroy the spanwer and therefore ensure the system won't run again.
            commandBuffer.DestroyEntity(entityInQueryIndex, entity);
        }).Schedule(inputDeps);
        
        carriages.Dispose(jobHandle);
        carriagePositions.Dispose(jobHandle);
        carriageRotation.Dispose(jobHandle);
        
        m_CarriageQuery.AddDependency(jobHandle);
        
        return jobHandle;
    }
}