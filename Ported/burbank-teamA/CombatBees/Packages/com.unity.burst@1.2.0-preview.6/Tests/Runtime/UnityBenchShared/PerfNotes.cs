using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[assembly: InternalsVisibleTo("Burst.Benchmarks")]

namespace UnityBenchShared
{
    // These test cases are from the `doc/perf-notes.md` document

    internal static class PerfNotes
    {
        public static int BenchArraySize = 20 * 1024;
    }

    internal struct PartialWrite : IJob, IDisposable
    {
        [ReadOnly] public NativeArray<int> a;
        public NativeArray<int> b;

        public struct Provider : IArgumentProvider
        {
            public object Value
            {
                get
                {
                    var length = PerfNotes.BenchArraySize;
                    var job = new PartialWrite()
                    {
                        a = new NativeArray<int>(length, Allocator.Persistent),
                        b = new NativeArray<int>(length, Allocator.Persistent)
                    };

                    var random = new System.Random(0);
                    for (int i = 0; i < length; i++)
                    {
                        job.a[i] = random.Next(200);
                        job.b[i] = random.Next();
                    }

                    return job;
                }
            }
        }

        public void Execute()
        {
            for (int i = 0; i < a.Length; ++i)
            {
                int av = a[i];

                if (av > 100)
                {
                    b[i] = av;
                }
            }
        }

        public void Dispose()
        {
            a.Dispose();
            b.Dispose();
        }
    }

    internal struct PartialWriteWorkaround : IJob, IDisposable
    {
        [ReadOnly] public NativeArray<int> a;
        public NativeArray<int> b;

        public struct Provider : IArgumentProvider
        {
            public object Value
            {
                get
                {
                    var length = PerfNotes.BenchArraySize;
                    var job = new PartialWriteWorkaround()
                    {
                        a = new NativeArray<int>(length, Allocator.Persistent),
                        b = new NativeArray<int>(length, Allocator.Persistent)
                    };

                    var random = new System.Random(0);
                    for (int i = 0; i < length; i++)
                    {
                        job.a[i] = random.Next(200);
                        job.b[i] = random.Next();
                    }

                    return job;
                }
            }
        }

        public void Execute()
        {
            for (int i = 0; i < a.Length; ++i)
            {
                int av = a[i];
                int v = b[i];

                if (av > 100)
                {
                    v = av;
                }

                b[i] = v;
            }
        }

        public void Dispose()
        {
            a.Dispose();
            b.Dispose();
        }
    }

    internal struct IntToFloatPenalty : IJob, IDisposable
    {
        [ReadOnly] public NativeArray<float> b;

        public struct Provider : IArgumentProvider
        {
            public object Value
            {
                get
                {
                    var length = PerfNotes.BenchArraySize;
                    var job = new IntToFloatPenalty()
                    {
                        b = new NativeArray<float>(length, Allocator.Persistent)
                    };

                    return job;
                }
            }
        }

        public void Execute()
        {
            int k = 100;
            for (int i = 0; i < b.Length; ++i)
            {
                b[i] = k;
                ++k;
            }
        }

        public void Dispose()
        {
            b.Dispose();
        }
    }

    internal struct DivisionByScalar : IJob, IDisposable
    {
        float divisor;
        [ReadOnly] public NativeArray<float> a;
        public NativeArray<float> b;

        public struct Provider : IArgumentProvider
        {
            public object Value
            {
                get
                {
                    var length = PerfNotes.BenchArraySize;
                    var job = new DivisionByScalar()
                    {
                        divisor = 13,
                        a = new NativeArray<float>(length, Allocator.Persistent),
                        b = new NativeArray<float>(length, Allocator.Persistent)
                    };

                    var random = new System.Random(0);
                    for (int i = 0; i < length; i++)
                    {
                        job.a[i] = random.Next();
                    }

                    return job;
                }
            }
        }

        public void Execute()
        {
            for (int i = 0; i < a.Length; ++i)
            {
                b[i] = a[i] / divisor;
            }
        }

