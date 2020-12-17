using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class FarmerIntentionSystem : SystemBase
{
    Random random;

    protected override void OnCreate()
    {
        random = new Random(1234);
    }
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var localRandom = random;
        Entities
            .WithAll<Farmer>()
            .WithNone<SmashRockIntention>()
            .WithNone<TillGroundIntention>()
            .WithNone<PlantCropIntention>()
            .WithNone<SellPlantIntention>()
            .ForEach((Entity entity) =>
            {
                switch(localRandom.NextInt(0, 3))
                {
                    case 0:
                        ecb.AddComponent(entity, new SmashRockIntention());
                        break;
                    case 1:
                        ecb.AddComponent(entity, new TillGroundIntention { Rect = default });
                        break;
                    case 2:
                        ecb.AddComponent(entity, new PlantCropIntention());
                        break;
                    //case 3:
                    //    ecb.AddComponent(entity, new SellPlantIntention());
                    //    break;
                }
            }).Run();

        random = localRandom;
        ecb.Playback(EntityManager);
    }
}