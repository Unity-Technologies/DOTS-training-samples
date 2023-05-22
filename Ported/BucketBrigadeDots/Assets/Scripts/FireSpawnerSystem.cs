using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct FireSpawnerSystem : ISystem
{
    private uint m_UpdateCounter;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FireSpawner>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var fireCellsQuery = SystemAPI.QueryBuilder().WithAll<FireCell>().Build();
        if (fireCellsQuery.IsEmpty)
        {
            var prefab = SystemAPI.GetSingleton<FireSpawner>().Prefab;

            // Instantiating an entity creates copy entities with the same component types and values.
            var instances = state.EntityManager.Instantiate(prefab, 500, Allocator.Temp);

            // Unlike new Random(), CreateFromIndex() hashes the random seed
            // so that similar seeds don't produce similar results.
            var random = Random.CreateFromIndex(m_UpdateCounter++);

            foreach (var entity in instances)
            {
                // Update the entity's LocalTransform component with the new position.
                var transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                transform.ValueRW.Position = (random.NextFloat3() - new float3(0.5f, 0, 0.5f)) * 20;
            }
        }
    }
}