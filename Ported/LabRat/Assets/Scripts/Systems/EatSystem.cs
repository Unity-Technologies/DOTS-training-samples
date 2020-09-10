using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class EatSystem : SystemBase
{
    private AnimalMovementSystem m_AnimalMovementSystem;
    private EntityCommandBufferSystem m_CommandBufferSystem;
    private EntityQuery m_CatsQuery;
    
    protected override void OnCreate()
    {
        m_AnimalMovementSystem = World.GetExistingSystem<AnimalMovementSystem>();
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
        var boardDimensions = m_AnimalMovementSystem.Dimensions;
        
        // find the tile for each cat
        var numCats = m_CatsQuery.CalculateEntityCount();
        var catTiles = new NativeArray<int>(numCats, Allocator.TempJob);
        var catData = new NativeArray<bool>(numCats, Allocator.TempJob);
        Entities
            .WithAll<CatTag>()
            .ForEach((int entityInQueryIndex, Entity entity, in PositionXZ position) =>
            {
                var tile = AnimalMovementSystem.TileKeyFromPosition(position.Value, boardDimensions);
                catTiles[entityInQueryIndex] = tile;
                catData[entityInQueryIndex] =  false;
            })
            .ScheduleParallel();
        
        //test if a rat is on one of those tiles
        Entities
            .WithAll<RatTag>()
            .WithDisposeOnCompletion(catTiles)
            .WithNativeDisableParallelForRestriction(catData)
            .ForEach((int entityInQueryIndex, Entity entity, in PositionXZ position) =>
            {
                var ratTile = AnimalMovementSystem.TileKeyFromPosition(position.Value, boardDimensions);

                for (var i = 0; i < catTiles.Length; ++i)
                {
                    if (ratTile == catTiles[i])
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                        catData[i] = true;
                        break;
                    }
                }
            })
            .ScheduleParallel();

        
        Entities
            .WithAll<CatTag>()
            .WithNativeDisableParallelForRestriction(catData)
            .WithDisposeOnCompletion(catData)
            .ForEach((int entityInQueryIndex, Entity entity, ref Size size) =>
            {
                if (catData[entityInQueryIndex])
                {
                    size.Grow = 0.5f;
                }
            })
            .ScheduleParallel();
        
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
