using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(AntMovementSystem))]
//[BurstCompile]
public partial struct PheromoneSpawningSystem : ISystem
{
    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        
        // TODO: can query config for map size?

        var e = state.EntityManager.CreateSingletonBuffer<PheromoneMap>();
        var map = state.EntityManager.GetBuffer<PheromoneMap>(e);
        map.Resize( PheromoneDisplaySystem.PheromoneTextureSizeX * PheromoneDisplaySystem.PheromoneTextureSizeY, NativeArrayOptions.ClearMemory);
    }

    //[BurstCompile]
    public void OnDestroy(ref SystemState state) {}

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        
        // TODO: config
        float pheromoneSpawnAmount = config.PheromoneSpawnRateSec * config.TimeScale * SystemAPI.Time.DeltaTime;
        
        // Pheromone Distance
        int sdx = config.PheromoneSpawnDistPixels;
        int sdy = config.PheromoneSpawnDistPixels;
        
        // Use straight edge to fit sphere influence, diagonal max dist would clip.
        float maxDist = math.max(sdx * sdx, 1);                  
        
        var pheromoneMap = SystemAPI.GetSingletonBuffer<PheromoneMap>();
        foreach( var (transform, ant) in SystemAPI.Query<TransformAspect, Ant>())
        {
            int2 posTex = new int2(PheromoneMapUtil.WorldToPheromoneMap(config.PlaySize, transform.LocalPosition.xz));

            for (int y = -sdy; y < sdy + 1; y++)
            {
                for (int x = -sdx; x < sdx + 1; x++)
                {
                    int d = x * x + y * y;
                    float strength = math.max(1.0f - d / maxDist, 0);
                    PheromoneMapUtil.AddAmount(ref pheromoneMap, posTex.x + x, posTex.y + y, strength * pheromoneSpawnAmount);
                }    
            }
        }
    }
}
