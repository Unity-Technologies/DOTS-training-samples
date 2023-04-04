using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

[BurstCompile]
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

        //Get the ECB
        var ECB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        EntityCommandBuffer.ParallelWriter ecbParallel = ECB.AsParallelWriter();

        foreach (var (fireTransform, fireTile) in SystemAPI.Query<LocalTransform, Tile>().WithAll<OnFire>())
        {
            foreach (var (tileTransform, tile, tileEntity) in SystemAPI.Query<LocalTransform, Tile>().WithEntityAccess())
            {
                if ((math.abs(tileTransform.Position.x - fireTransform.Position.x) <= config.cellSize * config.heatRadius &&
                    math.abs(tileTransform.Position.z - fireTransform.Position.z) <= config.cellSize * config.heatRadius) ||
                    (math.abs(tileTransform.Position.x - fireTransform.Position.x) <= config.cellSize * config.heatRadius &&
                     math.abs(tileTransform.Position.z - fireTransform.Position.z) == 0) ||
                    (math.abs(tileTransform.Position.x - fireTransform.Position.x) == 0 &&
                    math.abs(tileTransform.Position.z - fireTransform.Position.z) <= config.cellSize * config.heatRadius))
                {
                    var newTemperature = tile.Temperature + (fireTile.Temperature * config.heatTransferRate);
                    if (newTemperature > config.maxFlameHeight) newTemperature = config.maxFlameHeight;
                    var tileComponent = SystemAPI.GetComponent<Tile>(tileEntity);
                    tileComponent.Temperature = newTemperature;
                    SystemAPI.SetComponent(tileEntity, tileComponent);
                }
            }
        }

        var firePropagationJob = new FirePropagationJob
        {
           
        }.ScheduleParallel(state.Dependency);

        // Create and schedule a Job that tests if the GroundTiles should be on fire
        var ignitionTestJob = new IgnitionTestJob
        {
            config = config,
            ecb = ecbParallel

        }.ScheduleParallel(firePropagationJob);

        // Update Colour and Height of the fire based on the GroundTile temperature
        var onFireTileUpdateJob = new FireUpdateJob
        {
            config = config,
            time = UnityEngine.Time.time,
            ecb = ecbParallel,
        }.ScheduleParallel(ignitionTestJob);

        // Making sure that the last scheduled job in our onUpdate,
        // changes the last Job Dependency of the World state
        state.Dependency = onFireTileUpdateJob;
    }
}

[BurstCompile]
public partial struct IgnitionTestJob : IJobEntity
{
    [ReadOnly] public Config config;
    public EntityCommandBuffer.ParallelWriter ecb;

    void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, Tile tile, ref LocalTransform transform)
    {        
        var isOnFire = tile.Temperature >= config.flashpoint;

        ecb.SetComponentEnabled<OnFire>(chunkIndex, entity, isOnFire);
        if (!isOnFire)
        {
            ecb.SetComponent(chunkIndex, entity, new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)config.colour_fireCell_neutral });

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
    public EntityCommandBuffer.ParallelWriter ecb;

    void Execute([EntityIndexInChunk] int index, [ChunkIndexInQuery] int chunkIndex, Entity entity, Tile tile, ref LocalTransform transform)
    {
        // Handle Position
        float3 pos = transform.Position;
        pos.y = (-config.maxFlameHeight * 0.5f + (tile.Temperature * config.maxFlameHeight)) - config.flickerRange;
        pos.y += (config.flickerRange * 0.5f) + UnityEngine.Mathf.PerlinNoise((time - chunkIndex * 128 + index ) * config.flickerRate - tile.Temperature, tile.Temperature) * config.flickerRange;
        transform.Position = pos;

        // Handle Color
        UnityEngine.Vector4 groundColor(UnityEngine.Color cool, UnityEngine.Color hot)
        { return UnityEngine.Color.Lerp(cool, hot, tile.Temperature); }

        ecb.SetComponent(chunkIndex, entity, new URPMaterialPropertyBaseColor { Value = groundColor(config.colour_fireCell_cool, config.colour_fireCell_hot) });
    }
}


[BurstCompile]
public partial struct FirePropagationJob: IJobEntity
{
    void Execute([ChunkIndexInQuery] int index, Entity entity, Tile tile, LocalTransform transform)
    {
       
    }

}
