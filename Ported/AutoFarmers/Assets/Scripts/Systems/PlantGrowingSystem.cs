using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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

        float deltaTime = Time.DeltaTime;

        Entities
            .WithAll<Plant_Tag>()
            .WithNone<FullyGrownPlant_Tag>().ForEach((int entityInQueryIndex, Entity entity, ref Health health) =>
        {
            health.Value += deltaTime * FarmConstants.PlantGrowingScale;
            if (health.Value >= 1.0f)
            {
                ecb.AddComponent<FullyGrownPlant_Tag>(entityInQueryIndex, entity, new FullyGrownPlant_Tag());
            }
        }).ScheduleParallel();
        
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

public struct FarmConstants
{
    public static readonly float PlantGrowingScale = 0.1f; // 2 Seconds to mature 
}
