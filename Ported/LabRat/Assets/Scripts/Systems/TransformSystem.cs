using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(TransformSystemGroup))]
public class TransformSystem : SystemBase
{
    private EntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        this.ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = this.ecbSystem.CreateCommandBuffer();

        Entities.WithName("L2WFromPositionXZ")
            .WithStructuralChanges()
            .WithChangeFilter<PositionXZ>()
            .ForEach((Entity entity, in LocalToWorld l2w, in PositionXZ pos) =>
            {
                ecb.SetComponent(entity, new Translation
                {
                    Value = l2w.Position + new float3(pos.Value.x, 0, pos.Value.y)
                });
            }).Run();
    }
}