        public void Dispose()
        {
            a.Dispose();
            b.Dispose();
        }
    }

    internal struct SquareRoot : IJob, IDisposable
    {
        [ReadOnly] public NativeArray<float> a;
        public NativeArray<float> b;

        public struct Provider : IArgumentProvider
        {
            public object Value
            {
                get
                {
                    var length = PerfNotes.BenchArraySize;
                    var job = new SquareRoot()
                    {
                        a = new NativeArray<float>(length, Allocator.Persistent),
                        b = new NativeArray<float>(length, Allocator.Persistent)
                    };

                    var random = new System.Random(0);
                    for (int i = 0; i < length; i++)
                    {
                        job.a[i] = random.Next();
                    }

                    return job;
                }
            }
        }


        public void Execute()
        {
            for (int i = 0; i < a.Length; ++i)
            {
                b[i] = math.sqrt(a[i]);
            }
        }

        public void Dispose()
        {
            a.Dispose();
            b.Dispose();
        }
    }

    internal struct SquareRootRecip : IJob, IDisposable
    {
        [ReadOnly] public NativeArray<float> a;
        public NativeArray<float> b;

        public struct Provider : IArgumentProvider
        {
            public object Value
            {
                get
                {
                    var length = PerfNotes.BenchArraySize;
                    var job = new SquareRootRecip()
                    {
                        a = new NativeArray<float>(length, Allocator.Persistent),
                        b = new NativeArray<float>(length, Allocator.Persistent)
                    };

                    var random = new System.Random(0);
                    for (int i = 0; i < length; i++)
                    {
                        job.a[i] = random.Next();
                    }

                    return job;
                }
            }
        }

        public void Execute()
        {
            for (int i = 0; i < a.Length; ++i)
            {
                b[i] = math.rsqrt(a[i]);
            }
        }

        public void Dispose()
        {
            a.Dispose();
            b.Dispose();
        }
    }

    internal struct RedundantStore : IJob, IDisposable
    {
        public int sum;
        public NativeArray<int> a;

        public struct Provider : IArgumentProvider
        {
            public object Value
            {
                get
                {
                    var length = PerfNotes.BenchArraySize;
                    var job = new RedundantStore()
                    {
                        a = new NativeArray<int>(length, Allocator.Persistent)
                    };

                    var random = new System.Random(0);
                    for (int i = 0; i < length; i++)
                    {
                        job.a[i] = random.Next();
                    }

                    return job;
                }
            }
        }

        public void Execute()
        {
            for (int i = 0; i < a.Length; ++i)
            {
                sum += a[i];
            }
        }

        public void Dispose()
        {
            a.Dispose();
        }
    }

    internal struct RedundantStoreWorkaround : IJob, IDisposable
    {
        public int sum;
        public NativeArray<int> a;

        public struct Provider : IArgumentProvider
        {
            public object Value
            {
                get
                {
                    var length = PerfNotes.BenchArraySize;
                    var job = new RedundantStoreWorkaround()
                    {
                        a = new NativeArray<int>(length, Allocator.Persistent)
                    };

                    var random = new System.Random(0);
                    for (int i = 0; i < length; i++)
                    {
                        job.a[i] = random.Next();
                    }

                    return job;
                }
            }
        }

        public void Execute()
        {
            for (int i = 0; i < a.Length; ++i)
            {
                sum += a[i];
            }
        }

        public void Dispose()
        {
            a.Dispose();
        }
    }

    /* TODO: Is from the notes, but we don't have have a mock of IComponentData 
    public struct SimpleComponentData : IComponentData
    {
        public int val;
    }

    public struct SimpleComponentDataTest : IJob
    {
        public ComponentDataArray<SimpleComponentData> a;

        public void Execute()
        {
            for (int i = 0; i < a.Length; ++i)
            {
                var item = a[i];
                item.val += 9999;
                a[i] = item;
            }
        }
    }
    */
}
