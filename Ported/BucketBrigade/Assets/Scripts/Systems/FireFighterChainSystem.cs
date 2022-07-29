using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct FireFighterChainSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    int posToIndex(float2 pos, int GridSize, float CellSize)
    {
        pos += GridSize * CellSize * 0.5f;
        return (int)(math.floor(pos.x / (CellSize)) % GridSize + math.floor(pos.y / (CellSize)) * GridSize);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var terrainConfig = SystemAPI.GetSingleton<TerrainCellConfig>();
        int GridSize = terrainConfig.GridSize;
        float CellSize = terrainConfig.CellSize;

        var config = SystemAPI.GetSingleton<FireFighterConfig>();

        var HeatMap = SystemAPI.GetSingletonBuffer<Temperature>();
        foreach (var (FireFighterPos, target, FireFighterBucketId, lineId, lineIndex, WaterBringerState) in SystemAPI.Query<RefRO<Translation>, RefRW<Target>, RefRW<BucketId>, RefRO<LineId>, RefRO<LineIndex>, RefRW<WaterBringer>>().WithAll<WaterBringer>())
        {
            FireFighterLine fireFighterLine = new FireFighterLine();
            float distToNext = 1.0f;
            float2 posToNext = new float2();
            float2 idlePos = new float2();
            foreach (var line in SystemAPI.Query<RefRO<FireFighterLine>>())
            {
                if (line.ValueRO.LineId == lineId.ValueRO.Value)
                {
                    fireFighterLine = line.ValueRO;
                    distToNext = 0.25f * math.distancesq(fireFighterLine.StartPosition, fireFighterLine.EndPosition) / (config.PerLinesCount * config.PerLinesCount);
                    float2 direction = (fireFighterLine.EndPosition - fireFighterLine.StartPosition) / config.PerLinesCount;
                    posToNext = fireFighterLine.StartPosition + direction * (lineIndex.ValueRO.Value + 1);
                    idlePos = fireFighterLine.StartPosition + direction * lineIndex.ValueRO.Value;
                    break;
                }
            }

            switch (WaterBringerState.ValueRO.State)
            {
                case WaterBringer.WaterBringerState.GoToIdle:
                    {
                        if (math.distancesq(idlePos, FireFighterPos.ValueRO.Value.xz) < 0.01f)
                        {
                            WaterBringerState.ValueRW.State = WaterBringer.WaterBringerState.GoToFullBucket;
                        }
                        else
                        {
                            target.ValueRW.Value = idlePos;
                        }
                    }
                    break;
                case WaterBringer.WaterBringerState.GoToFullBucket:
                    {
                        bool hasBucket = false;
                        foreach (var (bucketInfo, bucketPos, volume, bucketId) in SystemAPI.Query<RefRW<BucketInfo>, RefRO<Translation>, RefRO<Volume>, RefRO<BucketId>>())
                        {
                            var dist = math.distancesq(bucketPos.ValueRO.Value, FireFighterPos.ValueRO.Value);
                            if (!bucketInfo.ValueRO.IsTaken && volume.ValueRO.Value == 1.0 && dist < distToNext )
                            {
                                hasBucket = true;
                                if (dist < 0.1f)
                                {
                                    FireFighterBucketId.ValueRW.Value = bucketId.ValueRO.Value;
                                    bucketInfo.ValueRW.IsTaken = true;
                                    WaterBringerState.ValueRW.State = WaterBringer.WaterBringerState.GoToNextFireFighter;
                                }
                                else
                                {
                                    target.ValueRW.Value = new float2(bucketPos.ValueRO.Value.x, bucketPos.ValueRO.Value.z);
                                }
                                break;
                            }
                        }
                        if(!hasBucket)
                        {
                            WaterBringerState.ValueRW.State = WaterBringer.WaterBringerState.GoToIdle;
                        }
                    }
                    break;
                case WaterBringer.WaterBringerState.GoToNextFireFighter:
                    {
                        if(math.distancesq(posToNext, FireFighterPos.ValueRO.Value.xz) < 0.01f)
                        {
                            // Drop the bucket
                            if(FireFighterBucketId.ValueRO.Value != -1)
                            {
                                foreach (var (bucketInfo, bucketPos, volume, bucketId) in SystemAPI.Query<RefRW<BucketInfo>, RefRO<Translation>, RefRW<Volume>, RefRO<BucketId>>())
                                {
                                    if (FireFighterBucketId.ValueRO.Value == bucketId.ValueRO.Value)
                                    {
                                        bucketInfo.ValueRW.IsTaken = false;
                                        FireFighterBucketId.ValueRW.Value = -1;


                                        WaterBringerState.ValueRW.State = WaterBringer.WaterBringerState.GoToIdle;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                WaterBringerState.ValueRW.State = WaterBringer.WaterBringerState.GoToIdle;
                            }
                        }
                        // Come back with the bucket
                        else if(FireFighterBucketId.ValueRO.Value != -1)
                        {
                            target.ValueRW.Value = posToNext;

                            if(FireFighterBucketId.ValueRO.Value != -1)
                            {
                                foreach (var (bucketId, bucketPos, volume) in SystemAPI.Query<RefRW<BucketId>, RefRW<Translation>, RefRW<Volume>>())
                                {
                                    if (FireFighterBucketId.ValueRO.Value == bucketId.ValueRO.Value)
                                    {
                                        bucketPos.ValueRW.Value = FireFighterPos.ValueRO.Value;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            WaterBringerState.ValueRW.State = WaterBringer.WaterBringerState.GoToIdle;
                        }
                    }
                    break;
            }
        }
    }
}