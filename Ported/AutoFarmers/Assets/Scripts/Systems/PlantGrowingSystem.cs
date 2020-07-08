using Unity.Entities;
using Unity.Mathematics;

public class PlantGrowingSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        
        Entities.WithNone<FullyGrownPlant_Tag>().ForEach((int entityInQueryIndex, Entity entity, ref Plant plant) =>
        {
            plant.Age += 1;
            if (plant.Age >= FarmConstants.PlantMaturityAge)
            {
                ecb.AddComponent<FullyGrownPlant_Tag>(entityInQueryIndex, entity, new FullyGrownPlant_Tag());
            }
        }).ScheduleParallel();
        
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

public struct FarmConstants
{
    public static int PlantMaturityAge = 40;
}
