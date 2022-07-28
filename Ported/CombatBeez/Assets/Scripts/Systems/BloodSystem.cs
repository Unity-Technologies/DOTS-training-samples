using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct BloodSystem : ISystem
{
    //EntityCommandBuffer ecb;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();

        //ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        // Create an EntityCommandBuffer.
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        // Component type FoodResource is implicitly defined by the query so no WithAll is needed.
        // WithAll puts ADDITIONAL requirements on the query.
        foreach (var (entity, blood, transform) in SystemAPI.Query<Entity, RefRW<Blood>, TransformAspect>())
        {
            switch (blood.ValueRW.State)
            {
                case BloodState.FALLING:
                    UpdatePositionOfFallingBlood(ref state, ref config, blood, transform);
                    break;
                case BloodState.POOLING:
                    MarkDryBlood(ref state, ref config, blood);
                    //AddParticleTag(ref state, entity);
                    break;
                case BloodState.DRY:
                    ecb.DestroyEntity(entity);
                    break;
            }
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    public void UpdatePositionOfFallingBlood(ref SystemState state, ref Config config, RefRW<Blood> blood, TransformAspect transform)
    {
        var position = new float3(transform.Position.x, transform.Position.y - config.FallingSpeed, transform.Position.z);
        if (position.y <= 0.0f)
        {
            position.y = 0.0f;
            blood.ValueRW.State = BloodState.POOLING;
        }
        transform.Position = position;
    }

    [BurstCompile]
    public void MarkDryBlood(ref SystemState state, ref Config config, RefRW<Blood> blood)
    {
        blood.ValueRW.TimePooling += state.Time.DeltaTime;
        if (config.TimeBloodTakesToDry <= blood.ValueRW.TimePooling)
        {
            blood.ValueRW.State = BloodState.DRY;
        }
    }

    //[BurstCompile]
    //private void AddParticleTag(ref SystemState state, Entity entity)
    //{
    //    var particleComponentForBlood = new Particle();
    //    particleComponentForBlood.State = ParticleState.NULL;
    //    particleComponentForBlood.Type = ParticleType.FLUID;
    //    ecb.AddComponent(entity, particleComponentForBlood);
    //}
}