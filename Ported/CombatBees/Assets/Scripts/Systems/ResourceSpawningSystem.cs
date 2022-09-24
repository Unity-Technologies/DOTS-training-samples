using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

//[BurstCompile]
partial struct ResourceSpawningSystem : ISystem
{
    Vector2Int m_GridCounts;
    Vector2 m_GridSize;
    Vector2 m_MinGridPos;
    float m_ResourceSize;
    float m_FieldHeight;
    
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
        m_FieldHeight = fieldConfig.FieldScale.y;
        m_ResourceSize = resourceConfig.resourceSize;

        var resources = CollectionHelper.CreateNativeArray<Entity>(resourceConfig.startResourceCount, Allocator.Temp);

        ecb.Instantiate(resourceConfig.resourcePrefab, resources);

        int[,] stackHeights = new int[m_GridCounts.x, m_GridCounts.y];

        foreach (var resource in resources)
        {
            float3 pos = new float3(m_MinGridPos.x * .25f + UnityEngine.Random.value * fieldConfig.FieldScale.x * .25f, UnityEngine.Random.value * 10f, m_MinGridPos.y + UnityEngine.Random.value * fieldConfig.FieldScale.z);
            pos = NearestSnappedPos(pos);

            int gridX, gridY;
            GetGridIndex(pos, out gridX, out gridY);

            var gridPos = new GridPosition() { Value = new int2 (gridX, gridY) };

            ecb.AddComponent(resource, gridPos);

            var stackIndex = new StackIndex();
            stackIndex.Value = stackHeights[gridX, gridY];

            ecb.AddComponent(resource, stackIndex);

            if ((stackIndex.Value + 1) * m_ResourceSize < fieldConfig.FieldScale.y)
            {
                stackHeights[gridX, gridY]++;
            }

            float floorY = GetStackPos(gridX, gridY, stackIndex.Value).y;
            pos.y = floorY;

            var scaleTransform = UniformScaleTransform.FromPositionRotationScale(pos, quaternion.identity, m_ResourceSize);

            ecb.SetComponent(resource, new LocalToWorldTransform() { Value = scaleTransform });
            
            ecb.AddComponent(resource, new PostTransformMatrix());
        }

        state.Enabled = false;
    }

    float3 NearestSnappedPos(float3 pos)
    {
        int x, y;
        GetGridIndex(pos, out x, out y);
        return new float3(m_MinGridPos.x + x * m_GridSize.x, pos.y, m_MinGridPos.y + y * m_GridSize.y);
    }

    void GetGridIndex(float3 pos, out int gridX, out int gridY)
    {
        gridX = Mathf.FloorToInt((pos.x - m_MinGridPos.x + m_GridSize.x * .5f) / m_GridSize.x);
        gridY = Mathf.FloorToInt((pos.z - m_MinGridPos.y + m_GridSize.y * .5f) / m_GridSize.y);

        gridX = Mathf.Clamp(gridX, 0, m_GridCounts.x - 1);
        gridY = Mathf.Clamp(gridY, 0, m_GridCounts.y - 1);
    }

    Vector3 GetStackPos(int x, int y, int height)
    {
        return new Vector3(m_MinGridPos.x + x * m_GridSize.x, -m_FieldHeight * .5f + (height + .5f) * m_ResourceSize, m_MinGridPos.y + y * m_GridSize.y);
    }
}
