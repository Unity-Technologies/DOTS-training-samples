
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(Unity.Entities.SimulationSystemGroup))]
public class PlatformCollision : SystemBase
{
    private EntityCommandBufferSystem ecbs;

    protected override void OnCreate()
    {
        var query = GetEntityQuery(typeof(Bullet), typeof(BoardTarget));
        RequireForUpdate(query);

        ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        if (TryGetSingleton<IsPaused>(out _))
            return;

        var ecb = ecbs.CreateCommandBuffer().AsParallelWriter();

        float currentTime = (float)Time.ElapsedTime;

        var query = GetEntityQuery(typeof(Bullet), typeof(BoardTarget), typeof(Time));
        NativeArray<BoardTarget> bulletTargets = query.ToComponentDataArray<BoardTarget>(Allocator.TempJob);
        NativeArray<Time> bulletTime = query.ToComponentDataArray<Time>(Allocator.TempJob);

        Entities
            .WithDisposeOnCompletion(bulletTargets)
            .WithDisposeOnCompletion(bulletTime)
            .WithAll<Platform, BoardPosition>()
            .WithReadOnly(bulletTargets)
            .WithReadOnly(bulletTime)
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
            .ScheduleParallel();

        ecbs.AddJobHandleForProducer(Dependency);
    }
}
