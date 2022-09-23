using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[WithAll(typeof(Falling))]
partial struct ApplyGravityJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;

    public float Floor;
    public float Gravity;
    public float DeltaTime;

    void Execute(Entity e, [EntityInQueryIndex] int idx, ref TransformAspect t, ref Velocity v)
    {
        if ((t.LocalToWorld.Position.y - t.LocalToWorld.Scale / 2) - Floor > float.Epsilon)
        {
            v.Value.y += Gravity * DeltaTime;
        }
        else
        {
            var localToWorld = t.LocalToWorld; 
            
            v.Value.y = 0f;
            localToWorld.Position.y = Floor + localToWorld.Scale / 2;
            t.LocalToWorld = localToWorld;

            ECB.SetComponentEnabled<Falling>(idx, e, false);
            ECB.AddComponent<Decay>(idx, e);

            // apply ground friction for bonus points?
        }
    }
}

[BurstCompile]
[UpdateAfter(typeof(AttackingSystem))]
[UpdateAfter(typeof(GatheringSystem))]
[UpdateBefore(typeof(BeeClampingSystem))]
partial struct BallisticMovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FieldConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var fieldConfig = SystemAPI.GetSingleton<FieldConfig>();

        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        new ApplyGravityJob
            {
                ECB = ecb, Floor = -fieldConfig.FieldScale.y * .5f, Gravity = fieldConfig.FieldGravity,
                DeltaTime = state.Time.DeltaTime
            }
            .ScheduleParallel();
    }
}