using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[UpdateAfter(typeof(AntMovementSystem))]
[BurstCompile]
public partial struct PheromoneSpawningSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        
        // TODO: can query config for map size?

        var e = state.EntityManager.CreateSingletonBuffer<PheromoneMap>();
        var map = state.EntityManager.GetBuffer<PheromoneMap>(e);
        map.Resize( PheromoneDisplaySystem.PheromoneTextureSizeX * PheromoneDisplaySystem.PheromoneTextureSizeY, NativeArrayOptions.ClearMemory);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {}

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        
        // TODO: config
        float spawnAmount = config.PheromoneSpawnRateSec * config.TimeScale * SystemAPI.Time.DeltaTime;
        
        // Pheromone Distance
        int sdx = config.PheromoneSpawnDistPixels;
        int sdy = config.PheromoneSpawnDistPixels;
        
        // Use straight edge to fit sphere influence, diagonal max dist would clip.
        float maxDist = math.max(sdx * sdx, 1);                  
        
        var pheromoneMap = SystemAPI.GetSingletonBuffer<PheromoneMap>();
        if (Input.GetKeyDown(KeyCode.R))
        {
            for (int i = 0; i < pheromoneMap.Length; i++)
                pheromoneMap.ElementAt(i).amount = 0;
        }
        
        // DebugFill with a couple X-lines
        /*for(int i = 0; i < 5; i++)
        for (int x = 0; x < PheromoneDisplaySystem.PheromoneTextureSizeX; x++)
        {
            int y = PheromoneDisplaySystem.PheromoneTextureSizeY / 5 * i;
            PheromoneMapUtil.SetAmount(ref pheromoneMap, x, y, 1);
        }*/

    #if false
        foreach( var (transform, curDirection, prevDirection, ant) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<CurrentDirection>, RefRO<PreviousDirection>, Ant>())
        {
            int2 posTex = new int2(PheromoneMapUtil.WorldToPheromoneMap(config.PlaySize, transform.ValueRO.Position.xz));
            float turnAngle = math.abs(curDirection.ValueRO.Angle - prevDirection.ValueRO.Angle);
            float spawnTurnStrength = 1.0f - math.saturate(turnAngle / (math.PI/2));

            for (int y = -sdy; y < sdy + 1; y++)
            {
                for (int x = -sdx; x < sdx + 1; x++)
                {
                    int d = x * x + y * y;
                    float strength = math.max(1.0f - d / maxDist, 0);
                    PheromoneMapUtil.AddAmount(ref pheromoneMap, posTex.x + x, posTex.y + y, strength * spawnTurnStrength * spawnAmount);
                }    
            }
        }
    #else
        var job = new PheromoneSpawnJob()
        {
            pheromoneMap = pheromoneMap,
            playAreaSize = config.PlaySize, 
            pheromoneSpawnAmount = spawnAmount,
            spawnDistanceMax = maxDist,
            spawnDistanceX = sdx,
            spawnDistanceY = sdy
        };
        job.Schedule();
    #endif

    }
}

[BurstCompile]
[WithAll(typeof(Ant))]
partial struct PheromoneSpawnJob : IJobEntity
{
    public DynamicBuffer<PheromoneMap> pheromoneMap;
    
    public int playAreaSize;
    public float pheromoneSpawnAmount;
    
    public float spawnDistanceMax;
    public int spawnDistanceX;
    public int spawnDistanceY;
    public void Execute(in LocalTransform localTransform, in CurrentDirection curDirection, in PreviousDirection prevDirection)
    {
        //return; // TODO: temp disable
        
        int2 posTex = new int2(PheromoneMapUtil.WorldToPheromoneMap(playAreaSize, localTransform.Position.xz));
        float turnAngle = math.abs(curDirection.Angle - prevDirection.Angle);
        float spawnTurnStrength = 1.0f - math.saturate(turnAngle / (math.PI/2));

        for (int y = -spawnDistanceY; y < spawnDistanceY + 1; y++)
        {
            for (int x = -spawnDistanceX; x < spawnDistanceX + 1; x++)
            {
                int d = x * x + y * y;
                float strength = math.max(1.0f - d / spawnDistanceMax, 0);
                
                // TODO: parallel, NativeList<T>.ParallelWriter??
                PheromoneMapUtil.AddAmount(ref pheromoneMap, posTex.x + x, posTex.y + y, strength * spawnTurnStrength * pheromoneSpawnAmount);
            }    
        }
    }
}
