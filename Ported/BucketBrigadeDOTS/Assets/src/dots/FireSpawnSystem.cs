using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class FireSpawnSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    private EntityQuery m_FireGridSpawnerQuery;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        RequireForUpdate(m_FireGridSpawnerQuery);
    }

    protected override void OnUpdate()
    {
        var fireGridSetting = GetSingleton<FireGridSettings>();
        var fireGridEntity = GetSingletonEntity<FireGridSettings>();
        Bounds gridBounds = EntityManager.GetComponentData<Bounds>(fireGridEntity);
        float2 gridCorner = gridBounds.BoundsCenter - gridBounds.BoundsExtent * 0.5f;
        var randomCopy = new Random(0x1234567);

        float2 scaleXZ = gridBounds.BoundsExtent / (float2)fireGridSetting.FireGridResolution;

        var ecb = m_CommandBufferSystem.CreateCommandBuffer();
        Entities
            .WithStoreEntityQueryInField(ref m_FireGridSpawnerQuery)
            .ForEach((Entity spawnerEntity, in FireGridSpawner fireGridSpawner) =>
        {
            // Allocate and resize the buffer
            var fireGrid = ecb.AddBuffer<FireCell>(fireGridEntity);
            var fireGridHist = ecb.AddBuffer<FireCellHistory>(fireGridEntity);
            fireGrid.Capacity = (int)(fireGridSetting.FireGridResolution.x * fireGridSetting.FireGridResolution.y);

            // Initialize the buffer
            FireCell nullCell;
            FireCellHistory nullCellHist;
            nullCell.FireTemperature = nullCellHist.FireTemperaturePrev = 0.0f;
            for (int y = 0; y < fireGridSetting.FireGridResolution.y; ++y)
            {
                for (int x = 0; x < fireGridSetting.FireGridResolution.x; ++x)
                {
                    // Nullify the value first
                    fireGrid.Add(nullCell);
                    fireGridHist.Add(nullCellHist);

                    // Add the instance prefab
                    var instance = ecb.Instantiate(fireGridSpawner.FirePrefab);

                    // Set the translation
                    Translation cellTranslation;
                    float2 cellPositionWS = gridCorner + scaleXZ * (0.5f + new float2(x, y));
                    cellTranslation.Value = new float3(cellPositionWS.x, 0.0f, cellPositionWS.y);
                    ecb.SetComponent<Translation>(instance, cellTranslation);

                    // Set a non-uniform scale (non guaranteed to be pre-existing)
                    NonUniformScale cellScale;
                    cellScale.Value = new float3(scaleXZ.x, 0.01f, scaleXZ.y);
                    ecb.AddComponent<NonUniformScale>(instance, cellScale);
  
                }
            }

            // Add the initial fire points
            FireCellHistory targetCell;
            uint gridSize = fireGridSetting.FireGridResolution.x * fireGridSetting.FireGridResolution.y;
            for (int n = 0; n < fireGridSpawner.StartingFireCount; ++n)
            {
                uint targetIndex = (randomCopy.NextUInt()) % gridSize;
                targetCell.FireTemperaturePrev = 0.5f + (randomCopy.NextFloat() * 0.5f);
                fireGridHist[(int)targetIndex] = targetCell;
            }

            // Remove the spawner component
            ecb.RemoveComponent<FireGridSpawner>(spawnerEntity);
        }).Schedule();
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
