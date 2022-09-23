using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
struct SpawnCommon
{
    public static void Spawn(int index, ref EntityCommandBuffer.ParallelWriter ECB, AABB Aabb, Entity Prefab, 
        EntityQueryMask Mask, int InitFaction, LocalToWorldTransform InitTransform, float4 InitColor, out Entity entity, 
        float3 InitVel)
    {
        entity = ECB.Instantiate(index, Prefab);
        var random = Random.CreateFromIndex((uint)index);

        var randomf3 = (random.NextFloat3() - new float3(0.5f, 0.5f, 0.5f)) * 2.0f; // [-1;1]

        float3 position = Aabb.Center + Aabb.Extents * randomf3;

        ECB.SetComponent(index, entity, new LocalToWorldTransform{Value = UniformScaleTransform.FromPositionRotationScale(position, InitTransform.Value.Rotation, InitTransform.Value.Scale)});
        ECB.SetSharedComponent(index, entity, new Faction{Value = InitFaction});
        ECB.AddComponentForLinkedEntityGroup(index, entity, Mask, new URPMaterialPropertyBaseColor { Value = InitColor});
		ECB.SetComponent(index, entity, new Velocity{Value = InitVel});
    }
}

[BurstCompile]
struct FoodSpawnJob : IJobParallelFor
{
    // A regular EntityCommandBuffer cannot be used in parallel, a ParallelWriter has to be explicitly used.
    public EntityCommandBuffer.ParallelWriter ECB;
    public Entity Prefab;
    public AABB Aabb;
    public LocalToWorldTransform InitTransform;
    public EntityQueryMask Mask;
    public int InitFaction;
    public float4 InitColor;
    public float3 InitVel;

    public void Execute(int index)
    {
        SpawnCommon.Spawn(index, ref ECB, Aabb, Prefab, Mask, InitFaction, InitTransform, InitColor, out _, InitVel);
    }
}

[BurstCompile]
struct BeeSpawnJob : IJobParallelFor
{
    // A regular EntityCommandBuffer cannot be used in parallel, a ParallelWriter has to be explicitly used.
    public EntityCommandBuffer.ParallelWriter ECB;
    public Entity Prefab;
    public AABB Aabb;
    public LocalToWorldTransform InitTransform;
    public EntityQueryMask Mask;
    public int InitFaction;
    public float4 InitColor;
    public float3 InitVel;
    public float Aggressivity;

    public void Execute(int index)
    {
        SpawnCommon.Spawn(index, ref ECB, Aabb, Prefab, Mask, InitFaction, InitTransform, InitColor, out var entity, InitVel);
        
        ECB.SetComponentEnabled<Dead>(index, entity, false);
        ECB.SetComponent<BeeProperties>(index, entity, new BeeProperties
        {
            Aggressivity = Aggressivity,
            BeeMode = BeeMode.Idle,
            Target = Entity.Null,
            TargetPosition = float3.zero,
        });
    }
}