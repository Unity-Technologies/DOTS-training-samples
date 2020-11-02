using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Magneto.JobBank
{
    [BurstCompile]
    public struct IntegerBufferSwapJob : IJobParallelFor
    {
        public NativeArray<int> BufferA;        
        public NativeArray<int> BufferB;
        
        public void Execute(int index)
        {
            int temp = BufferA[index];
            BufferA[index] = BufferB[index];
            BufferB[index] = temp;
        }
    }
}