using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class FarmerIntentionSystem : SystemBase
{
    Random random;

    private EndSimulationEntityCommandBufferSystem m_ECB;
    
    protected override void OnCreate()
    {
        base.OnCreate();

        random = new Random(1234);
        
        m_ECB = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var ecb = m_ECB.CreateCommandBuffer().AsParallelWriter();

        var localRandom = random;
        
        Dependency = Entities
            .WithAll<Farmer>()
            .WithNone<SmashRockIntention>()
            .WithNone<TillGroundIntention>()
            .WithNone<PlantCropIntention>()
            .WithNone<SellPlantIntention>()
            .ForEach((Entity entity,  int entityInQueryIndex) =>
            {
                switch(localRandom.NextInt(0, 3))
                {
                    case 0:
                        ecb.AddComponent(entityInQueryIndex, entity, new SmashRockIntention());
                        break;
                    case 1:
                        ecb.AddComponent(entityInQueryIndex, entity, new TillGroundIntention { Rect = default });
                        break;
                    case 2:
                        ecb.AddComponent(entityInQueryIndex, entity, new PlantCropIntention());
                        break;
                    //case 3:
                    //    ecb.AddComponent(entity, new SellPlantIntention());
                    //    break;
                }
            }).ScheduleParallel(Dependency);

        random = localRandom;
        
        m_ECB.AddJobHandleForProducer(Dependency);
    }
}