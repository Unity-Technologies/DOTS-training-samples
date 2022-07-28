using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

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
        foreach (var fireFighterLine in SystemAPI.Query<RefRO<FireFighterLine>>())
        {
            allStartPositions[fireFighterLine.ValueRO.LineId] = fireFighterLine.ValueRO.StartPosition;
            allEndPositions[fireFighterLine.ValueRO.LineId] = fireFighterLine.ValueRO.EndPosition;
        }

        // Creating an instance of the job.
        var bucketBringerFindNewTarget = new BucketBringerFindNewTarget
        {
            StartPositions = allStartPositions,
            EndPositions = allEndPositions,
            FFCountPerLine = ffConfig.PerLinesCount,
            CellSize = tConfig.CellSize
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
    
    // Note that the TurretAspects parameter is "in", which declares it as read only.
    // Making it "ref" (read-write) would not make a difference in this case, but you
    // will encounter situations where potential race conditions trigger the safety system.
    // So in general, using "in" everywhere possible is a good principle.
    void Execute( ref LineId lineId, ref LineIndex lineIndex, ref Target target)
    {
        var multiplier = ((float)lineIndex.Value / (float)(FFCountPerLine - 1));
        var pos = EndPositions[lineId.Value] + multiplier * (StartPositions[lineId.Value] - EndPositions[lineId.Value]);
        target.Value = pos;
    }
}