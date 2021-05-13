using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityInput = UnityEngine.Input;

[UpdateInGroup(typeof(Unity.Entities.SimulationSystemGroup))]
[UpdateAfter(typeof(BulletMovementSystem))]
[UpdateAfter(typeof(PlatformCollision))]
public class BulletDespawnerSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        var query = GetEntityQuery(typeof(Tank), typeof(BoardPosition));
        RequireForUpdate(query);

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        if (TryGetSingleton<IsPaused>(out _))
            return;

        var currentTime = Time.ElapsedTime;
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithAll<Bullet>()
            .ForEach((
                Entity bulletEntity,
                int entityInQueryIndex,
                in Time time) =>
            {
                if (time.EndTime < currentTime)
                    ecb.DestroyEntity(entityInQueryIndex, bulletEntity);
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}