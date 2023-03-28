using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;

[BurstCompile]
[UpdateAfter(typeof(GridTilesSpawningSystem))]
public partial struct FireHandlingSystem : ISystem
{
    public ComponentLookup<OnFire> m_OnFireActive;

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

    void Execute([EntityIndexInQuery]int index, Entity entity, Tile tile, ref LocalTransform transform)
    {        
        var isOnFire = tile.Temperature >= config.flashpoint;

        ecb.SetComponentEnabled<OnFire>(index, entity, isOnFire);

        if (!isOnFire)
        {
            ecb.SetComponent(index, entity, new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)config.colour_fireCell_neutral });

            // Resetting ground height after it is not on fire
            float3 pos = transform.Position;
            pos.y = -(config.maxFlameHeight * 0.5f);
            transform.Position = pos;
        }
    }
}


[BurstCompile]
[WithAll(typeof(OnFire))]
public partial struct FireUpdateJob: IJobEntity
{
    [ReadOnly] public Config config;
    [ReadOnly] public float time;
    public EntityCommandBuffer.ParallelWriter ecb;

    void Execute([EntityIndexInQuery] int index, Entity entity, Tile tile, ref LocalTransform transform)
    {
        // Handle Position
        float3 pos = transform.Position;
        pos.y = (-config.maxFlameHeight * 0.5f + (tile.Temperature * config.maxFlameHeight)) - config.flickerRange;
        pos.y += (config.flickerRange * 0.5f) + UnityEngine.Mathf.PerlinNoise((time - index) * config.flickerRate - tile.Temperature, tile.Temperature) * config.flickerRange;
        transform.Position = pos;

        // Handle Color
        UnityEngine.Vector4 groundColor(UnityEngine.Color cool, UnityEngine.Color hot)
        { return UnityEngine.Color.Lerp(cool, hot, tile.Temperature); }

        ecb.SetComponent(index, entity, new URPMaterialPropertyBaseColor { Value = groundColor(config.colour_fireCell_cool, config.colour_fireCell_hot) });
    }
}

[BurstCompile]
public partial struct FirePropagationJob: IJobEntity
{
    void Execute([EntityIndexInQuery] int index, Entity entity)
    {

    }

}
