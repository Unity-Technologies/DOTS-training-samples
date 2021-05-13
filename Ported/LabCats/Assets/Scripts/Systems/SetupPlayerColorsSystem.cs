using Unity.Collections;
using Unity.Entities;
using Unity.Entities.CodeGeneratedJobForEach;
using Unity.Mathematics;
using Unity.Rendering;

public class SetupPlayerColorsSystem : SystemBase
{
    private EntityQuery RequirePropagation;
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Dependency = Entities.WithAll<ShouldSetupColor>().ForEach((Entity e, int entityInQueryIndex, ref URPMaterialPropertyBaseColor urpColor, in PlayerIndex playerIndex) =>
        {
            if(playerIndex.Value == 0)
                urpColor.Value = new float4(0.0f, 0.0f, 0.0f, 1.0f);
            else if (playerIndex.Value == 1)
                urpColor.Value = new float4(0.0f, 1.0f, 0.0f, 1.0f);
            else if (playerIndex.Value == 2)
                urpColor.Value = new float4(0.0f, 0.0f, 1.0f, 1.0f);
            else
                urpColor.Value = new float4(1.0f, 0.0f, 0.0f, 1.0f);
            ecb.RemoveComponent<ShouldSetupColor>(entityInQueryIndex, e);
        }).ScheduleParallel(Dependency);
        
        
        var cdfe = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>();
        
        Dependency = Entities
            .WithNativeDisableContainerSafetyRestriction(cdfe)
            .WithStoreEntityQueryInField(ref RequirePropagation)
            .WithAll<PropagateColor>()
            .ForEach((Entity e, int entityInQueryIndex, in DynamicBuffer<LinkedEntityGroup> group
                , in URPMaterialPropertyBaseColor color) =>
            {
                for (int i = 1; i < group.Length; ++i)
                {
                    cdfe[group[i].Value] = color;
                }
                ecb.RemoveComponent<PropagateColor>(entityInQueryIndex, e);
            }).ScheduleParallel(Dependency);
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
