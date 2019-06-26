#if !UNITY_DOTSPLAYER
using NUnit.Framework;
using System;
using Unity.Collections;
using Unity.Collections.Experimental;
using Unity.Jobs;
using Unity.Burst;

class NativeArrayChunked8Tests
{
    struct T1
    {
        public uint A;
    }

    struct T2
    {
        public int A;
        public int B;
    }

    struct T3
    {
        public int A;
        public T2 B;
    }

    struct T4
    {
        public T3 A;
        public T2 B;
    }

    [Test]
    public void TestBasicCreateDestroy()
    {
        using (var a = new NativeArrayChunked8<T1>(150, Allocator.Temp))
        {
            Assert.AreEqual(Allocator.Temp, a.Allocator);
            Assert.AreEqual(150, a.Length);
        }
    }

    struct F1
    {
        byte a;
    }

    struct F2
    {
        short a;
    }

    struct F3
    {
        double a;
    }

    [Test]
    public void TestUnsupportedTypes()
    {
        Assert.Throws<ArgumentException>(() => new NativeArrayChunked8<F1>(150, Allocator.Temp));
        Assert.Throws<ArgumentException>(() => new NativeArrayChunked8<F2>(150, Allocator.Temp));
        Assert.Throws<ArgumentException>(() => new NativeArrayChunked8<F3>(150, Allocator.Temp));
    }

    [Test]
    public void TestIndexing1()
    {
        var a = new NativeArrayChunked8<T1>(150, Allocator.Temp);

        for (int i = 0; i < a.Length; ++i)
        {
            a[i] = new T1 { A = (uint) i };
        }

        for (int i = 0; i < a.Length; ++i)
        {
            var e = a[i];
            Assert.AreEqual(i, e.A);
        }

        a.Dispose();
    }

    [Test]
    public void TestIndexing2()
    {
        var a = new NativeArrayChunked8<T2>(150, Allocator.Temp);

        for (int i = 0; i < a.Length; ++i)
        {
            a[i] = new T2 {
                A = i,
                B = 900-i
            };
        }

        for (int i = 0; i < a.Length; ++i)
        {
            var e = a[i];
            Assert.AreEqual(i, e.A);
            Assert.AreEqual(900-i, e.B);
        }

        a.Dispose();
    }

    [Test]
    public void TestIndexing3()
    {
        var a = new NativeArrayChunked8<T4>(150, Allocator.Temp);

        for (int i = a.Length - 1; i >= 0; --i)
        {
            a[i] = new T4
            {
                A = new T3
                {
                    A = i,
                    B = new T2 { A = i * 13, B = 900 - i * 3 }
                },
                B = new T2 { A = i * 33, B = 900 - i * 5 },
            };
        }

        for (int i = 0; i < a.Length; ++i)
        {
            var e = a[i];
            Assert.AreEqual(i, e.A.A);
            Assert.AreEqual(i * 13, e.A.B.A);
            Assert.AreEqual(900 - i * 3, e.A.B.B);
            Assert.AreEqual(i * 33, e.B.A);
            Assert.AreEqual(900 - i * 5, e.B.B);
        }

        a.Dispose();
    }
}

class NativeArrayFullSOATests
{
    struct T1
    {
        public uint A;
    }

    struct T2
    {
        public int A;
        public int B;
    }

    struct T3
    {
        public int A;
        public T2 B;
    }

    struct T4
    {
        public T3 A;
        public T2 B;
    }

    [Test]
    public void TestBasicCreateDestroy()
    {
        using (var a = new NativeArrayFullSOA<T1>(150, Allocator.Temp))
        {
            Assert.AreEqual(Allocator.Temp, a.Allocator);
            Assert.AreEqual(150, a.Length);
        }
    }

    struct F1
    {
        byte a;
    }

    struct F2
    {
        short a;
    }

    struct F3
    {
        double a;
    }

    [Test]
    public void TestUnsupportedTypes()
    {
        Assert.Throws<ArgumentException>(() => new NativeArrayFullSOA<F1>(150, Allocator.Temp));
        Assert.Throws<ArgumentException>(() => new NativeArrayFullSOA<F2>(150, Allocator.Temp));
        Assert.Throws<ArgumentException>(() => new NativeArrayFullSOA<F3>(150, Allocator.Temp));
    }

    [Test]
    public void TestIndexing1()
    {
        var a = new NativeArrayFullSOA<T1>(150, Allocator.Temp);

        for (int i = 0; i < a.Length; ++i)
        {
            a[i] = new T1 { A = (uint) i };
        }

        for (int i = 0; i < a.Length; ++i)
        {
            var e = a[i];
            Assert.AreEqual(i, e.A);
        }

        a.Dispose();
    }

    [Test]
    public void TestIndexing2()
    {
        var a = new NativeArrayFullSOA<T2>(150, Allocator.Temp);

        for (int i = 0; i < a.Length; ++i)
        {
            a[i] = new T2 {
                A = i,
                B = 900-i
            };
        }

        for (int i = 0; i < a.Length; ++i)
        {
            var e = a[i];
            Assert.AreEqual(i, e.A);
            Assert.AreEqual(900-i, e.B);
        }

        a.Dispose();
    }

    [Test]
    public void TestIndexing3()
    {
        var a = new NativeArrayFullSOA<T4>(150, Allocator.Temp);

        for (int i = a.Length - 1; i >= 0; --i)
        {
            a[i] = new T4
            {
                A = new T3
                {
                    A = i,
                    B = new T2 { A = i * 13, B = 900 - i * 3 }
                },
                B = new T2 { A = i * 33, B = 900 - i * 5 },
            };
        }

        for (int i = 0; i < a.Length; ++i)
        {
            var e = a[i];
            Assert.AreEqual(i, e.A.A);
            Assert.AreEqual(i * 13, e.A.B.A);
            Assert.AreEqual(900 - i * 3, e.A.B.B);
            Assert.AreEqual(i * 33, e.B.A);
            Assert.AreEqual(900 - i * 5, e.B.B);
        }

        a.Dispose();
    }

    [BurstCompile(CompileSynchronously = true)]
    struct NativeArrayFullSOATestJob : IJob
    {
        internal NativeArrayFullSOA<T4> array;

        public void Execute()
        {
            T4 t4 = new T4();
            for (int i = 0; i < array.Length; i++)
            {
                t4.A.A = i;
                array[i] = t4;
            }
        }
    }

    [Test]
    public void WorksInBurstJob()
    {
        const int kLength = 100;
        var array = new NativeArrayFullSOA<T4>(kLength, Allocator.Persistent);
        var job = new NativeArrayFullSOATestJob { array = array };
        job.Schedule().Complete();
        for (int i = 0; i < kLength; ++i)
            Assert.AreEqual(i, array[i].A.A, "NativeArrayFullSOA failed to write values from within Burst job.");
        array.Dispose();
    }

}
#endif
