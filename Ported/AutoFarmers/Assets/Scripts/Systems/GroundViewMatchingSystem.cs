using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;

[BurstCompile]
public partial struct GroundViewMatchingSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Ground>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var config = SystemAPI.GetSingleton<GameConfig>();

        Entity normalPrefab = config.GroundTileNormalPrefab;
        int normalMaterialId = state.EntityManager.GetComponentData<MaterialMeshInfo>(normalPrefab).Material;

        Entity tilledPrefab = config.GroundTileTilledPrefab;
        int tilledMaterialId = state.EntityManager.GetComponentData<MaterialMeshInfo>(tilledPrefab).Material;

        Entity unpassablePrefab = config.GroundTileUnpassablePrefab;
        int unpassableMaterialId = state.EntityManager.GetComponentData<MaterialMeshInfo>(unpassablePrefab).Material;

        Entity groundEntity = SystemAPI.GetSingletonEntity<Ground>();

        BufferFromEntity<GroundTile> groundDataLookup = state.GetBufferFromEntity<GroundTile>(true);
        if (!groundDataLookup.TryGetBuffer(groundEntity, out DynamicBuffer<GroundTile> bufferData))
        {
            return;
        }

        var updateVisualsJob = new GroundVisualUpdater
        {
            tilledMaterialId = tilledMaterialId,
            unpassableMaterialId = unpassableMaterialId,
            normalMaterialId = normalMaterialId,

            ecb = ecb.AsParallelWriter(),
            GroundData = bufferData
        };

        // Schedule execution in a single thread, and do not block main thread.
        state.Dependency = updateVisualsJob.ScheduleParallel(state.Dependency);
    }
}



[BurstCompile]
partial struct GroundVisualUpdater : IJobEntity
{
    public int tilledMaterialId;
    public int unpassableMaterialId;
    public int normalMaterialId;

    public EntityCommandBuffer.ParallelWriter ecb;

    [ReadOnly]
    public DynamicBuffer<GroundTile> GroundData;

    void Execute(ref GroundTileAspect instance, [ChunkIndexInQuery] int chunkIndex)
    {
        GroundTileState tileState = GroundData[instance.tileView.Index].tileState;

        if (tileState != instance.tileView.ViewState)
        {
            MaterialMeshInfo meshInfo = instance.meshInfo;
            if (GroundUtilities.IsTileTilled(tileState))
            {
                meshInfo.Material = tilledMaterialId;
            }
            else if (!GroundUtilities.IsTilePassable(tileState))
            {
                meshInfo.Material = unpassableMaterialId;
            }
            else
            {
                meshInfo.Material = normalMaterialId;
            }
            ecb.SetComponent(chunkIndex, instance.self, meshInfo);

            GroundTileView tileView = instance.tileView;
            tileView.ViewState = tileState;
            ecb.SetComponent(chunkIndex, instance.self, tileView);
        }
    }
}