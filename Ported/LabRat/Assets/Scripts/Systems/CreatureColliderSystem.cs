

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class CreatureColliderSystem : SystemBase
{
    private EntityCommandBufferSystem mECBSystem;
    private EntityQuery catsQuery;

    protected override void OnCreate()
    {
        mECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        catsQuery = GetEntityQuery(
            ComponentType.ReadOnly<Cat>(),
            ComponentType.ReadOnly<Tile>(),
            ComponentType.ReadOnly<Translation>());
        RequireForUpdate(catsQuery);
        RequireSingletonForUpdate<GameRunning>();
    }

    private void CellCollison()
    {
        var ecb = mECBSystem.CreateCommandBuffer().AsParallelWriter();

        var catTiles = catsQuery.ToComponentDataArray<Tile>(Allocator.TempJob);

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
        var ecb = mECBSystem.CreateCommandBuffer().AsParallelWriter();

        var catTranslations = catsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

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

    private void DistanceCollisionAsync()
    {
        var query = GetEntityQuery(ComponentType.ReadOnly<Cat>(), ComponentType.ReadOnly<Translation>());

        if (query.IsEmpty)
        {
            return;
        }

        var ecb = mECBSystem.CreateCommandBuffer().AsParallelWriter();

        var catTranslations = query.ToComponentDataArrayAsync<Translation>(Allocator.TempJob, out var catTransCopyJob);

        Dependency = Entities
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
            }).ScheduleParallel(catTransCopyJob);

        mECBSystem.AddJobHandleForProducer(Dependency);
    }

    private void DistanceCollisionAnimationAsync()
    {
        var query = GetEntityQuery(ComponentType.ReadOnly<Cat>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadWrite<Scale>());

        if (query.IsEmpty)
        {
            return;
        }

        var ecb = mECBSystem.CreateCommandBuffer().AsParallelWriter();

        var catTranslations = query.ToComponentDataArrayAsync<Translation>(Allocator.TempJob, out var catTransCopyJob);
        var catScalings = query.ToComponentDataArrayAsync<Scale>(Allocator.TempJob, out var catScaleCopyJob);

        Dependency = Entities
            .WithAll<Mouse>()
            .WithReadOnly(catTranslations)
            // Need to disable checks for catScalings or checking will complain, even with the async copy as a dependency for the ForEach
            .WithNativeDisableContainerSafetyRestriction(catScalings)
            .WithDisposeOnCompletion(catTranslations)
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation) =>
            {
                for (int i = 0; i < catTranslations.Length; ++i)
                {
                    var transform = catTranslations[i];

                    if (math.distance(translation.Value, transform.Value) < 0.8f)
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);

                        // Make cat a little big when eating a mouse
                        var scale = catScalings[i];
                        scale.Value = 1.4f;
                        catScalings[i] = scale;
                    }
                }
            }).ScheduleParallel(JobHandle.CombineDependencies(catTransCopyJob, catScaleCopyJob));

        query.AddDependency(Dependency);
        query.CopyFromComponentDataArrayAsync(catScalings, out var catScalingCopyBackJob);

        Dependency = JobHandle.CombineDependencies(Dependency, catScalingCopyBackJob);

        var catScalingDisposeJob = catScalings.Dispose(Dependency);

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
            case CollisionMode.DistanceAsync:
                DistanceCollisionAsync();
                break;
            case CollisionMode.DistanceAnimationAsync:
                DistanceCollisionAnimationAsync();
                break;
            default:
                Debug.Log("No Collision implemented for enum " + config.CollisionMode);
                break;
        };
    }
}
