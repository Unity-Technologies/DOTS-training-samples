using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;

namespace Unity.Jobs.Tests.ManagedJobs
{
    public class JobStressTests : JobTestsFixture
    {
        struct JobSetIndexValue : IJobParallelFor
        {
            public NativeArray<int> value;

            public void Execute(int index)
            {
                value[index] = index;
            }
        }

        [Test]
        public void StressTestParallelFor()
        {
            StressTestParallelForIterations(1, 5000);
        }

        public void StressTestParallelForIterations(int amount, int amountOfData)
        {
            for (var k = 0; k != amount; k++)
            {
                var len = UnityEngine.Random.Range(1, amountOfData);

                JobSetIndexValue job1;
                job1.value = new NativeArray<int>(len, Allocator.TempJob);

                JobSetIndexValue job2;
                job2.value = new NativeArray<int>(len, Allocator.TempJob);

                var job1Handle = job1.Schedule(len, UnityEngine.Random.Range(1, 1024));
                var job2Handle = job2.Schedule(len, UnityEngine.Random.Range(1, 1024));

                job2Handle.Complete();
                job1Handle.Complete();

                for (var i = 0; i < len; i++)
                {
                    if (job1.value[i] != i)
                        Assert.AreEqual(i, job1.value[i]);

                    if (job2.value[i] != i)
                        Assert.AreEqual(i, job2.value[i]);
                }

                job1.value.Dispose();
                job2.value.Dispose();
            }
        }

        struct JobSetValue : IJob
        {
            public int expected;
            public NativeArray<int> value;

            public void Execute()
            {
                value[0] = value[0] + 1;
            }
        }

        [Test]
        public void DeepDependencyChain()
        {
            var array = new NativeArray<int>(1, Allocator.Persistent);
            var jobHandle = new JobHandle();
            const int depth = 10000;
            for (var i = 0; i < depth; i++)
            {
                var job = new JobSetValue
                {
                    value = array,
                    expected = i
                };
                jobHandle = job.Schedule(jobHandle);
            }

            jobHandle.Complete();
            Assert.AreEqual(depth, array[0]);

            array.Dispose();
        }
    }
}