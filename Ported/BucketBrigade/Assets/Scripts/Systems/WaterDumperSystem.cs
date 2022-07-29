using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct WaterDumperSystem : ISystem
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
        
        //var fireFigherLine = SystemAPI.Query<RefRO<FireFighterLine>>();
        
        var HeatMap = SystemAPI.GetSingletonBuffer<Temperature>();
        foreach (var (FireFighterPos, target, FireFighterBucketId, lineId, waterDumperState) in SystemAPI.Query<RefRO<Translation>, RefRW<Target>, RefRW<BucketId>, RefRO<LineId>, RefRW<WaterDumper>>().WithAll<WaterDumper>())
        {
            switch(waterDumperState.ValueRO.state)
            {
                case WaterDumper.WaterDumperState.GoToBucket:
                    {
                        bool hasBucket = false;
                        foreach (var (bucketInfo, bucketPos, volume, bucketId) in SystemAPI.Query<RefRW<BucketInfo>, RefRO<Translation>, RefRO<Volume>, RefRO<BucketId>>())
                        {
                            var dist = math.distancesq(bucketPos.ValueRO.Value, FireFighterPos.ValueRO.Value);
                            if (!bucketInfo.ValueRO.IsTaken && volume.ValueRO.Value > 0 && dist < 1.0f )
                            {
                                hasBucket = true;
                                if (dist < 0.1f)
                                {
                                    FireFighterBucketId.ValueRW.Value = bucketId.ValueRO.Value;
                                    bucketInfo.ValueRW.IsTaken = true;
                                    waterDumperState.ValueRW.state = WaterDumper.WaterDumperState.GoToFire;
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
                            waterDumperState.ValueRW.state = WaterDumper.WaterDumperState.GoToFire;
                        }
                    }
                    break;
                case WaterDumper.WaterDumperState.GoToFire:
                    {
                        foreach (var line in SystemAPI.Query<RefRO<FireFighterLine>>())
                        {
                            if (line.ValueRO.LineId == lineId.ValueRO.Value)
                            {
                                if(math.distancesq(line.ValueRO.EndPosition, FireFighterPos.ValueRO.Value.xz) < 0.01f)
                                {
                                    if(FireFighterBucketId.ValueRO.Value != -1)
                                    {
                                        foreach (var (bucketInfo, bucketPos, volume, bucketId) in SystemAPI.Query<RefRW<BucketInfo>, RefRO<Translation>, RefRW<Volume>, RefRO<BucketId>>())
                                        {
                                            if (FireFighterBucketId.ValueRO.Value == bucketId.ValueRO.Value)
                                            {
                                                bucketInfo.ValueRW.IsTaken = false;
                                                volume.ValueRW.Value = 0.0f;
                                                FireFighterBucketId.ValueRW.Value = -1;

                                                var coord = posToIndex(target.ValueRO.Value, GridSize, CellSize);

                                                HeatMap.ElementAt(math.clamp(coord + 0, 0, GridSize * GridSize)).Value = 0.0f;
                                                HeatMap.ElementAt(math.clamp(coord + 1, 0, GridSize * GridSize)).Value = 0.0f;
                                                HeatMap.ElementAt(math.clamp(coord - 1, 0, GridSize * GridSize)).Value = 0.0f;
                                                HeatMap.ElementAt(math.clamp(coord + GridSize, 0, GridSize * GridSize)).Value = 0.0f;
                                                HeatMap.ElementAt(math.clamp(coord - GridSize, 0, GridSize * GridSize)).Value = 0.0f;

                                                waterDumperState.ValueRW.state = WaterDumper.WaterDumperState.GoToBucket;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        waterDumperState.ValueRW.state = WaterDumper.WaterDumperState.GoToBucket;
                                    }
                                }
                                // Come back with the bucket
                                else
                                {
                                    target.ValueRW.Value = line.ValueRO.EndPosition;

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
                            }
                        }
                    }
                    break;
            }
        }
    }
}