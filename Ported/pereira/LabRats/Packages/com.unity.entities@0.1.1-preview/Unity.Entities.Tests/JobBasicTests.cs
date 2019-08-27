using System;
using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;

namespace Unity.Entities.Tests
{
    // These are very basic tests. As we bring up the Tiny system,
    // it's useful to have super simple tests to make sure basics
    // are working.
    public class JobBasicTests : ECSTestsFixture
    {
        public static int didDispose;

        public struct TestDisposable : IDisposable
        {
            public void Dispose()
            {
                JobBasicTests.didDispose++;
            }
        }

        public struct SimpleJob : IJob
        {
            public const int N = 1000;

            public int a;
            public int b;
            public NativeArray<int> result;

            [DeallocateOnJobCompletion]
            public TestDisposable mDisposable;

            public void Execute()
            {
                for (int i = 0; i < N; ++i)
                    result[i] = a + b;
            }
        }

        [Test]
        public void RunSimpleJob()
        {
            didDispose = 0;
            SimpleJob job = new SimpleJob()
            {
                a = 5,
                b = 10
            };

            NativeArray<int> jobResult = new NativeArray<int>(SimpleJob.N, Allocator.TempJob);
            job.result = jobResult;

            job.Run();

#if NET_DOTS
            // TODO: didDispose does not get called for reflection (big-ECS), but does for code-gen (Tiny-ECS)
            // The code-gen is finding the DoDeallocateOnJobCompletion at compile time, and my be using
            // a slightly different rule set for finding the attribute.
            Assert.AreEqual(1, didDispose);
#endif

            for (int i = 0; i < SimpleJob.N; ++i)
            {
                Assert.AreEqual(15, jobResult[i]);
            }

            jobResult.Dispose();
        }

        public struct SimpleAdd : IJob
        {
            public const int N = 100 * 1000;

            public int a;
            [ReadOnly] public NativeArray<int> input;
            public NativeArray<int> result;

            [DeallocateOnJobCompletion] public TestDisposable mDisposable;

            public void Execute()
            {
                for (int i = 0; i < N; ++i)
                    result[i] = a + input[i];
            }
        }

        [Test]
        public void Run3SimpleJobsInSerial()
        {
            didDispose = 0;
            NativeArray<int> input = new NativeArray<int>(SimpleAdd.N, Allocator.TempJob);
            NativeArray<int> jobResult1 = new NativeArray<int>(SimpleAdd.N, Allocator.TempJob);
            NativeArray<int> jobResult2 = new NativeArray<int>(SimpleAdd.N, Allocator.TempJob);
            NativeArray<int> jobResult3 = new NativeArray<int>(SimpleAdd.N, Allocator.TempJob);

            for (int i = 0; i < SimpleAdd.N; ++i)
            {
                input[i] = i;
            }

            SimpleAdd job1 = new SimpleAdd() {a = 1, input = input, result = jobResult1};
            SimpleAdd job2 = new SimpleAdd() {a = 2, input = jobResult1, result = jobResult2};
            SimpleAdd job3 = new SimpleAdd() {a = 3, input = jobResult2, result = jobResult3};

            JobHandle handle1 = job1.Schedule();
            JobHandle handle2 = job2.Schedule(handle1);
            JobHandle handle3 = job3.Schedule(handle2);

            handle3.Complete();

#if NET_DOTS
            Assert.AreEqual(3, didDispose);
#endif

            for (int i = 0; i < SimpleAdd.N; ++i)
            {
                Assert.AreEqual(i + 1 + 2 + 3, jobResult3[i]);
            }

            input.Dispose();
            jobResult1.Dispose();
            jobResult2.Dispose();
            jobResult3.Dispose();
        }

