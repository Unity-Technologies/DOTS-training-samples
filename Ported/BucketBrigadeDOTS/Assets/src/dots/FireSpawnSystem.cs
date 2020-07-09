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

            // Remove the spawner component
            ecb.RemoveComponent<FireGridSpawner>(spawnerEntity);
        }).Schedule();
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
