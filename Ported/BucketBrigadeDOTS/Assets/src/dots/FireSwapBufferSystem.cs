using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class FireSwapBufferSystem : SystemBase
{
    protected override void OnCreate()
    {
        GetEntityQuery(typeof(FireCell));
    }

    [BurstCompile]
    struct CopyBufferJob : IJobParallelFor
    {
        public NativeArray<FireCellHistory> bufferDst;
        [ReadOnly]
        public NativeArray<FireCell> bufferSrc;

        public void Execute(int index)
        {
            FireCellHistory cell;
            cell.FireTemperaturePrev = bufferSrc[index].FireTemperature;
            bufferDst[index] = cell;
        }
    }

    protected override void OnUpdate()
    {
        var fireGridEntity = GetSingletonEntity<FireGridSettings>();
        var bufferSrc = EntityManager.GetBuffer<FireCell>(fireGridEntity);
        var bufferDst = EntityManager.GetBuffer<FireCellHistory>(fireGridEntity);

        var bufferCopy = new CopyBufferJob
        {
            bufferSrc = bufferSrc.AsNativeArray(),
            bufferDst = bufferDst.AsNativeArray(),
        };

        JobHandle jobHandle = bufferCopy.Schedule(bufferDst.Length, 256);
        jobHandle.Complete();
    }
}
