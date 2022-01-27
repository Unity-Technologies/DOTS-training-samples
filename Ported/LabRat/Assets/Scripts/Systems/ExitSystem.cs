using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public partial class ExitSystem: SystemBase
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
        var removeQueue = mPlayerUpdateSystem.RemovePointsFromPlayerQueue.AsParallelWriter();

        var addPointsJob = Entities
            .WithName("AddPointsJob")
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
            .ScheduleParallel(Dependency);

        var removePointsJob = Entities
            .WithName("RemovePointsJob")
            .WithAll<Cat>()
            .WithReadOnly(exitTiles)
            .WithReadOnly(exitOwners)
            .ForEach((Entity entity, int entityInQueryIndex, in Tile catTile) =>
            {

                for (int i = 0; i < exitTiles.Length; ++i)
                {
                    if (catTile.Coords.Equals(exitTiles[i].Coords))
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                        // enqueue a point loss for the PlayerUpdateSystem
                        removeQueue.Enqueue(exitOwners[i].Owner);
                    }
                }
            })
            .ScheduleParallel(addPointsJob);

        JobHandle intermediateDependencies = JobHandle.CombineDependencies(addPointsJob, removePointsJob);

        Dependency = JobHandle.CombineDependencies(exitTiles.Dispose(intermediateDependencies), exitOwners.Dispose(intermediateDependencies));

        mECBSystem.AddJobHandleForProducer(Dependency);
    }
}
