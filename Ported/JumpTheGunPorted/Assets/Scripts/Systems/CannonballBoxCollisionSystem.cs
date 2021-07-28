using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CannonballBoxCollisionSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        var query = GetEntityQuery(typeof(Cannonball));
        RequireForUpdate(query);
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var parallelWriter = ecb.AsParallelWriter();

        Entities
            .WithAll<Cannonball>()
            .ForEach((Entity entity, int entityInQueryIndex, in ParabolaTValue tValue) =>
            {
                if (tValue.Value >= 0.97f) // TODO: fix movement system so it actually gets to 1.0
                {
                    parallelWriter.DestroyEntity(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}