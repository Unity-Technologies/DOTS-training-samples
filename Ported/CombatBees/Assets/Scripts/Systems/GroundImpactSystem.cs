using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[WithAll(typeof(Decay))]
[WithNone(typeof(BlueTeam), typeof(YellowTeam))]
partial struct ResourceImpactJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;

    public float3 FieldExtents;

    public int numBeesToSpawn;
    public Entity BeePrefab;

    void Execute(Entity resource, [EntityInQueryIndex] int idx, in TransformAspect prs)
    {
        if (math.abs(prs.LocalToWorld.Position.x) >= FieldExtents.x)
        {
            var newBees = CollectionHelper.CreateNativeArray<Entity>(numBeesToSpawn, Allocator.Temp);
            var pos = prs.LocalToWorld.Position;
            BeeSpawnHelper.SpawnBees(BeePrefab, ref ECB, idx, ref newBees,
                prs.LocalToWorld.Position.x > 0 ? BeeTeam.Yellow : BeeTeam.Blue, in pos);
        }
        else
        {
            ECB.RemoveComponent<Decay>(idx, resource);
        }
    }
}

[BurstCompile]
[WithAll(typeof(Decay))]
[WithAny(typeof(YellowTeam), typeof(BlueTeam))]
partial struct BeeImpactJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;

    public float3 FieldExtents;

    void Execute(Entity bee)
    {
    }
}

// [BurstCompile]
[UpdateAfter(typeof(BallisticMovementSystem))]
partial struct GroundImpactSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ResourceConfig>();
        state.RequireForUpdate<FieldConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var resourceConfig = SystemAPI.GetSingleton<ResourceConfig>();
        var fieldConfig = SystemAPI.GetSingleton<FieldConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        new BeeImpactJob { ECB = ecb.AsParallelWriter(), FieldExtents = fieldConfig.FieldScale }.ScheduleParallel();
        new ResourceImpactJob
            {
                ECB = ecb.AsParallelWriter(), FieldExtents = fieldConfig.FieldScale * .8f / 2,
                numBeesToSpawn = resourceConfig.beesPerResource,
                BeePrefab = InitialBeeSpawningSystem.BeePrefab // accessing this prevents burst
            }
            .ScheduleParallel();
    }
}