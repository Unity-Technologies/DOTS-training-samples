using UnityEngine;
using NUnit.Framework;
using System;
using Unity.Jobs;
using Unity.Collections;

#pragma warning disable 0219
#pragma warning disable 0414

public class NativeListJobDebuggerTests
{
    struct NativeListAddJob : IJob
    {
        NativeList<int> list;

        public NativeListAddJob(NativeList<int> list) { this.list = list; }

        public void Execute()
        {
            list.Add(1);
        }
    }

    struct NativeArrayTest : IJob
    {
        NativeArray<int> array;

        public NativeArrayTest(NativeArray<int> array) { this.array = array; }

        public void Execute()
        {
        }
    }

    [Test]
    public void AddElementToListFromJobInvalidatesArray()
    {
        var list = new NativeList<int>(Allocator.TempJob);
        list.Add(0);

        NativeArray<int> arrayBeforeSchedule = list;
        Assert.AreEqual(list.Length, 1);

        var jobData = new NativeListAddJob(list);
        var job = jobData.Schedule();

        Assert.Throws<System.InvalidOperationException>(() => { Debug.Log(arrayBeforeSchedule[0]); });
        Assert.Throws<System.InvalidOperationException>(() => { NativeArray<int> array = list; Debug.Log(array.Length); });
        Assert.Throws<System.InvalidOperationException>(() => { Debug.Log(list.Capacity); });
        Assert.Throws<System.InvalidOperationException>(() => { list.Dispose(); });
        Assert.Throws<System.InvalidOperationException>(() => { Debug.Log(list[0]); });

        job.Complete();

        Assert.AreEqual(1, arrayBeforeSchedule.Length);
        Assert.Throws<System.InvalidOperationException>(() => { Debug.Log(arrayBeforeSchedule[0]); });

        Assert.AreEqual(2, list.Length);
        Assert.AreEqual(0, list[0]);
        Assert.AreEqual(1, list[1]);

        NativeArray<int> arrayAfter = list;
        Assert.AreEqual(2, arrayAfter.Length);
        Assert.AreEqual(0, arrayAfter[0]);
        Assert.AreEqual(1, arrayAfter[1]);

        list.Dispose();
    }

    [Test]
    public void AccessBefore()
    {
        var list = new NativeList<int>(Allocator.TempJob);

        var jobHandle = new NativeListAddJob(list).Schedule();
        Assert.Throws<System.InvalidOperationException>(() =>
        {
           list.AsArray();
        });

        jobHandle.Complete();
        list.Dispose();
    }

    [Test]
    public void AccessAfter()
    {
        var list = new NativeList<int>(Allocator.TempJob);
        var array = list.AsArray();
        var jobHandle = new NativeListAddJob(list).Schedule();
        Assert.Throws<System.InvalidOperationException>(() =>
       {
           new NativeArrayTest(array).Schedule(jobHandle);
       });
        jobHandle.Complete();

        list.Dispose();
    }

    [Test]
    public void ScheduleDerivedArrayAllowDerivingArrayAgain()
    {
        var list = new NativeList<int>(1, Allocator.Persistent);

        // The scheduled job only receives a NativeArray thus it can't be resized
        var writeJobHandle = new NativeArrayTest(list).Schedule();

        // For that reason casting here is legal, as opposed to AddElementToListFromJobInvalidatesArray case where it is not legal
        // Since we NativeList is passed to the job
#pragma warning disable 0219 // assigned but its value is never used
        NativeArray<int> array = list;
#pragma warning restore 0219

        list.Dispose(writeJobHandle);
    }

    [Test]
    public void ScheduleDerivedArrayExceptions()
    {
        var list = new NativeList<int>(1, Allocator.Persistent);

        var addListJobHandle = new NativeListAddJob(list).Schedule();
#pragma warning disable 0219 // assigned but its value is never used
        Assert.Throws<System.InvalidOperationException>(() => { NativeArray<int> array = list; });
#pragma warning restore 0219

        addListJobHandle.Complete();
        list.Dispose();
    }

    [Test]
    public void ScheduleDerivedArrayExceptions2()
    {
        var list = new NativeList<int>(1, Allocator.Persistent);
        NativeArray<int> array = list;

        var addListJobHandle = new NativeListAddJob(list).Schedule();
        // The array previously cast should become invalid
        // as soon as the job is scheduled, since we can't predict if an element will be added or not
        Assert.Throws<System.InvalidOperationException>(() => { new NativeArrayTest(array).Schedule(); });

        addListJobHandle.Complete();
        list.Dispose();
    }



    struct ReadOnlyListAccess : IJob
    {
        [ReadOnly]
        NativeList<int> list;

        public ReadOnlyListAccess(NativeList<int> list) { this.list = list; }

        public void Execute()
        {
        }
    }

    [Test]
    public void ReadOnlyListInJobKeepsAsArrayValid()
    {
        var list = new NativeList<int>(Allocator.TempJob);
        list.Add(0);
        var arrayBeforeSchedule = list.AsArray();

        var jobData = new ReadOnlyListAccess(list);
        var job = jobData.Schedule();
        job.Complete();

        Assert.AreEqual(0, arrayBeforeSchedule[0]);

        list.Dispose();
    }


