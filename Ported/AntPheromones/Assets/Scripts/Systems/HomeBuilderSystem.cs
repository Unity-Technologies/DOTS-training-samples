    
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

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
                Entity home = ecb.Instantiate(homeBuilder.homePrefab);
                Translation homeTranslation = new Translation { Value = new float3() };
                
                ecb.SetComponent(home, homeTranslation);
                ecb.AddComponent(home, new URPMaterialPropertyBaseColor
                {
                    Value = homeBuilder.homeColor
                });
            }).Run();
        
    }
}