        [Test]
        public void Run3SimpleJobsInParallel()
        {
            didDispose = 0;
            NativeArray<int> input = new NativeArray<int>(SimpleAdd.N, Allocator.TempJob);
            NativeArray<int> jobResult1 = new NativeArray<int>(SimpleAdd.N, Allocator.TempJob);
            NativeArray<int> jobResult2 = new NativeArray<int>(SimpleAdd.N, Allocator.TempJob);
            NativeArray<int> jobResult3 = new NativeArray<int>(SimpleAdd.N, Allocator.TempJob);

            for (int i = 0; i < SimpleAdd.N; ++i)
            {
                input[i] = i;
            }

            SimpleAdd job1 = new SimpleAdd() {a = 1, input = input, result = jobResult1};
            SimpleAdd job2 = new SimpleAdd() {a = 2, input = input, result = jobResult2};
            SimpleAdd job3 = new SimpleAdd() {a = 3, input = input, result = jobResult3};

            JobHandle handle1 = job1.Schedule();
            JobHandle handle2 = job2.Schedule();
            JobHandle handle3 = job3.Schedule();

            JobHandle[] arr = {handle1, handle2, handle3};
            NativeArray<JobHandle> group = new NativeArray<JobHandle>(arr, Allocator.TempJob);
            JobHandle handle = JobHandle.CombineDependencies(group);

            handle.Complete();

#if NET_DOTS
            Assert.AreEqual(3, didDispose);
#endif

            for (int i = 0; i < SimpleAdd.N; ++i)
            {
                Assert.AreEqual(i + 1, jobResult1[i]);
                Assert.AreEqual(i + 2, jobResult2[i]);
                Assert.AreEqual(i + 3, jobResult3[i]);
            }

            input.Dispose();
            jobResult1.Dispose();
            jobResult2.Dispose();
            jobResult3.Dispose();
            group.Dispose();
        }

        public struct SimpleParallelFor : IJobParallelFor
        {
            public const int N = 10 * 1000;

            [DeallocateOnJobCompletion] public TestDisposable mDisposable;

            [ReadOnly] public NativeArray<int> a;
            [ReadOnly] public NativeArray<int> b;

            public NativeArray<int> result;

            public void Execute(int i)
            {
                result[i] = a[i] + b[i];
            }
        }

        [Test]
        public void RunSimpleParallelFor()
        {
            didDispose = 0;
            NativeArray<int> a = new NativeArray<int>(SimpleParallelFor.N, Allocator.TempJob);
            NativeArray<int> b = new NativeArray<int>(SimpleParallelFor.N, Allocator.TempJob);
            NativeArray<int> result = new NativeArray<int>(SimpleParallelFor.N, Allocator.TempJob);

            for (int i = 0; i < SimpleParallelFor.N; ++i)
            {
                a[i] = 100 + i;
                b[i] = 200 + i;
            }

            SimpleParallelFor job = new SimpleParallelFor() {a = a, b = b, result = result};
            job.a = a;
            job.b = b;
            job.result = result;

            JobHandle handle = job.Schedule(result.Length, 300);
            handle.Complete();

#if NET_DOTS
            Assert.AreEqual(didDispose, 1);
#endif

            for (int i = 0; i < SimpleParallelFor.N; ++i)
            {
                Assert.AreEqual(result[i], 300 + i * 2);
            }

            a.Dispose();
            b.Dispose();
            result.Dispose();
        }

        public struct SimpleChunk : IJobChunk
        {
            [DeallocateOnJobCompletion] public TestDisposable mDisposable;
            public ArchetypeChunkComponentType<EcsTestData> testType;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<EcsTestData> chunkData = chunk.GetNativeArray(testType);

                for (int i = 0; i < chunk.Count; ++i)
                {
                    chunkData[i] = new EcsTestData() {value = 100 + chunkData[i].value};
                }
            }
        }

        [Test]
        public void RunSimpleIJobChunk()
        {
            didDispose = 0;
            const int N = 10 * 1000;
            NativeArray<Entity> eArr = new NativeArray<Entity>(N, Allocator.TempJob);
            var arch = m_Manager.CreateArchetype(typeof(EcsTestData));

            m_Manager.CreateEntity(arch, eArr);

            for (int i = 0; i < N; ++i)
            {
                m_Manager.SetComponentData(eArr[i], new EcsTestData() {value = 10 + i});
            }

            EntityQuery query = EmptySystem.GetEntityQuery(typeof(EcsTestData));
            var job = new SimpleChunk {testType = m_Manager.GetArchetypeChunkComponentType<EcsTestData>(false)};
            job.Schedule(query).Complete();

            for (int i = 0; i < N; ++i)
            {
                EcsTestData data = m_Manager.GetComponentData<EcsTestData>(eArr[i]);
                Assert.AreEqual(data.value, 10 + i + 100);
            }

#if NET_DOTS
            Assert.AreEqual(1, didDispose);
#endif

            eArr.Dispose();
        }
    }
}
