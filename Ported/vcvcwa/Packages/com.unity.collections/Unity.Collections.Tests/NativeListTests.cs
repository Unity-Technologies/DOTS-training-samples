using NUnit.Framework;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.Tests;
using Unity.Jobs;

public class NativeListTests
{
    [Test]
    public void NullListThrow()
    {
        var list = new NativeList<int> ();
        Assert.Throws<NullReferenceException> (()=> list[0] = 5 );
        Assert.Throws<InvalidOperationException> (()=> list.Add(1) );
    }
    
	[Test]
	public void NativeList_Allocate_Deallocate_Read_Write()
	{
		var list = new NativeList<int> (Allocator.Persistent);

		list.Add(1);
		list.Add(2);

		Assert.AreEqual (2, list.Length);
		Assert.AreEqual (1, list[0]);
		Assert.AreEqual (2, list[1]);

		list.Dispose();
	}

	[Test]
	public void NativeArrayFromNativeList()
	{
		var list = new NativeList<int> (Allocator.Persistent);
		list.Add(42);
		list.Add(2);

		NativeArray<int> array = list;

		Assert.AreEqual (2, array.Length);
		Assert.AreEqual (42, array[0]);
		Assert.AreEqual (2, array[1]);

		list.Dispose();
	}

	[Test]
	public void NativeArrayFromNativeListInvalidatesOnAdd()
	{
		var list = new NativeList<int> (Allocator.Persistent);

		// This test checks that adding an element without reallocation invalidates the native array
		// (length changes)
		list.Capacity = 2;
		list.Add(42);

		NativeArray<int> array = list;

		list.Add(1000);

		Assert.AreEqual (2, list.Length);
		Assert.Throws<System.InvalidOperationException> (()=> { array[0] = 1; });

		list.Dispose();
	}

	[Test]
	public void NativeArrayFromNativeListInvalidatesOnCapacityChange()
	{
		var list = new NativeList<int> (Allocator.Persistent);
		list.Add(42);

		NativeArray<int> array = list;

		Assert.AreEqual (1, list.Length);
		list.Capacity = 10;

		Assert.AreEqual (1, array.Length);
		Assert.Throws<System.InvalidOperationException> (()=> { array[0] = 1; });

		list.Dispose();
	}

	[Test]
	public void NativeArrayFromNativeListInvalidatesOnDispose()
	{
		var list = new NativeList<int> (Allocator.Persistent);
		list.Add(42);
		NativeArray<int> array = list;
		list.Dispose();

		Assert.Throws<System.InvalidOperationException> (()=> { array[0] = 1; });
		Assert.Throws<System.InvalidOperationException> (()=> { list[0] = 1; });
	}

	[Test]
	public void NativeArrayFromNativeListmayNotDeallocate()
	{
		var list = new NativeList<int> (Allocator.Persistent);
		list.Add(42);

		NativeArray<int> array = list;
		Assert.Throws<System.InvalidOperationException> (()=> { array.Dispose(); });
		list.Dispose();
	}

	[Test]
	public void CopiedNativeListIsKeptInSync()
	{
		var list = new NativeList<int> (Allocator.Persistent);
		var listCpy = list;
		list.Add (42);

		Assert.AreEqual (42, listCpy[0]);
		Assert.AreEqual (42, list[0]);
		Assert.AreEqual (1, listCpy.Length);
		Assert.AreEqual (1, list.Length);

		list.Dispose();
	}

    [BurstCompile(CompileSynchronously = true)]
    struct TempListInJob : IJob
    {
        public NativeArray<int> Output;
        public void Execute()
        {
            var list = new NativeList<int>(Allocator.Temp);

            list.Add(17);

            Output[0] = list[0];

            list.Dispose();
        }
    }

    
    [Test]
    [Ignore("Not supported yet, requires a fix in DisposeSentinel")]
    public void TempListInBurstJob()
    {
        var job = new TempListInJob() { Output = new NativeArray<int>(1, Allocator.TempJob) };
        job.Schedule().Complete();
        Assert.AreEqual(17, job.Output[0]);
        
        job.Output.Dispose();
    }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
    [Test]
    public void SetCapacityLessThanLength()
    {
        var list = new NativeList<int> (Allocator.Persistent);
        list.Resize(10, NativeArrayOptions.UninitializedMemory);
        Assert.Throws<ArgumentException>(() => { list.Capacity = 5; });

        list.Dispose();
    }

    [Test]
	public void DisposingNativeListDerivedArrayThrows()
	{
		var list = new NativeList<int> (Allocator.Persistent);
		list.Add(1);

		NativeArray<int> array = list;
	    Assert.Throws<InvalidOperationException>(() => { array.Dispose(); });

		list.Dispose();
	}
#endif

    [Test]
    public void NativeList_DisposeJob()
    {
        var container = new NativeList<int>(Allocator.Persistent);
        Assert.True(container.IsCreated);
        Assert.DoesNotThrow(() => { container.Add(0); });
        Assert.DoesNotThrow(() => { container.Contains(0); });

        var disposeJob = container.Dispose(default);
        Assert.False(container.IsCreated);
        Assert.Throws<InvalidOperationException>(() => { container.Contains(0); });

        disposeJob.Complete();
    }
    
    [Test]
    public void ForEachWorks()
    {
        var container = new NativeList<int>(Allocator.Persistent);
        container.Add(10);
        container.Add(20);

        int sum = 0;
        int count = 0;
        GCAllocRecorder.ValidateNoGCAllocs(() =>
        {
            sum = 0;
            count = 0;
            foreach (var p in container)
            {
                sum += p;
                count++;
            }
        });
        
        Assert.AreEqual(30, sum);
        Assert.AreEqual(2, count);

        container.Dispose();
    }
}
