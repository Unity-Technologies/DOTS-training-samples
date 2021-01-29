    
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class HomeBuilderSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        
        Entities
            .WithAll<HomeBuilder>()
            .WithNone<Initialized>()
            .ForEach((Entity entity, HomeBuilder homeBuilder) =>
            {
                Entity homeEntity = ecb.Instantiate(homeBuilder.homePrefab);
                Translation homeTranslation = new Translation { Value = new float3() };
                
                ecb.SetComponent(homeEntity, homeTranslation);
                ecb.AddComponent(homeEntity, new URPMaterialPropertyBaseColor
                {
                    Value = homeBuilder.homeColor
                });
                ecb.AddComponent<Home>(homeEntity);
                
                ecb.AddComponent(entity,new Initialized());
            }).Run();
        
        ecb.Playback(EntityManager);
    }
}
