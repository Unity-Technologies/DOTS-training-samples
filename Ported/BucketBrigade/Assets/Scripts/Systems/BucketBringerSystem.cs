using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
partial struct BucketBringerSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FireFighterConfig>();
        state.RequireForUpdate<TerrainCellConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ffConfig = SystemAPI.GetSingleton<FireFighterConfig>();
        var tConfig = SystemAPI.GetSingleton<TerrainCellConfig>();

        var allStartPositions = new NativeArray<float2>(ffConfig.LinesCount, Allocator.TempJob);
        var allEndPositions = new NativeArray<float2>(ffConfig.LinesCount, Allocator.TempJob);
        var perpendicularVectors = new NativeArray<Vector2>(ffConfig.LinesCount, Allocator.TempJob);
        
        foreach (var fireFighterLine in SystemAPI.Query<RefRO<FireFighterLine>>())
        {
            allStartPositions[fireFighterLine.ValueRO.LineId] = fireFighterLine.ValueRO.StartPosition;
            allEndPositions[fireFighterLine.ValueRO.LineId] = fireFighterLine.ValueRO.EndPosition;
            
            // Calculate offsets
            Vector2 s = fireFighterLine.ValueRO.StartPosition;
            Vector2 e =   fireFighterLine.ValueRO.StartPosition + ((float)1 / (float)(ffConfig.PerLinesCount - 1)) * (fireFighterLine.ValueRO.EndPosition - fireFighterLine.ValueRO.StartPosition);
            perpendicularVectors[fireFighterLine.ValueRO.LineId] = Vector2.Perpendicular(s - e);
            
        }

        // Creating an instance of the job.
        var bucketBringerFindNewTarget = new BucketBringerFindNewTarget
        {
            StartPositions = allStartPositions,
            EndPositions = allEndPositions,
            FFCountPerLine = ffConfig.PerLinesCount,
            CellSize = tConfig.CellSize,
            Perpendiculars = perpendicularVectors
        };

        // Schedule execution in a single thread, and do not block main thread.
        bucketBringerFindNewTarget.Schedule();
    }
}

// Requiring the Shooting tag component effectively prevents this job from running
// for the tanks which are in the safe zone.
[WithAll(typeof(BucketBringer))]
[BurstCompile]
partial struct BucketBringerFindNewTarget : IJobEntity
{
    [ReadOnly]
    [DeallocateOnJobCompletion]
    public NativeArray<float2> StartPositions;
    
    [ReadOnly]
    [DeallocateOnJobCompletion]
    public NativeArray<float2> EndPositions;
    
    public int FFCountPerLine;
    public float CellSize;
    
    [ReadOnly]
    [DeallocateOnJobCompletion]
    public NativeArray<Vector2> Perpendiculars;
    
    // Note that the TurretAspects parameter is "in", which declares it as read only.
    // Making it "ref" (read-write) would not make a difference in this case, but you
    // will encounter situations where potential race conditions trigger the safety system.
    // So in general, using "in" everywhere possible is a good principle.
    void Execute( ref LineId lineId, ref LineIndex lineIndex, ref Position target)
    {
        var lineIndexCopy = lineIndex.Value;

        var multiplier = ((float)lineIndexCopy / (float)(FFCountPerLine - 1));
        var pos = EndPositions[lineId.Value] + multiplier * (StartPositions[lineId.Value] - EndPositions[lineId.Value]);
        var offset = lineIndexCopy <= (FFCountPerLine / 2f) ? 
            ((float)lineIndexCopy / (float)(FFCountPerLine/2)) : 
            ((float)FFCountPerLine - (float)lineIndexCopy - 1f)/(float)(FFCountPerLine/2f);

        target.Start = pos - new float2(Perpendiculars[lineId.Value].x * offset, Perpendiculars[lineId.Value].y * offset);
        lineIndexCopy = math.min(lineIndexCopy + 1, FFCountPerLine - 1);

        multiplier = ((float)lineIndexCopy / (float)(FFCountPerLine - 1));
        pos = EndPositions[lineId.Value] + multiplier * (StartPositions[lineId.Value] - EndPositions[lineId.Value]);
        offset = lineIndexCopy <= (FFCountPerLine / 2f) ?
            ((float)lineIndexCopy / (float)(FFCountPerLine / 2)) :
            ((float)FFCountPerLine - (float)lineIndexCopy - 1f) / (float)(FFCountPerLine / 2f);

        target.End = pos - new float2(Perpendiculars[lineId.Value].x * offset, Perpendiculars[lineId.Value].y * offset);
    }
}



