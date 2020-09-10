using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateBefore(typeof(BeeAttackingSystem))]
public class BeeAgonizingSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {

        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var deltaTime = Time.DeltaTime;

        var b = GetSingleton<BattleField>();

        //for resources
        Entities.WithAll<Agony>()
                .WithNone<Bee>()
                .ForEach( ( Entity bee, ref NonUniformScale nuScale) =>
            {
                
                float delta = 5f * deltaTime;
                nuScale.Value -= new float3(delta, delta, delta);
                if(nuScale.Value.x <= 0)
                {
                    ecb.DestroyEntity( bee );
                }
            } ).Run();
        
        //for bees
        Entities.WithAll<Agony>()
            .WithAll<Bee>()
            .ForEach( ( Entity bee, ref NonUniformScale nuScale) =>
            {
                nuScale.Value -= new float3(deltaTime, deltaTime, deltaTime); 
                
                if(nuScale.Value.x <= 0)
                {
                    ecb.DestroyEntity( bee );
                }
            } ).Run();

            //Play the ECB directly after
            ecb.Playback(EntityManager);
            ecb.Dispose();
    }
}
