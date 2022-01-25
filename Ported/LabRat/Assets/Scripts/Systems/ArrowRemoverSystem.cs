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
        var arrowsQuery = GetEntityQuery(typeof(Arrow), typeof(PlayerOwned));
        if (arrowsQuery.IsEmpty)
            return;

        var config = GetSingleton<Config>();
        
        var arrows = arrowsQuery.ToEntityArray(Allocator.TempJob);
        var arrowTimes = arrowsQuery.ToComponentDataArray<Arrow>(Allocator.TempJob);
        var arrowOwners = arrowsQuery.ToComponentDataArray<PlayerOwned>(Allocator.TempJob);
        
        var ecb = mECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithAll<Player>()
            .WithReadOnly(arrows)
            .WithReadOnly(arrowTimes)
            .WithReadOnly(arrowOwners)
            .WithDisposeOnCompletion(arrows)
            .WithDisposeOnCompletion(arrowTimes)
            .WithDisposeOnCompletion(arrowOwners)
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
                }
            }).ScheduleParallel();

        mECBSystem.AddJobHandleForProducer(Dependency);
    }
}