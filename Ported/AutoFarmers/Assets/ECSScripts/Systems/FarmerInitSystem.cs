
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateAfter(typeof(GameGenSystem))]
public class FarmerInitSystem : SystemBase
{
    Random m_Random;
    EntityQuery m_AvailableDepotsQuery;
    
    protected override void OnCreate()
    {
        m_Random = new Random(12345789);
        RequireSingletonForUpdate<FarmerSpawn>();
        m_AvailableDepotsQuery = GetEntityQuery(typeof(Depot));
    }

    protected override void OnUpdate()
    {
        var gameStateEntity = GetSingletonEntity<GameState>();
        var gameState = GetSingleton<GameState>();
        // Remove the game spawn component so that this system doesn't run again next frame
        EntityManager.RemoveComponent<FarmerSpawn>(gameStateEntity);

        int depotCount = m_AvailableDepotsQuery.CalculateEntityCount();
        NativeArray<Entity> depotsEntities = m_AvailableDepotsQuery.ToEntityArray(Allocator.TempJob);

        for(int i = 0; i < gameState.FarmersCount; i++)
        {
            int spawnDepotIndex = m_Random.NextInt(depotCount);
            EntityManager.AddComponent<DepotCanSpawn>(depotsEntities[spawnDepotIndex]);
        }

        depotsEntities.Dispose(Dependency);
    }
}
