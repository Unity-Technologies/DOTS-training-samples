
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateAfter(typeof(SpawnFromFoodSystem))]
public class BeeRandomInitializationSystem: SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var random = GetSingleton<Randomizer>();
        Entities
            .WithNone<RandomComponent>()
            .WithAll<BeeTag>()
            .ForEach((Entity e) =>
            {
                ecb.AddComponent(e, new RandomComponent
                {
                    Value = new Random(random.Random.NextUInt()),
                });
            }).Run();
        
        SetSingleton(random);
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
