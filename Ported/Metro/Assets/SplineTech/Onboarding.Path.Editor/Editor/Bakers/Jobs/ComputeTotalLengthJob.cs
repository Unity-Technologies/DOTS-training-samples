using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

namespace Onboarding.BezierPath
{
    [BurstCompile(FloatMode = FloatMode.Fast)]
    public struct ComputeTotalLengthJob : IJob
    {
        [ReadOnly]
        public NativeArray<float> segmentsLengths;

        public NativeArray<float> totalLength;

        public void Execute()
        {
            float length = 0;
            for (int i = 0; i < segmentsLengths.Length; ++i)
                length += segmentsLengths[i];

            totalLength[0] = length;
        }
    }
}
