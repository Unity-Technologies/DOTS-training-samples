using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class FireSpreadSystem : SystemBase
{
    protected override void OnCreate()
    {
    }

    [BurstCompile]
    struct SpreadFireJob : IJobParallelFor
    {
        public uint2 resolution;
        [ReadOnly]
        public NativeArray<FireCellHistory> bufferSrc; // RO
        public NativeArray<FireCell> bufferDst;
        public float timeStep;

        public void Execute(int chunkIndex)
        { 
            int i = chunkIndex;

            int x = i % (int)resolution.x;
            int y = i / (int)resolution.x;

            int kernelRadius = 1;
            int2 rangeX = math.clamp(new int2(x - kernelRadius, x + kernelRadius), 0, (int)resolution.x - 1);
            int2 rangeY = math.clamp(new int2(y - kernelRadius, y + kernelRadius), 0, (int)resolution.y - 1);

            FireCell cell = bufferDst[i];
            cell.FireTemperature = bufferSrc[i].FireTemperaturePrev;
            for (int v = rangeY.x; v <= rangeY.y; ++v)
                for (int u = rangeX.x; u <= rangeX.y; ++u)
                {
                    float targetHistoryTemp = bufferSrc[v * (int)resolution.x + u].FireTemperaturePrev;
                    cell.FireTemperature += targetHistoryTemp > 0.2f ? targetHistoryTemp * 0.03f * timeStep : 0.0f;
                }

            cell.FireTemperature = math.min(cell.FireTemperature, 1.0f);
            bufferDst[i] = cell;
        }
    }

    protected override void OnUpdate()
    {
        var fireGridEntity = GetSingletonEntity<FireGridSettings>();
        var fireGridSetting = GetComponent<FireGridSettings>(fireGridEntity);
        var bufferSrc = EntityManager.GetBuffer<FireCellHistory>(fireGridEntity);
        var bufferDst = EntityManager.GetBuffer<FireCell>(fireGridEntity);

        var fireSim = new SpreadFireJob
        {
            resolution = fireGridSetting.FireGridResolution,
            bufferSrc = bufferSrc.AsNativeArray(),
            bufferDst = bufferDst.AsNativeArray(),
            timeStep = Time.DeltaTime,
        };

        JobHandle jobHandle = fireSim.Schedule(bufferDst.Length, 256);
        jobHandle.Complete();
    }
}
