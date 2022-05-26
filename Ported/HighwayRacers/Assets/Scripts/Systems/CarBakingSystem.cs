using Unity.Entities;
using Unity.Rendering;

[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
partial class CarBakingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithEntityQueryOptions(EntityQueryOptions.IncludePrefab)
            .WithImmediatePlayback()
            .ForEach((DynamicBuffer<ChildrenWithRenderer> group, EntityCommandBuffer ecb) =>
            {
                var entities = group.AsNativeArray().Reinterpret<Entity>();
                ecb.AddComponent(entities, new URPMaterialPropertyBaseColor());
            }).Run();
    }
}