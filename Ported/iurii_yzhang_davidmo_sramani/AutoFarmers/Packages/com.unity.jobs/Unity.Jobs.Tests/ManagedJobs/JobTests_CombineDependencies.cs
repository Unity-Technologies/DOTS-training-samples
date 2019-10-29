using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;

namespace Unity.Jobs.Tests.ManagedJobs
{
    public class JobTests_CombineDependencies
    {
        struct ArrayJob1 : IJob
        {
            public NativeArray<int> data;

            public void Execute()
            {
                data[0] = data[0] + 1;
            }
        }

        struct ArrayJob2 : IJob
        {
            public NativeArray<int> a;
            public NativeArray<int> b;

            public void Execute()
            {
                a[0] = a[0] + 100;
                b[0] = b[0] + 100;
            }
        }

        [Test]
        public void CombineDependenciesWorks()
        {
            var arrayA = new NativeArray<int>(2, Allocator.Persistent);
            var arrayB = new NativeArray<int>(2, Allocator.Persistent);

            var jobA = new ArrayJob1 {data = arrayA};
            var jobAHandle = jobA.Schedule();

            var jobB = new ArrayJob1 {data = arrayB};
            var jobBHandle = jobB.Schedule();

            var combinedHandle = JobHandle.CombineDependencies(jobAHandle, jobBHandle);

            var job2 = new ArrayJob2
            {
                a = arrayA,
                b = arrayB
            };
            job2.Schedule(combinedHandle).Complete();

            for (int i = 0; i < arrayA.Length; ++i)
            {
                Assert.AreEqual(arrayA[0], arrayB[0]);
            }

            arrayA.Dispose();
            arrayB.Dispose();
        }

        public void DeepCombineDependencies(int depth, int arraySize)
        {
            var arrays = new NativeArray<int>[arraySize];
            for (var i = 0; i < arrays.Length; i++)
            {
                arrays[i] = new NativeArray<int>(1, Allocator.Persistent);
                arrays[i][0] = 0;
            }

            var handles = new NativeArray<JobHandle>(arrays.Length, Allocator.Persistent);

            var previousJobHandle = new JobHandle();
            for (var i = 0; i < depth; i++)
            {
                for (var a = 0; a != arrays.Length; a++)
                {
                    var job = new ArrayJob1 {data = arrays[a]};
                    handles[a] = job.Schedule(previousJobHandle);
                }

                var combinedHandle = JobHandle.CombineDependencies(handles);

                var job2 = new ArrayJob2
                {
                    a = arrays[0],
                    b = arrays[1]
                };
                previousJobHandle = job2.Schedule(combinedHandle);
            }

            previousJobHandle.Complete();

            Assert.AreEqual(100 * depth + depth, arrays[0][0]);
            Assert.AreEqual(100 * depth + depth, arrays[1][0]);

            for (var i = 2; i < arrays.Length; i++)
                Assert.AreEqual(depth, arrays[i][0]);

            for (var a = 0; a != arrays.Length; a++)
                arrays[a].Dispose();

            handles.Dispose();
        }

        [Test]
        public void DeepCombineDependenciesWorks()
        {
            DeepCombineDependencies(5, 21);
        }
    }
}