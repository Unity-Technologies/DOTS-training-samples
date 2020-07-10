using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Scenes;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(SceneSystemGroup))]
public class FireSpawnSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    private EntityQuery m_FireGridSpawnerQuery;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        RequireForUpdate(m_FireGridSpawnerQuery);
    }

    static int ComputeNumberOfMips(int resolution)
    {
        int mip = 1;
        while (resolution != 1)
        {
            resolution >>= 1;
            mip++;
        }
        return mip;
    }

    protected override void OnUpdate()
    {
        var fireGridSetting = GetSingleton<FireGridSettings>();
        var fireGridEntity = GetSingletonEntity<FireGridSettings>();
        Bounds gridBounds = EntityManager.GetComponentData<Bounds>(fireGridEntity);
        float2 gridCorner = gridBounds.BoundsCenter - gridBounds.BoundsExtent * 0.5f;
        var randomCopy = new Random(0x1234567);
        int mipChainCount = ComputeNumberOfMips((int)fireGridSetting.FireGridResolution.x);
        float2 scaleXZ = gridBounds.BoundsExtent / (float2)fireGridSetting.FireGridResolution;

        // Hack to ensure correct ordering of spawned entities
        var gridSpawner = GetSingleton<FireGridSpawner>();
        EntityManager.AddComponent(gridSpawner.FirePrefab, ComponentType.ReadWrite<WorldRenderBounds>());
        EntityManager.AddComponent(gridSpawner.FirePrefab, ComponentType.ChunkComponent<ChunkWorldRenderBounds>());
        EntityManager.AddComponent(gridSpawner.FirePrefab, ComponentType.ChunkComponent<HybridChunkInfo>());

        var ecb = m_CommandBufferSystem.CreateCommandBuffer();
        Entities
            .WithStoreEntityQueryInField(ref m_FireGridSpawnerQuery)
            .ForEach((Entity spawnerEntity, in FireGridSpawner fireGridSpawner) =>
        {
            // Allocate and resize the buffer
            var fireGrid = ecb.AddBuffer<FireCell>(fireGridEntity);
            var fireGridHist = ecb.AddBuffer<FireCellHistory>(fireGridEntity);
            int nativeBufferSize = (int)(fireGridSetting.FireGridResolution.x * fireGridSetting.FireGridResolution.y);
            fireGrid.Capacity = nativeBufferSize;

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
                    LocalToWorld trs;

                    float2 cellPositionWS = gridCorner + scaleXZ * (0.5f + new float2(x, y));
                    var trans = float4x4.Translate(new float3(cellPositionWS.x, 0.0f, cellPositionWS.y));
                    var scale = float4x4.Scale(new float3(scaleXZ.x, 0.01f, scaleXZ.y));
                    trs.Value = math.mul(trans, scale);

                    ecb.SetComponent<LocalToWorld>(instance, trs);
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

            // Allocate the mip chain buffer
            var fireGridFlags = ecb.AddBuffer<FireCellFlag>(fireGridEntity);
            int mipChainLength = (int)(nativeBufferSize * 1.333333334f);
            fireGridFlags.ResizeUninitialized(mipChainLength);
            for (int i = 0; i < mipChainLength; ++i)
            {
                FireCellFlag invalidFlag;
                invalidFlag.OnFire = false;
                fireGridFlags[i] = invalidFlag;
            }

            // Allocate the mip info buffer
            var fireGridMipInfos = ecb.AddBuffer<FireGridMipLevelData>(fireGridEntity);
            fireGridMipInfos.ResizeUninitialized(mipChainCount);
            uint2 currentSize = fireGridSetting.FireGridResolution;
            uint currentOffset = 0;
            for (int i = 0; i < mipChainCount; ++i)
            {
                FireGridMipLevelData mipLevelData;
                mipLevelData.dimensions = currentSize;
                mipLevelData.offset = currentOffset;
                mipLevelData.cellSize = gridBounds.BoundsExtent / currentSize;
                mipLevelData.minCellPosition = gridBounds.BoundsCenter - gridBounds.BoundsExtent * 0.5f + mipLevelData.cellSize * 0.5f;
                currentOffset += (currentSize.x * currentSize.y);
                currentSize = currentSize >> 1;
                fireGridMipInfos[i] = mipLevelData;
            }


            // Spawn the water cells
            WaterVolume nullVolume;
            // The volume for every cell is fixed
            nullVolume.Volume = 50.0f;
            // Compute the scale of the water cells
            float2 waterScale = gridBounds.BoundsExtent.xy / (float)fireGridSpawner.ElementsPerSide;
            // For every side of the grid
            for (int i = 0; i < 4; ++i)
            {
                float2 lineCenter;
                float2 lineDirection;
                if (i == 0)
                {
                    lineCenter = new float2(1.0f, 0.0f);
                    lineDirection = new float2(0.0f, 1.0f);
                }
                else if (i == 1)
                {
                    lineCenter = new float2(-1.0f, 0.0f);
                    lineDirection = new float2(0.0f, 1.0f);
                }
                else if (i == 2)
                {
                    lineCenter = new float2(0.0f, 1.0f);
                    lineDirection = new float2(1.0f, 0.0f);
                }
                else
                {
                    lineCenter = new float2(0.0f, -1.0f);
                    lineDirection = new float2(1.0f, 0.0f);
                }
                
                // The number of elements on each side is fixed
                for (int e = 0; e < fireGridSpawner.ElementsPerSide; ++e)
                {
                    // Compute the position of the element
                    float2 elementPosition = gridBounds.BoundsCenter - (gridBounds.BoundsExtent * (0.5f + 0.05f * randomCopy.NextFloat()) + fireGridSpawner.DistanceToGrid) * lineCenter;

                    // The virtual line center with the displacement
                    float2 lineMin = elementPosition - (gridBounds.BoundsExtent * 0.5f * lineDirection);
                    float lineStep = math.dot((float2)gridBounds.BoundsExtent, lineDirection) / (float)fireGridSpawner.ElementsPerSide;

                    // Add the instance prefab
                    var instance = ecb.Instantiate(fireGridSpawner.WaterPrefab);
                    ecb.AddComponent<WaterVolume>(instance);
    
                    // Compute the translation
                    Translation trs;
                    trs.Value = new float3(lineMin.x + lineStep * lineDirection.x * e, 0.0f, lineMin.y + lineStep * lineDirection.y * e);
                    ecb.SetComponent<Translation>(instance, trs);

                    // Compute the scale
                    NonUniformScale scale;
                    scale.Value = new float3(waterScale.x, 0.001f, waterScale.y) ;
                    ecb.SetComponent<NonUniformScale>(instance, scale);
                }
            }

            // Remove the spawner component
            ecb.RemoveComponent<FireGridSpawner>(spawnerEntity);
        }).Run();
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
