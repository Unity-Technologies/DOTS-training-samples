using Unity.Entities;
using Unity.Mathematics;

public class PlantMakeHarvestableSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .WithAll<FullyGrownPlant_Tag>()            
            .WithNone<HarvestablePlant_Tag>()
            .ForEach((int entityInQueryIndex, Entity entity, ref Position position) =>
        {
            //UpdatePlantHarvestableGrid(position);
            ecb.AddComponent<HarvestablePlant_Tag>(entityInQueryIndex, entity, new HarvestablePlant_Tag());
        }).ScheduleParallel();
        
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

