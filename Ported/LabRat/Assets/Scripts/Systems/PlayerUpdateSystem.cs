using Unity.Collections;
using Unity.Entities;

[UpdateAfter(typeof(MiceExitSystem))]
public partial class PlayerUpdateSystem : SystemBase
{
    public NativeQueue<Entity> AddPointsToPlayerQueue;

    protected override void OnCreate()
    {
        AddPointsToPlayerQueue = new NativeQueue<Entity>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        AddPointsToPlayerQueue.Dispose();
    }

    protected override void OnUpdate()
    {
        var addPointsToPlayer = AddPointsToPlayerQueue.ToArray(Allocator.TempJob);
        Entities
            .WithAll<Player>()
            .WithReadOnly(addPointsToPlayer)
            .WithDisposeOnCompletion(addPointsToPlayer)
            .ForEach((Entity entity, ref Player player) =>
            {
                foreach (var playerEntity in addPointsToPlayer)
                {
                    if (playerEntity == entity)
                    {
                        player.Score += 1;
                    }
                }
            }).Schedule();
        AddPointsToPlayerQueue.Clear();
    }
     
}
