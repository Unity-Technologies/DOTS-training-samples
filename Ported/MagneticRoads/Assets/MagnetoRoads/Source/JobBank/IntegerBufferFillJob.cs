using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Magneto.JobBank
{
    [BurstCompile]
    public struct IntegerBufferFillJob : IJobParallelFor
    {
        [WriteOnly] public NativeArray<int> Buffer;
        [ReadOnly] public int FillValue;

        public void Execute(int index)
        {
            Buffer[index] = FillValue;
        }
    }
}