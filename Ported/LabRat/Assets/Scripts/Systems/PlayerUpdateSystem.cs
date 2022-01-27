using Unity.Collections;
using Unity.Entities;

[UpdateAfter(typeof(ExitSystem))]
public partial class PlayerUpdateSystem : SystemBase
{
    public NativeQueue<Entity> AddPointsToPlayerQueue;
    public NativeQueue<Entity> RemovePointsFromPlayerQueue;

    protected override void OnCreate()
    {
        AddPointsToPlayerQueue = new NativeQueue<Entity>(Allocator.Persistent);
        RemovePointsFromPlayerQueue = new NativeQueue<Entity>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        AddPointsToPlayerQueue.Dispose();
        RemovePointsFromPlayerQueue.Dispose();
    }

    protected override void OnUpdate()
    {
        if (AddPointsToPlayerQueue.IsEmpty() && RemovePointsFromPlayerQueue.IsEmpty())
        {
            return;
        }

        var addPointsToPlayer = AddPointsToPlayerQueue.ToArray(Allocator.TempJob);
        var removePointsFromPlayer = RemovePointsFromPlayerQueue.ToArray(Allocator.TempJob);
        Entities
            .WithAll<Player>()
            .WithReadOnly(addPointsToPlayer)
            .WithReadOnly(removePointsFromPlayer)
            .WithDisposeOnCompletion(addPointsToPlayer)
            .WithDisposeOnCompletion(removePointsFromPlayer)
            .ForEach((Entity entity, ref Score playerScore) =>
            {
                foreach (var playerEntity in addPointsToPlayer)
                {
                    if (playerEntity == entity)
                    {
                        playerScore.Value += 1;
                    }
                }
                foreach (var playerEntity in removePointsFromPlayer)
                {
                    if (playerEntity == entity)
                    {
                        playerScore.Value *= (int)(playerScore.Value * 0.6666f);
                    }
                }
            }).Run();
        AddPointsToPlayerQueue.Clear();
        RemovePointsFromPlayerQueue.Clear();
    }
     
}