    [Test]
    public void AsArrayJobKeepsAsArrayValid()
    {
        var list = new NativeList<int>(Allocator.TempJob);
        list.Add(0);
        var arrayBeforeSchedule = list.AsArray();

        var jobData = new NativeArrayTest(list);
        var job = jobData.Schedule();
        job.Complete();

        Assert.AreEqual(0, arrayBeforeSchedule[0]);

        list.Dispose();
    }


    struct NativeListToArrayConversionFromJob : IJob
    {
        public NativeList<int> list;

        public void Execute()
        {
            list.Add(0);
            list.Add(0);

            NativeArray<int> arr = list;
            arr[0] = 1;
            arr[1] = 2;
        }
    }

    [Test]
    public void CastListToArrayInsideJob()
    {
        var jobData = new NativeListToArrayConversionFromJob();
        jobData.list = new NativeList<int>(1, Allocator.Persistent);
        jobData.Schedule().Complete();

        Assert.AreEqual(new int[] { 1, 2 }, jobData.list.ToArray());
        jobData.list.Dispose();
    }

    struct WriteJob : IJobParallelFor
    {
        public NativeArray<float> output;

        public void Execute(int i)
        {
            output[i] = i;
        }
    }

    [Test]
    public void WriteToArrayFromJobThenReadListFromMainThread()
    {
        var list = new NativeList<float>(1, Allocator.Persistent);
        list.Add(0);
        list.Add(1);

        for (int i = 0; i < 2; i++)
        {
            var writeJob = new WriteJob();
            writeJob.output = list;
            var writeJobHandle = writeJob.Schedule(list.Length, 1);

            Assert.Throws<InvalidOperationException>(() => { Debug.Log(writeJob.output[0]); });

            writeJobHandle.Complete();
        }
        list.Dispose();
    }


    [Test]
    public void DisposeJobWorks()
    {
        var list = new NativeList<int>(Allocator.Persistent);
        var deps = new NativeListAddJob(list).Schedule();
        deps = list.Dispose(deps);
        Assert.IsFalse(list.IsCreated);
        deps.Complete();
    }

    [Test]
    public void DisposeJobWithMissingDependencyThrows()
    {
        var list = new NativeList<int>(Allocator.Persistent);
        var deps = new NativeListAddJob(list).Schedule();
        Assert.Throws<InvalidOperationException>(() => { list.Dispose(default(JobHandle)); });
        deps.Complete();
        list.Dispose();
    }

    [Test]
    public void DisposeJobListCantBeScheduled()
    {
        var list = new NativeList<int>(Allocator.Persistent);
        var deps = list.Dispose(default(JobHandle));
        Assert.Throws<InvalidOperationException>(() => { new NativeListAddJob(list).Schedule(deps); });
        deps.Complete();
    }

    struct InvalidArrayAccessFromListJob : IJob
    {
        public NativeList<int> list;

        public void Execute()
        {
            list.Add(1);
            NativeArray<int> array = list;
            list.Add(2);

            Assert.Throws<InvalidOperationException>(() => { array[0] = 5; });
        }
    }
    [Test]
    public void InvalidatedArrayAccessFromListThrowsInsideJob()
    {
        var job = new InvalidArrayAccessFromListJob { list = new NativeList<int>(Allocator.TempJob) };
        job.Schedule().Complete();
        job.list.Dispose();
    }

    [Test]
    public void DisposeAliasedArrayThrows()
    {
        var list = new NativeList<int>(Allocator.Persistent);
        var array = list.AsArray();
        Assert.Throws<InvalidOperationException>(() => { array.Dispose(); });

        list.Dispose();
    }

    struct NativeArrayTestReadOnly : IJob
    {
        [ReadOnly]
        NativeArray<int> array;

        public NativeArrayTestReadOnly(NativeArray<int> array) { this.array = array; }

        public void Execute()
        {
            var arr = array;
            Assert.Throws<InvalidOperationException>(() => { arr[0] = 5; });
            Assert.AreEqual(7, array[0]);
        }
    }

    [Test]
    public void ReadOnlyAliasedArrayThrows()
    {
        var list = new NativeList<int>(Allocator.Persistent);
        list.Add(7);
        new NativeArrayTestReadOnly(list).Schedule().Complete();

        list.Dispose();
    }

    struct NativeArrayTestWriteOnly : IJob
    {
        [WriteOnly]
        NativeArray<int> array;

        public NativeArrayTestWriteOnly(NativeArray<int> array) { this.array = array; }

        public void Execute()
        {
            var arr = array;
            Assert.Throws<InvalidOperationException>(() => { int read = arr[0]; });
            arr[0] = 7;
        }
    }

    [Test]
    public void AsParallel()
    {
        var list = new NativeList<int>(Allocator.Persistent);
        list.Add(0);

        var writer = list.AsParallelWriter();
        var writerJob = new NativeArrayTestWriteOnly(writer).Schedule();

        var reader = list.AsParallelReader();
        var readerJob = new NativeArrayTestReadOnly(reader).Schedule(writerJob);

        // Tests that read only container safety check trows...
        var writerJob2 = new NativeArrayTestWriteOnly(reader).Schedule(readerJob);

        // Tests that write only container safety check trows...
        var readerJob2 = new NativeArrayTestReadOnly(writer).Schedule(writerJob2);

        readerJob2.Complete();

        list.Dispose();
    }
}
