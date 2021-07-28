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

        Entities
            .WithoutBurst()
            .WithAll<Cannonball>()
            .ForEach((Entity entity, in ParabolaTValue tValue) =>
            {
                UnityEngine.Debug.Log(tValue.Value);
                if (tValue.Value >= 0.97f)
                {
                    ecb.DestroyEntity(entity);
                }
            }).Schedule();
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}