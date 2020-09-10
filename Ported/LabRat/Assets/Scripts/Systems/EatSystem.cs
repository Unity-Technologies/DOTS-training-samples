using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class EatSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    private EntityQuery m_CatsQuery;
    
    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
        
        m_CatsQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<CatTag>()
            }
        });
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        // find the tile for each cat
        var numCats = m_CatsQuery.CalculateEntityCount();
        var catTiles = new NativeArray<int>(numCats, Allocator.TempJob);
        Entities
            .WithAll<CatTag>()
            .ForEach((int entityInQueryIndex, Entity entity, in PositionXZ position) =>
            {
                var tile = AnimalMovementSystem.TileKeyFromPosition(position.Value);
                catTiles[entityInQueryIndex] = tile;
            })
            .ScheduleParallel();
        
        //test if a rat is on one of those tiles
        Entities
            .WithAll<RatTag>()
            .WithDisposeOnCompletion(catTiles)
            .ForEach((int entityInQueryIndex, Entity entity, in PositionXZ position) =>
            {
                var ratTile = AnimalMovementSystem.TileKeyFromPosition(position.Value);

                for (var i = 0; i < catTiles.Length; ++i)
                {
                    if (ratTile == catTiles[i])
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                        break;
                    }
                }
            })
            .ScheduleParallel();
        
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
