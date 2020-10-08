using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FakeCreateCrops : SystemBase
{
    EntityQuery m_PlainsQuery;
    Random m_Random;
    
    protected override void OnCreate()
    {
        m_PlainsQuery = GetEntityQuery(typeof(Plains), 
                                        ComponentType.Exclude(typeof(Depot)),
                                        ComponentType.Exclude(typeof(Forest)));
        m_Random = new Random(123);
        RequireSingletonForUpdate<FakeCreateCropsTag>();
    }

    protected override void OnUpdate()
    {
        var gameStateEntity = GetSingletonEntity<GameState>();
        EntityManager.RemoveComponent<FakeCreateCropsTag>(gameStateEntity);
        
        var gameState = GetComponent<GameState>(gameStateEntity);
        var prefab = gameState.PlantPrefab;
        int farmersCount = gameState.FarmersCount;
        
        int plainsCount = m_PlainsQuery.CalculateEntityCount();
        NativeArray<Entity> plainsEntities = m_PlainsQuery.ToEntityArray(Allocator.TempJob);
        
        float4 color = new float4(1.0f, 0.75f, 0.0f, 1.0f);
        float s = 1f / 1.5f;
        float3 scale = new float3(s, s, s);
        
        for(int i = 0; i < 2*farmersCount; i++)
        {
            Entity targetPlain = plainsEntities[m_Random.NextInt(plainsCount)];
            Position plainPos = GetComponent<Position>(targetPlain);
            
            var cropEntity = EntityManager.Instantiate(prefab);
            EntityManager.AddComponent<ECSMaterialOverride>(cropEntity);
            EntityManager.SetComponentData(cropEntity, new ECSMaterialOverride { Value = color });
            EntityManager.SetComponentData(cropEntity,
                new Translation { Value = new float3(plainPos.Value.x, 0.0f, plainPos.Value.y) });
            EntityManager.AddComponent<NonUniformScale>(cropEntity);
            EntityManager.SetComponentData(cropEntity, new NonUniformScale { Value = scale });
            EntityManager.AddComponent<Crop>(cropEntity);
        }

        plainsEntities.Dispose();
    }
}
