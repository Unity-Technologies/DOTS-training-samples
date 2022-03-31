using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Color = UnityEngine.Color;


public partial class CollisionSystem : SystemBase
{
    private EntityCommandBufferSystem endEcbSystem;
    
    protected override void OnCreate()
    {
        endEcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    } 
    protected override void OnUpdate()
    {
        var playerEntity = GetSingletonEntity<PlayerTag>();
        var playerBounds = GetComponent<WorldRenderBounds>(playerEntity);

        var ecb = endEcbSystem.CreateCommandBuffer();
        var ecbParallel = ecb.AsParallelWriter();
        
        Entities
            .ForEach((int entityInQueryIndex, Entity entity, in CannonBallTag tag, in WorldRenderBounds bound) =>
            {
                if (bound.Value.ToBounds().Intersects(playerBounds.Value.ToBounds()))
                {
                    // Restart the game
                    ecbParallel.AddComponent<RestartTag>(entityInQueryIndex, playerEntity);
                }
            }).ScheduleParallel();
        endEcbSystem.AddJobHandleForProducer(Dependency);
    }
}