using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[UpdateBefore(typeof(AntMovementSystem))]
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

        float sampleDist = config.PheromoneSampleDistPixels * config.TimeScale * SystemAPI.Time.DeltaTime;
        float steerAngleRad = config.PheromoneSampleStepAngle;
        int stepCount = config.PheromoneSampleStepCount;
        
        foreach (var (transform, currentDirection, pheromoneDirection, ant) in SystemAPI.Query<TransformAspect, RefRO<CurrentDirection>, RefRW<PheromoneDirection>, Ant>())
        {
            float2 mapPos = PheromoneMapUtil.WorldToPheromoneMap(config.PlaySize, transform.LocalPosition.xz);
            float curAngle = currentDirection.ValueRO.Angle;

            float angle = 0;
            for (int s = -stepCount; s < stepCount + 1; s++)
            {
                float2 dir = new float2(math.sin(curAngle + steerAngleRad * s), math.cos(curAngle + steerAngleRad * s));
                int2 texPos = new int2(mapPos + dir * sampleDist);
                float amount = PheromoneMapUtil.GetAmount(ref pheromoneMap, texPos.x, texPos.y);
                
                //Debug.Log($"Step:{s}, SteerAngleDeg:{math.degrees(s * steerAngleRad)}, texPos:{texPos}");
                angle += steerAngleRad * s * amount;
            }
            
            //Debug.Log($"Best step:{bestStep}, SteerAngleDeg:{math.degrees(bestStep * steerAngleRad)}");
            pheromoneDirection.ValueRW.Angle = angle;
        }
    }
}