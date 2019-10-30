using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;

[assembly: InternalsVisibleTo("Burst.Benchmarks")]

namespace UnityBenchShared
{
    internal struct SumNumbersTest : IJob, IDisposable
    {
        public const int Count = 1000;

        public NativeArray<float> output;

        [ReadOnly]
        public NativeArray<float> a;
        [ReadOnly]
        public NativeArray<float> b;

        public void Execute()
        {
            for (int i = 0; i < Count; ++i)
            {
                output[i] = a[i] + b[i];
            }
        }

        public struct Provider : IArgumentProvider
        {
            public object Value
            {
                get
                {

                    var job = new SumNumbersTest();
                    job.output = new NativeArray<float>(Count, Allocator.Persistent);
                    job.a = new NativeArray<float>(Count, Allocator.Persistent);
                    job.b = new NativeArray<float>(Count, Allocator.Persistent);
                    for (int i = 0; i < Count; i++)
                    {
                        job.a[i] = i;
                    }
                    return job;
                }
            }
        }

        public void Dispose()
        {
            output.Dispose();
            a.Dispose();
            b.Dispose();
        }
    }
}
