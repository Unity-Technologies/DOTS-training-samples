using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(FoodBuilderSystem))]
public class AntSpawnerSystem : SystemBase
{
    private EntityCommandBufferSystem bufferSystem;
    protected override void OnCreate()
    {
        bufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<FoodBuilder>();
    }

    protected override void OnUpdate()
    {
        var ecb = bufferSystem.CreateCommandBuffer();

        float2 foodPos = this.GetSingleton<FoodBuilder>().foodLocation;
        var random = new Unity.Mathematics.Random(1234);

        Entities
            .WithAll<AntSpawner>()
            .WithNone<Initialized>()
            .ForEach((Entity entity, in AntSpawner spawner) =>
            {
                for (int i = 0; i < spawner.AntCount; ++i)
                {
                    var instance = ecb.Instantiate(spawner.AntPrefab);
                    var translation = new Translation { Value = new float3(0, 0, 0) };
                    var heading = new AntHeading { Degrees = random.NextFloat() * 360f };

                    ecb.SetComponent(instance, heading);
                    ecb.SetComponent(instance, translation);
                    ecb.AddComponent(instance, new CurrentTarget(){Value = foodPos});
                }
                
                ecb.AddComponent<Initialized>(entity);
            }).Schedule();

        bufferSystem.AddJobHandleForProducer(Dependency);
    }
}

public struct Initialized : IComponentData
{
    
}