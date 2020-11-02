using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Magneto.JobBank
{
    [BurstCompile]
    public struct IntegerBufferCopyJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<int> Source;
        [WriteOnly] public NativeArray<int> Destination;        
        
        public void Execute(int index)
        {
            Destination[index] = Source[index];
        }
    }
}