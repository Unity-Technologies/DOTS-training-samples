

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class CreatureColliderSystem : SystemBase
{
    private EntityCommandBufferSystem mECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        mECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    private void CellCollison()
    {
        // Could make this only once, if cats never change
        var query = GetEntityQuery(typeof(Cat), typeof(Tile));

        if (query.IsEmpty)
        {
            return;
        }

        var ecb = mECBSystem.CreateCommandBuffer().AsParallelWriter();

        var catTiles = query.ToComponentDataArray<Tile>(Allocator.TempJob);

        Entities
            .WithAll<Mouse>()
            .WithReadOnly(catTiles)
            .WithDisposeOnCompletion(catTiles)
            .ForEach((Entity entity, int entityInQueryIndex, in Tile mouseTile) =>
            {
                foreach (var catTile in catTiles)
                {
                    // If cat and mouse on the same tile, destroy it - could change to a range based check
                    if (mouseTile.Coords.x == catTile.Coords.x && mouseTile.Coords.y == catTile.Coords.y)
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                    }
                }
            }).ScheduleParallel();

        mECBSystem.AddJobHandleForProducer(Dependency);
    }

    private void DistanceCollision()
    {
        // Could make this only once, if cats never change
        var query = GetEntityQuery(typeof(Cat), typeof(Translation));

        if (query.IsEmpty)
        {
            return;
        }

        var ecb = mECBSystem.CreateCommandBuffer().AsParallelWriter();

        var catTranslations = query.ToComponentDataArray<Translation>(Allocator.TempJob);

        Entities
            .WithAll<Mouse>()
            .WithReadOnly(catTranslations)
            .WithDisposeOnCompletion(catTranslations)
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation) =>
            {
                foreach (var transform in catTranslations)
                {
                    if (math.distance(translation.Value, transform.Value) < 0.8f)
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                    }
                }
            }).ScheduleParallel();

        mECBSystem.AddJobHandleForProducer(Dependency);
    }

    protected override void OnUpdate()
    {
        var config = GetSingleton<Config>();

        switch(config.CollisionMode)
        {
            case CollisionMode.Cell:
                CellCollison();
                break;
            case CollisionMode.Distance:
                DistanceCollision();
                break;
        };
    }
}
