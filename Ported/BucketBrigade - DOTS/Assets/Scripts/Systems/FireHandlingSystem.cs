using Systems;
using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(InitializeChainIndecies))]
[UpdateAfter(typeof(GridTilesSpawningSystem))]
public partial struct FireHandlingSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var fireQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, Tile, OnFire>().Build();

        var firePropagationJob = new FirePropagationJob
        {
            fireTiles = fireQuery.ToComponentDataArray<Tile>(Allocator.TempJob),
            fireTransforms = fireQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob),
            config = config
        }.ScheduleParallel(state.Dependency);

        // Create and schedule a Job that tests if the GroundTiles should be on fire
        var ignitionTestJob = new IgnitionTestJob
        {
            config = config
           
        }.ScheduleParallel(firePropagationJob);

        // Update Colour and Height of the fire based on the GroundTile temperature
        var onFireTileUpdateJob = new FireUpdateJob
        {
            config = config,
            time = UnityEngine.Time.time

        }.ScheduleParallel(ignitionTestJob);

        // Making sure that the last scheduled job in our onUpdate,
        // changes the last Job Dependency of the World state
        state.Dependency = onFireTileUpdateJob;
        
    }
}

[BurstCompile]
[WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
public partial struct IgnitionTestJob : IJobEntity
{
    [ReadOnly] public Config config;
    
    void Execute(Tile tile, ref LocalTransform transform, ref URPMaterialPropertyBaseColor color, EnabledRefRW<OnFire> onFireState)
    {        
        var isOnFire = tile.Temperature >= config.flashpoint;
        onFireState.ValueRW = isOnFire;
        if (!isOnFire)
        {
            color = new URPMaterialPropertyBaseColor { Value = (Vector4)config.colour_fireCell_neutral };

            // Resetting ground height after it is not on fire
            float3 pos = transform.Position;
            pos.y = -(config.maxFlameHeight * 0.5f);
            transform.Position = pos;
        }
    }
}


[BurstCompile]
public partial struct FireUpdateJob: IJobEntity
{
    [ReadOnly] public Config config;
    [ReadOnly] public float time;

    void Execute([EntityIndexInChunk] int index, [ChunkIndexInQuery] int chunkIndex, Tile tile, ref LocalTransform transform,
        ref URPMaterialPropertyBaseColor color, EnabledRefRO<OnFire> onFireState)
    {
        if (!onFireState.ValueRO)
            return;

        // Handle Position
        float3 pos = transform.Position;
        pos.y = (-config.maxFlameHeight * 0.5f + (tile.Temperature * config.maxFlameHeight)) - config.flickerRange;
        pos.y += (config.flickerRange * 0.5f) + UnityEngine.Mathf.PerlinNoise((time - chunkIndex * 128 + index ) * config.flickerRate - tile.Temperature, tile.Temperature) * config.flickerRange;
        transform.Position = pos;

        // Handle Color
        UnityEngine.Vector4 groundColor(UnityEngine.Color cool, UnityEngine.Color hot)
        { return UnityEngine.Color.Lerp(cool, hot, tile.Temperature); }

        color = new URPMaterialPropertyBaseColor { Value = groundColor(config.colour_fireCell_cool, config.colour_fireCell_hot) };
    }
}

[BurstCompile]
public partial struct FirePropagationJob: IJobEntity
{
    [ReadOnly] public NativeArray<Tile> fireTiles;
    [ReadOnly] public NativeArray<LocalTransform> fireTransforms;
    [ReadOnly] public Config config;
    void Execute(ref Tile tile, in LocalTransform tileTransform)
    {
        for (int i = 0; i < fireTiles.Length; i++)
        {
            if (math.abs(tileTransform.Position.x - fireTransforms[i].Position.x) <= config.cellSize * config.heatRadius &&
                math.abs(tileTransform.Position.z - fireTransforms[i].Position.z) <= config.cellSize * config.heatRadius)
            {
                var newTemperature = tile.Temperature + (fireTiles[i].Temperature * config.heatTransferRate);
                if (newTemperature > config.maxFlameHeight) newTemperature = config.maxFlameHeight;
                tile.Temperature = newTemperature;
            }
        }
    }
}
