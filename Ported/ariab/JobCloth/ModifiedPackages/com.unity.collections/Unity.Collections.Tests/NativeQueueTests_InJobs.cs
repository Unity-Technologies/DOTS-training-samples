using NUnit.Framework;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;

public class NativeQueueTests_InJobs
{
	struct ConcurrentEnqueue : IJobParallelFor
	{
		public NativeQueue<int>.Concurrent queue;
		public NativeArray<int> result;

		public void Execute(int index)
		{
			result[index] = 1;
			try
			{
				queue.Enqueue(index);
			}
			catch (System.Exception)
			{
				result[index] = 0;
			}
		}
	}

	[Test]
	public void Enqueue()
	{
		const int queueSize = 100*1024;
		var queue = new NativeQueue<int>(Allocator.TempJob);
		var writeStatus = new NativeArray<int>(queueSize, Allocator.TempJob);

		var enqueueJob = new ConcurrentEnqueue();
		enqueueJob.queue = queue.ToConcurrent();
		enqueueJob.result = writeStatus;

		enqueueJob.Schedule(queueSize, 1).Complete();

		Assert.AreEqual(queueSize, queue.Count, "Job enqueued the wrong number of values");
		var allValues = new HashSet<int>();
		for (int i = 0; i < queueSize; ++i)
		{
			Assert.AreEqual(1, writeStatus[i], "Job failed to enqueue value");
			int enqueued = queue.Dequeue();
			Assert.IsTrue(enqueued >= 0 && enqueued < queueSize, "Job enqueued invalid value");
			Assert.IsTrue(allValues.Add(enqueued), "Job enqueued same value multiple times");
		}

		queue.Dispose();
		writeStatus.Dispose();
	}

    struct EnqueueDequeueJob : IJob
    {
        public NativeQueue<int> q;
        [ReadOnly] public NativeArray<int> arr;
        public int val;

        public void Execute()
        {
            for (int i = 0; i < 10000; ++i)
            {
                q.Enqueue(0);
                val += arr[q.Dequeue()];
            }
        }

    }

    [Test]
	public void EnqueueDequeueMultipleQueuesInMultipleJobs()
	{
	    var handles = new NativeArray<JobHandle>(4, Allocator.Temp);
	    for (int i = 0; i < 10; ++i)
	    {
            var q1 = new NativeQueue<int>(Allocator.TempJob);
            var q2 = new NativeQueue<int>(Allocator.TempJob);
            var q3 = new NativeQueue<int>(Allocator.TempJob);
            var q4 = new NativeQueue<int>(Allocator.TempJob);
            var rangeCheck = new NativeArray<int>(1, Allocator.TempJob);
            var j1 = new EnqueueDequeueJob {q = q1, arr = rangeCheck, val = 0};
            var j2 = new EnqueueDequeueJob {q = q2, arr = rangeCheck, val = 0};
            var j3 = new EnqueueDequeueJob {q = q3, arr = rangeCheck, val = 0};
            var j4 = new EnqueueDequeueJob {q = q4, arr = rangeCheck, val = 0};
	        handles[0] = j1.Schedule();
	        handles[1] = j2.Schedule();
	        handles[2] = j3.Schedule();
	        handles[3] = j4.Schedule();
	        JobHandle.ScheduleBatchedJobs();

	        JobHandle.CombineDependencies(handles).Complete();

            q1.Dispose();
            q2.Dispose();
            q3.Dispose();
            q4.Dispose();
            rangeCheck.Dispose();
	    }
	    handles.Dispose();
	}
}
