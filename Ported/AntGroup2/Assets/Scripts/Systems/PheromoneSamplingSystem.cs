using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[UpdateBefore(typeof(PheromoneSpawningSystem))]
//[BurstCompile]
public partial struct PheromoneSamplingSystem : ISystem
{
    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }
    //[BurstCompile]
    public void OnDestroy(ref SystemState state) {}
    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var pheromoneMap = SystemAPI.GetSingletonBuffer<PheromoneMap>();

        // Search distance
        int sdx = config.PheromoneSampleDistPixels;
        int sdy = config.PheromoneSampleDistPixels;
        
        foreach (var (transform, currentDirection, pheromoneDirection, ant) in SystemAPI.Query<TransformAspect, RefRO<CurrentDirection>, RefRW<PheromoneDirection>, Ant>())
        {
            int2 posTex = new int2(PheromoneMapUtil.WorldToPheromoneMap(config.PlaySize, transform.LocalPosition.xz));

            int dirX = 0;
            int dirY = 0;
            float dirAmount = 0.0f;
            for (int y = -sdy; y < sdy + 1; y++)
            {
                for (int x = -sdx; x < sdx + 1; x++)
                {
                    float amount = PheromoneMapUtil.GetAmount(ref pheromoneMap, posTex.x + x, posTex.y + y);
                    if (amount > dirAmount)
                    {
                        dirAmount = amount;
                        dirX = x;
                        dirY = y;
                    }
                }    
            }

            if (dirX != 0 && dirY != 0)
            {
                var pDir = math.normalize(new float2(dirX, dirY));
                var pAngle = math.dot(pDir, new float2(1, 0));
                pheromoneDirection.ValueRW.Angle = pAngle - currentDirection.ValueRO.Angle;  // It this correct??
                pheromoneDirection.ValueRW.Strength = dirAmount;
                
                //Debug.Log( $"Sample: {dirX},{dirY}, pheromoneDir {pheromoneDir}, pAngle {angle}, Result:{pheromoneDirection.ValueRW.Angle}, Current:{currentDirection.ValueRO.Angle}");
            }
            else
            {
                pheromoneDirection.ValueRW.Angle = 0;
                
                //Debug.Log( $"Sample: {dirX},{dirY}");
            }
        }
    }
}