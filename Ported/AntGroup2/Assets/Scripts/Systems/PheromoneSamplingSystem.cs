using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[UpdateBefore(typeof(AntMovementSystem))]
[BurstCompile]
public partial struct PheromoneSamplingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state) {}
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var pheromoneMap = SystemAPI.GetSingletonBuffer<PheromoneMap>();
        
        float sampleDist = config.PheromoneSampleDistPixels * config.TimeScale * SystemAPI.Time.DeltaTime;
        float steerAngleRad = math.radians(config.PheromoneSampleStepAngle);
        int stepCount = config.PheromoneSampleStepCount;
        
    #if false
        foreach (var (transform, currentDirection, pheromoneDirection) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<CurrentDirection>, RefRW<PheromoneDirection>>().WithAll<Ant>())
        {
            float2 mapPos = PheromoneMapUtil.WorldToPheromoneMap(config.PlaySize, transform.ValueRO.Position.xz);
            float curAngle = currentDirection.ValueRO.Angle;

            #if true
            float bestAmount;
            float bestStep = 0;
            {
                float2 dir = new float2(math.sin(curAngle ), math.cos(curAngle ));
                int2 texPos = new int2(mapPos + dir * sampleDist);
                bestAmount = PheromoneMapUtil.GetAmount(in pheromoneMap, texPos.x, texPos.y);
                bestStep = 0;
            }
            
            for (int s = -stepCount; s < stepCount + 1; s++)
            {
                float2 dir = new float2(math.sin(curAngle + steerAngleRad * s), math.cos(curAngle + steerAngleRad * s));
                int2 texPos = new int2(mapPos + dir * sampleDist);
                float amount = PheromoneMapUtil.GetAmount(in pheromoneMap, texPos.x, texPos.y);

                if (amount > bestAmount)
                {
                    bestAmount = amount;
                    bestStep = s;
                }
            }
            pheromoneDirection.ValueRW.Angle = steerAngleRad * bestStep;// * bestAmount;
            #else
            
            
            float angle = 0;
            float totalAmount = 0;
            for (int s = -stepCount; s < stepCount + 1; s++)
            {
                float2 dir = new float2(math.sin(curAngle + steerAngleRad * s), math.cos(curAngle + steerAngleRad * s));
                int2 texPos = new int2(mapPos + dir * sampleDist);
                float amount = PheromoneMapUtil.GetAmount(in pheromoneMap, texPos.x, texPos.y);

                angle = steerAngleRad * s * amount;
                totalAmount += amount;
            }

            
            Debug.Log($"angle:{math.degrees(angle)}, totalAmount:{totalAmount}");
            angle /= math.max(totalAmount, 0.0001f);
            pheromoneDirection.ValueRW.Angle = angle;
            Debug.Log($"SteerAngleDeg:{math.degrees(angle)}");
            #endif

            //Debug.Log($"Best step:{bestStep}, SteerAngleDeg:{math.degrees(bestStep * steerAngleRad)}");

            
        }
    #else
        var job = new PheromoneSamplingJob()
        {
            pheromoneMap = pheromoneMap,
            playAreaSize =  config.PlaySize,
            stepCount = stepCount,
            stepSteerAngleRad = steerAngleRad,
            sampleDistance = sampleDist
        };
        job.ScheduleParallel();
#endif
    }
}

[BurstCompile]
[WithAll(typeof(Ant))]
partial struct PheromoneSamplingJob : IJobEntity
{
    [ReadOnly]
    public DynamicBuffer<PheromoneMap> pheromoneMap;
    
    public int playAreaSize;
    public int stepCount;
    public float stepSteerAngleRad;
    public float sampleDistance;
    public void Execute(in LocalTransform localTransform, in CurrentDirection currentDirection, ref PheromoneDirection pheromoneDirection)
    {
        float2 mapPos = PheromoneMapUtil.WorldToPheromoneMap(playAreaSize, localTransform.Position.xz);
        float curAngle = currentDirection.Angle;

        float angle = 0;
        #if false
        // Sum sampled angles weighted with the pheromone
        for (int s = -stepCount; s < stepCount + 1; s++)
        {
            float2 dir = new float2(math.sin(curAngle + stepSteerAngleRad * s), math.cos(curAngle + stepSteerAngleRad * s));
            int2 texPos = new int2(mapPos + dir * sampleDistance);
            float amount = PheromoneMapUtil.GetAmount(in pheromoneMap, texPos.x, texPos.y);
                
            //Debug.Log($"Step:{s}, SteerAngleDeg:{math.degrees(s * steerAngleRad)}, texPos:{texPos}");
            angle += stepSteerAngleRad * s * amount;
        }
        #elif true
        // Try to step with different angles and choose the best.
        
        // Init with straight value to go straight by default
        float bestAmount;
        float bestStep = 0;
        {
            float2 dir = new float2(math.sin(curAngle ), math.cos(curAngle ));
            int2 texPos = new int2(mapPos + dir * sampleDistance);
            bestAmount = PheromoneMapUtil.GetAmount(in pheromoneMap, texPos.x, texPos.y);
            bestStep = 0;
        }
            
        for (int s = -stepCount; s < stepCount + 1; s++)
        {
            if (s == 0)
                continue;
            
            float2 dir = new float2(math.sin(curAngle + stepSteerAngleRad * s), math.cos(curAngle + stepSteerAngleRad * s));
            int2 texPos = new int2(mapPos + dir * sampleDistance);
            float amount = PheromoneMapUtil.GetAmount(in pheromoneMap, texPos.x, texPos.y);

            if (amount > bestAmount)
            {
                bestAmount = amount;
                bestStep = s;
            }
        }
        angle = stepSteerAngleRad * bestStep;
        #else
        // Average sampled direction angles weighted by pheromone
        float totalAmount = 0;
        for (int s = -stepCount; s < stepCount + 1; s++)
        {
            float2 dir = new float2(math.sin(curAngle + stepSteerAngleRad * s), math.cos(curAngle + stepSteerAngleRad * s));
            int2 texPos = new int2(mapPos + dir * sampleDistance);
            float amount = PheromoneMapUtil.GetAmount(in pheromoneMap, texPos.x, texPos.y);

            angle = stepSteerAngleRad * s * amount;
            totalAmount += amount;
        }

            
        angle /= math.max(totalAmount, 0.0001f);
        angle = -angle;
        #endif
            
        pheromoneDirection.Angle = angle;
    }
}