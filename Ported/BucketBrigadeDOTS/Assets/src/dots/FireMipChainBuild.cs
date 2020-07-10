using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(FireSpreadSystem))]
public class FireMipChainBuild : SystemBase
{
    EntityQuery m_POIRequestQuery;

    protected override void OnCreate()
    {
        m_POIRequestQuery = GetEntityQuery(ComponentType.ReadOnly<PointOfInterestRequest>());
    }

    [BurstCompile]
    struct MipBuildJob : IJobParallelFor
    {
        public int2 resolutionPrevious;
        public int2 resolutionCurrent;
        public int offsetSrc;
        public int offsetDst;
        [NativeDisableParallelForRestriction]
        public NativeArray<FireCellFlag> mipChainBuffer;

        public void Execute(int chunkIndex)
        {
            int i = chunkIndex;

            int currentX = i % (int)resolutionCurrent.x;
            int currentY = i / (int)resolutionCurrent.x;

            FireCellFlag cellFlag;
            cellFlag.OnFire = false;
            for (int v = currentY * 2; v <= (currentY * 2 + 1); ++v)
                for (int u = currentX * 2; u <= (currentX * 2 + 1); ++u)
                {
                    cellFlag.OnFire |= mipChainBuffer[offsetSrc + v * (int)resolutionPrevious.x + u].OnFire;
                }
            mipChainBuffer[offsetDst + i] = cellFlag;
        }
    }

    static int ComputeNumberOfMips(int resolution)
    {
        int mip = 1;
        while (resolution != 1)
        {
            resolution >>= 1;
            mip++;
        }
        return mip;
    }

    protected override void OnUpdate()
    {
        // Grab all the data we need
        var fireGridEntity = GetSingletonEntity<FireGridSettings>();
        var fireGridSetting = GetComponent<FireGridSettings>(fireGridEntity);
        var mipChainBuffer = EntityManager.GetBuffer<FireCellFlag>(fireGridEntity);

        // If debug mode is not enabled and no PointOfInterestRequest exist return;
        if (fireGridSetting.MipDebugIndex < 0 && 
            m_POIRequestQuery.CalculateEntityCount() == 0)
            return;

        // Compute the number of  mips that we will have
        int mipChainCount = ComputeNumberOfMips((int)fireGridSetting.FireGridResolution.x);

        // This the base previous resolution
        int2 currentResolution = (int2)fireGridSetting.FireGridResolution;
        // We start from the beginning of the array
        int bufferOffset = 0;

        // The mip 0 is done outside, so we start from mip 1
        for (int mipIdx = 1; mipIdx < mipChainCount; ++mipIdx)
        {
            // Compute the data for the mip we are evaluating
            int localOffset = currentResolution.x * currentResolution.y;
            int2 halfResolution = (currentResolution >> 1);

            // Create the job
            var fireSim = new MipBuildJob
            {
                resolutionPrevious = currentResolution,
                resolutionCurrent = halfResolution,
                mipChainBuffer = mipChainBuffer.AsNativeArray(),
                offsetSrc = bufferOffset,
                offsetDst = bufferOffset + localOffset,
            };

            bufferOffset += localOffset;
            currentResolution = halfResolution;

            JobHandle jobHandle = fireSim.Schedule(halfResolution.x * halfResolution.y, 256);
            jobHandle.Complete();
        }
    }
}