[BurstCompile]
partial struct FireFighterAISystem : ISystem
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
        foreach (var (FireFighterDefaultPos, FireFighterPos, target, FireFighterBucketId, lineId, lineIndex, BucketBringerState) in SystemAPI.Query<RefRW<Position>, RefRW<Translation>, RefRW<Target>, RefRW<BucketId>, RefRO<LineId>, RefRO<LineIndex>, RefRW<BucketBringer>>().WithAll<BucketBringer>())
        {
            FireFighterLine fireFighterLine = new FireFighterLine();
            float distToNext = 1.0f;
            float2 posToNext = FireFighterDefaultPos.ValueRO.End;
            float2 idlePos = FireFighterDefaultPos.ValueRO.Start;
            foreach (var line in SystemAPI.Query<RefRO<FireFighterLine>>())
            {
                if (line.ValueRO.LineId == lineId.ValueRO.Value)
                {
                    fireFighterLine = line.ValueRO;
                    //distToNext = 0.25f * math.distancesq(fireFighterLine.StartPosition, fireFighterLine.EndPosition) / (config.PerLinesCount * config.PerLinesCount);
                    //float2 direction = (fireFighterLine.EndPosition - fireFighterLine.StartPosition) / config.PerLinesCount;
                    //posToNext = fireFighterLine.StartPosition + direction * (lineIndex.ValueRO.Value + 1);
                    //idlePos = fireFighterLine.StartPosition + direction * lineIndex.ValueRO.Value;
                    break;
                }
            }

            switch (BucketBringerState.ValueRO.State)
            {
                case BucketBringer.BucketBringerState.GoToIdle:
                    {
                        if (math.distancesq(idlePos, FireFighterPos.ValueRO.Value.xz) < 0.1)
                        {
                            BucketBringerState.ValueRW.State = BucketBringer.BucketBringerState.GoToEmptyBucket;
                        }
                        else
                        {
                            target.ValueRW.Value = idlePos;
                        }
                    }
                    break;
                case BucketBringer.BucketBringerState.GoToEmptyBucket:
                    {
                        bool hasBucket = false;
                        foreach (var (bucketInfo, bucketPos, volume, bucketId) in SystemAPI.Query<RefRW<BucketInfo>, RefRO<Translation>, RefRO<Volume>, RefRO<BucketId>>())
                        {
                            var dist = math.distancesq(bucketPos.ValueRO.Value, FireFighterPos.ValueRO.Value);
                            if (!bucketInfo.ValueRO.IsTaken && volume.ValueRO.Value == 0.0 && dist < distToNext)
                            {
                                hasBucket = true;
                                if (dist < 0.1f)
                                {
                                    FireFighterBucketId.ValueRW.Value = bucketId.ValueRO.Value;
                                    bucketInfo.ValueRW.IsTaken = true;
                                    BucketBringerState.ValueRW.State = BucketBringer.BucketBringerState.GoToNextFireFighter;
                                }
                                else
                                {
                                    target.ValueRW.Value = new float2(bucketPos.ValueRO.Value.x, bucketPos.ValueRO.Value.z);
                                }
                                break;
                            }
                        }
                        if (!hasBucket)
                        {
                            BucketBringerState.ValueRW.State = BucketBringer.BucketBringerState.GoToIdle;
                        }
                    }
                    break;
                case BucketBringer.BucketBringerState.GoToNextFireFighter:
                    {
                        if (math.distancesq(posToNext, FireFighterPos.ValueRO.Value.xz) < 0.01f)
                        {
                            // Drop the bucket
                            if (FireFighterBucketId.ValueRO.Value != -1)
                            {
                                foreach (var (bucketInfo, bucketPos, volume, bucketId) in SystemAPI.Query<RefRW<BucketInfo>, RefRO<Translation>, RefRW<Volume>, RefRO<BucketId>>())
                                {
                                    if (FireFighterBucketId.ValueRO.Value == bucketId.ValueRO.Value)
                                    {
                                        bucketInfo.ValueRW.IsTaken = false;
                                        FireFighterBucketId.ValueRW.Value = -1;


                                        BucketBringerState.ValueRW.State = BucketBringer.BucketBringerState.GoToIdle;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                BucketBringerState.ValueRW.State = BucketBringer.BucketBringerState.GoToIdle;
                            }
                        }
                        // Come back with the bucket
                        else if (FireFighterBucketId.ValueRO.Value != -1)
                        {
                            target.ValueRW.Value = posToNext;

                            if (FireFighterBucketId.ValueRO.Value != -1)
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
                            BucketBringerState.ValueRW.State = BucketBringer.BucketBringerState.GoToIdle;
                        }
                    }
                    break;
            }
        }
    }
}