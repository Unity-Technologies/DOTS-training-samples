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
        m_CarriageQuery = GetEntityQuery(ComponentType.ReadOnly(typeof(CarriageVisualsSpawner)) );
        
        RequireForUpdate(m_CarriageQuery);
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var spawnerComponent = m_CarriageQuery.GetSingleton<CarriageVisualsSpawner>();
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        var needsVisualsComponentType = ComponentType.ReadOnly(typeof(NeedsVisuals));
        var hasVisualsComponentType = ComponentType.ReadOnly(typeof(HasVisuals));
            
        var jobHandle = Entities
            .WithName("TrainSpawnerSystem")
//                .WithReadOnly(spawner)
            .WithAll<NeedsVisuals>()
//            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, in LinePosition linePosition, in Translation translation, in Rotation rotation) =>
        {
            var visuals = commandBuffer.Instantiate(entityInQueryIndex, spawnerComponent.Prefab);
            commandBuffer.SetComponent(entityInQueryIndex, visuals, translation);
            commandBuffer.SetComponent(entityInQueryIndex, visuals, rotation);
            commandBuffer.SetComponent(entityInQueryIndex, visuals, new CarriageVisuals{LogicalEntity = entity });
            commandBuffer.RemoveComponent(entityInQueryIndex, entity, needsVisualsComponentType);
            commandBuffer.AddComponent(entityInQueryIndex, entity, hasVisualsComponentType);
            commandBuffer.SetComponent(entityInQueryIndex, entity, new HasVisuals{Visuals = visuals});
            
        }).Schedule(inputDeps);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}