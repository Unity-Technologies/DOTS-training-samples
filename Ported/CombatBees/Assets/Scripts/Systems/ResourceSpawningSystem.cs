using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct ResourceSpawningSystem : ISystem
{
    Vector2Int m_GridCounts;
    Vector2 m_GridSize;
    Vector2 m_MinGridPos;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FieldConfig>();
        state.RequireForUpdate<ResourceConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var resourceConfig = SystemAPI.GetSingleton<ResourceConfig>();
        var fieldConfig = SystemAPI.GetSingleton<FieldConfig>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        m_GridCounts = Vector2Int.RoundToInt(new Vector2(fieldConfig.FieldScale.x, fieldConfig.FieldScale.z) / resourceConfig.resourceSize);
        m_GridSize = new Vector2(fieldConfig.FieldScale.x / m_GridCounts.x, fieldConfig.FieldScale.z / m_GridCounts.y);
        m_MinGridPos = new Vector2((m_GridCounts.x - 1f) * -.5f * m_GridSize.x, (m_GridCounts.y - 1f) * -.5f * m_GridSize.y);

        var resources = CollectionHelper.CreateNativeArray<Entity>(resourceConfig.startResourceCount, Allocator.Temp);

        ecb.Instantiate(resourceConfig.resourcePrefab, resources);

        foreach (var resource in resources)
        {
            float3 pos = new float3(m_MinGridPos.x * .25f + UnityEngine.Random.value * fieldConfig.FieldScale.x * .25f, UnityEngine.Random.value * 10f, m_MinGridPos.y + UnityEngine.Random.value * fieldConfig.FieldScale.z);

            ecb.AddComponent(resource, new GridPosition());

            var scaleTransform = UniformScaleTransform.FromPosition(pos);
            scaleTransform.Scale = resourceConfig.resourceSize;

            ecb.SetComponent(resource, new LocalToWorldTransform() { Value = scaleTransform });
        }

        state.Enabled = false;
    }
}
