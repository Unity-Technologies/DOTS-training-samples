using Unity.Collections;
using Unity.Entities;

public partial class MiceExitSystem: SystemBase
{
    private EntityCommandBufferSystem mECBSystem;

    protected override void OnCreate()
    {
        mECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var exitsQuery = GetEntityQuery(typeof(Exit), typeof(Tile));
        if (exitsQuery.IsEmpty)
        {
            return;
        }
        
        var ecb = mECBSystem.CreateCommandBuffer().AsParallelWriter();

        var exitTiles = exitsQuery.ToComponentDataArray<Tile>(Allocator.TempJob);
        
        Entities
            .WithAll<Mouse>()
            .WithReadOnly(exitTiles)
            .ForEach((Entity entity, int entityInQueryIndex, in Tile mouseTile) =>
            {
                foreach (var exitTile in exitTiles)
                {
                    if (mouseTile.Coords.Equals(exitTile.Coords))
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                        // TODO: Increment player score here
                    }
                }
            }).WithDisposeOnCompletion(exitTiles).ScheduleParallel();
        
        mECBSystem.AddJobHandleForProducer(Dependency);
    }
}
