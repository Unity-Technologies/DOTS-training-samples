
using Unity.Collections;
using Unity.Entities;

public class PlatformCollision : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        var query = GetEntityQuery(typeof(Bullet), typeof(BoardTarget));
        RequireForUpdate(query);

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        if (TryGetSingleton<IsPaused>(out _))
            return;

        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        float currentTime = (float)Time.ElapsedTime;

        NativeArray<BoardTarget> bulletTargets = GetEntityQuery(typeof(Bullet), typeof(BoardTarget)).ToComponentDataArray<BoardTarget>(Allocator.TempJob);
        NativeArray<Time> bulletTime = GetEntityQuery(typeof(Bullet), typeof(Time)).ToComponentDataArray<Time>(Allocator.TempJob);

        Entities.
            WithAll<Platform, BoardPosition>()
            .WithStructuralChanges()
            .ForEach((int entityInQueryIndex, Entity entity, in BoardPosition boardPosition) => {

                int count = 0;

                for (int index = 0; index < bulletTargets.Length; index++)
                {
                    if (bulletTime[index].EndTime > currentTime)
                        continue;

                    if (bulletTargets[index].Value.Equals(boardPosition.Value))
                        continue;

                    count++;
                }

                if (count > 0)
                    ecb.AddComponent<WasHit>(entityInQueryIndex, entity, new WasHit { Count = count });

            })
            .WithDisposeOnCompletion(bulletTargets)
            .WithDisposeOnCompletion(bulletTime)
            .Run();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
