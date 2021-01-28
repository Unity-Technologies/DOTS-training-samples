
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateAfter(typeof(SpawnFromFoodSystem))]
public class BeeRandomInitializationSystem: SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities
            .WithNone<RandomComponent>()
            .WithAll<BeeTag>()
            .ForEach((Entity e) =>
            {
                ecb.AddComponent(e, new RandomComponent
                {
                    // TODO: Seed correctly
                    Value = new Random(1234),
                });
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
