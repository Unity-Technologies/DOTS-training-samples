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
            .WithNone<SmashRocks>()
            .WithNone<TillGround>()
            .WithNone<PlantCrop>()
            .WithNone<SellPlant>()
            .ForEach((Entity entity) =>
            {
                switch(random.NextInt(0, 4))
                {
                    case 0:
                        ecb.AddComponent(entity, new SmashRocks());
                        break;
                    case 1:
                        ecb.AddComponent(entity, new TillGround());
                        break;
                    case 2:
                        ecb.AddComponent(entity, new PlantCrop());
                        break;
                    case 3:
                        ecb.AddComponent(entity, new SellPlant());
                        break;
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}