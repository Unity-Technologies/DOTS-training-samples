using Unity.Collections;
using Unity.Entities;

[UpdateAfter(typeof(ArrowPlacerSystem))]
public partial class ArrowRemoverSystem : SystemBase
{

    private EntityCommandBufferSystem mECBSystem;

    protected override void OnCreate()
    {
        mECBSystem = World.GetExistingSystem<EntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var arrowsQuery = GetEntityQuery(typeof(Arrow), typeof(PlayerOwned), typeof(Tile));
        if (arrowsQuery.IsEmpty)
            return;

        var config = GetSingleton<Config>();
        
        var arrows = arrowsQuery.ToEntityArray(Allocator.TempJob);
        var arrowTimes = arrowsQuery.ToComponentDataArray<Arrow>(Allocator.TempJob);
        var arrowOwners = arrowsQuery.ToComponentDataArray<PlayerOwned>(Allocator.TempJob);
        var arrowTiles = arrowsQuery.ToComponentDataArray<Tile>(Allocator.TempJob);
        
        var ecb = mECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithAll<Player>()
            .WithReadOnly(arrows)
            .WithReadOnly(arrowTimes)
            .WithReadOnly(arrowOwners)
            .WithReadOnly(arrowTiles)
            .WithDisposeOnCompletion(arrows)
            .WithDisposeOnCompletion(arrowTimes)
            .WithDisposeOnCompletion(arrowOwners)
            .WithDisposeOnCompletion(arrowTiles)
            .ForEach((Entity player, int nativeThreadIndex) =>
            {
                int playerArrows = 0;
                Entity oldestArrowForThisPlayerSoFar = Entity.Null;
                float oldestArrowTime = float.PositiveInfinity;
                for (int i = 0; i < arrows.Length; ++i)
                {
                    if (arrowOwners[i].Owner == player)
                    {
                        playerArrows++;
                        if (oldestArrowTime > arrowTimes[i].PlacedTime)
                        {
                            oldestArrowForThisPlayerSoFar = arrows[i];
                            oldestArrowTime = arrowTimes[i].PlacedTime;
                        }
                        if (playerArrows > config.MaxArrowsPerPlayer)
                        {
                            // just pick the first match and destroy it - no need to clear all of the arrows
                            // we're supposed to see just one single arrow generated per frame per player at most
                            ecb.DestroyEntity(nativeThreadIndex, oldestArrowForThisPlayerSoFar);
                            return;
                        }
                    }

                    // loop through the rest of the arrows to see if somebody placed an arrow on top of this one
                    for (int j = i+1; j < arrows.Length; j++)
                    {
                        // if coordinates are equal, these two arrows were spawned in the same place
                        // we need to destroy the older of them both
                        if (arrowTiles[i].Coords.Equals(arrowTiles[j].Coords))
                        {
                            // if arrow j is older, destroy j
                            if (arrowTimes[i].PlacedTime > arrowTimes[j].PlacedTime)
                            {
                                ecb.DestroyEntity(nativeThreadIndex, arrows[j]);
                            }
                            // else destroy i
                            else
                            {
                                ecb.DestroyEntity(nativeThreadIndex, arrows[i]);
                            }
                            return;
                        }
                    }
                }
            }).ScheduleParallel();

        mECBSystem.AddJobHandleForProducer(Dependency);
    }
}