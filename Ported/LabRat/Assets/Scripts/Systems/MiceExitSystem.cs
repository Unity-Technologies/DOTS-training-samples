using Unity.Collections;
using Unity.Entities;

public partial class MiceExitSystem: SystemBase
{
    private EntityCommandBufferSystem mECBSystem;
    private PlayerUpdateSystem mPlayerUpdateSystem;
    private EntityQuery exitsQuery;

    protected override void OnCreate()
    {
        mECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        mPlayerUpdateSystem = World.GetExistingSystem<PlayerUpdateSystem>();
        exitsQuery = GetEntityQuery(
            ComponentType.ReadOnly<Exit>(),
            ComponentType.ReadOnly<Tile>(),
            ComponentType.ReadOnly<PlayerOwned>());
        RequireForUpdate(exitsQuery);
        RequireSingletonForUpdate<GameRunning>();
    }

    protected override void OnUpdate()
    {
        var ecb = mECBSystem.CreateCommandBuffer().AsParallelWriter();

        var exitTiles = exitsQuery.ToComponentDataArray<Tile>(Allocator.TempJob);
        
        var exitOwners = exitsQuery.ToComponentDataArray<PlayerOwned>(Allocator.TempJob);

        var pointsQueue = mPlayerUpdateSystem.AddPointsToPlayerQueue.AsParallelWriter();
        
        Entities
            .WithAll<Mouse>()
            .WithReadOnly(exitTiles)
            .WithReadOnly(exitOwners)
            .ForEach((Entity entity, int entityInQueryIndex, in Tile mouseTile) =>
            {
                
                for (int i = 0; i < exitTiles.Length; ++i)
                {
                    if (mouseTile.Coords.Equals(exitTiles[i].Coords))
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                        // enqueue a point increment for the PlayerUpdateSystem
                        pointsQueue.Enqueue(exitOwners[i].Owner);
                    }
                }
            })
            .WithDisposeOnCompletion(exitTiles)
            .WithDisposeOnCompletion(exitOwners)
            .ScheduleParallel();
        
        mECBSystem.AddJobHandleForProducer(Dependency);
    }
}
