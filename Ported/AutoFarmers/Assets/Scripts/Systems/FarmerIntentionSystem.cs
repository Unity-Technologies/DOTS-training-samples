using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class FarmerIntentionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var random = new Unity.Mathematics.Random(1234);
        Entities
            .WithAll<Farmer>()
            .WithNone<SmashRockIntention>()
            .WithNone<TillGroundIntention>()
            .WithNone<PlantCropIntention>()
            .WithNone<SellPlantIntention>()
            .ForEach((Entity entity) =>
            {
                switch(random.NextInt(0, 4))
                {
                    case 0:
                        ecb.AddComponent(entity, new SmashRockIntention());
                        break;
                    case 1:
                        ecb.AddComponent(entity, new TillGroundIntention());
                        break;
                    case 2:
                        ecb.AddComponent(entity, new PlantCropIntention());
                        break;
                    case 3:
                        ecb.AddComponent(entity, new SellPlantIntention());
                        break;
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}