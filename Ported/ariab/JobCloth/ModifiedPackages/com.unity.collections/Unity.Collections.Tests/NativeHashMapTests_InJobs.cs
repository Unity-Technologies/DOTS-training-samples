using NUnit.Framework;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;

public class NativeHashMapTests_InJobs : NativeHashMapTestsFixture
{
    [Test]
    public void Read_And_Write()
    {
        var hashMap = new NativeHashMap<int, int>(hashMapSize, Allocator.TempJob);
        var writeStatus = new NativeArray<int>(hashMapSize, Allocator.TempJob);
        var readValues = new NativeArray<int>(hashMapSize, Allocator.TempJob);

        var writeData = new HashMapWriteJob();
        writeData.hashMap = hashMap.ToConcurrent();
        writeData.status = writeStatus;
        writeData.keyMod = hashMapSize;

        var readData = new HashMapReadParallelForJob();
        readData.hashMap = hashMap;
        readData.values = readValues;
        readData.keyMod = writeData.keyMod;
        var writeJob = writeData.Schedule();
        var readJob = readData.Schedule(hashMapSize, 1, writeJob);
        readJob.Complete();

        for (int i = 0; i < hashMapSize; ++i)
        {
            Assert.AreEqual(0, writeStatus[i], "Job failed to write value to hash map");
            Assert.AreEqual(i, readValues[i], "Job failed to read from hash map");
        }

        hashMap.Dispose();
        writeStatus.Dispose();
        readValues.Dispose();
    }

    [Test]
    public void Read_And_Write_Full()
    {
        var hashMap = new NativeHashMap<int, int>(hashMapSize/2, Allocator.TempJob);
        var writeStatus = new NativeArray<int>(hashMapSize, Allocator.TempJob);
        var readValues = new NativeArray<int>(hashMapSize, Allocator.TempJob);

        var writeData = new HashMapWriteJob();
        writeData.hashMap = hashMap.ToConcurrent();
        writeData.status = writeStatus;
        writeData.keyMod = hashMapSize;
        var readData = new HashMapReadParallelForJob();
        readData.hashMap = hashMap;
        readData.values = readValues;
        readData.keyMod = writeData.keyMod;
        var writeJob = writeData.Schedule();
        var readJob = readData.Schedule(hashMapSize, 1, writeJob);
        readJob.Complete();

        var missing = new HashSet<int>();
        for (int i = 0; i < hashMapSize; ++i)
        {
            if (writeStatus[i] == -2)
            {
                missing.Add(i);
                Assert.AreEqual(-1, readValues[i], "Job read a value form hash map which should not be there");
            }
            else
            {
                Assert.AreEqual(0, writeStatus[i], "Job failed to write value to hash map");
                Assert.AreEqual(i, readValues[i], "Job failed to read from hash map");
            }
        }
        Assert.AreEqual(hashMapSize - hashMapSize/2, missing.Count, "Wrong indices written to hash map");

        hashMap.Dispose();
        writeStatus.Dispose();
        readValues.Dispose();
    }

    [Test]
    public void Key_Collisions()
    {
        var hashMap = new NativeHashMap<int, int>(hashMapSize, Allocator.TempJob);
        var writeStatus = new NativeArray<int>(hashMapSize, Allocator.TempJob);
        var readValues = new NativeArray<int>(hashMapSize, Allocator.TempJob);

        var writeData = new HashMapWriteJob();
        writeData.hashMap = hashMap.ToConcurrent();
        writeData.status = writeStatus;
        writeData.keyMod = 16;
        var readData = new HashMapReadParallelForJob();
        readData.hashMap = hashMap;
        readData.values = readValues;
        readData.keyMod = writeData.keyMod;
        var writeJob = writeData.Schedule();
        var readJob = readData.Schedule(hashMapSize, 1, writeJob);
        readJob.Complete();

        var missing = new HashSet<int>();
        for (int i = 0; i < hashMapSize; ++i)
        {
            if (writeStatus[i] == -1)
            {
                missing.Add(i);
                Assert.AreNotEqual(i, readValues[i], "Job read a value form hash map which should not be there");
            }
            else
            {
                Assert.AreEqual(0, writeStatus[i], "Job failed to write value to hash map");
                Assert.AreEqual(i, readValues[i], "Job failed to read from hash map");
            }
        }
        Assert.AreEqual(hashMapSize - writeData.keyMod, missing.Count, "Wrong indices written to hash map");

        hashMap.Dispose();
        writeStatus.Dispose();
        readValues.Dispose();
    }

    struct Clear : IJob
    {
        public NativeHashMap<int, int> hashMap;

        public void Execute()
        {
            hashMap.Clear();
        }
    }

    [Test]
    public void Clear_And_Write()
    {
        var hashMap = new NativeHashMap<int, int>(hashMapSize/2, Allocator.TempJob);
        var writeStatus = new NativeArray<int>(hashMapSize, Allocator.TempJob);

        var clearJob = new Clear
        {
            hashMap = hashMap
        };

        var clearJobHandle = clearJob.Schedule();

        var writeJob = new HashMapWriteJob
        {
            hashMap = hashMap.ToConcurrent(),
            status = writeStatus,
            keyMod = hashMapSize,
        };

        var writeJobHandle = writeJob.Schedule(clearJobHandle);
        writeJobHandle.Complete();

        writeStatus.Dispose();
        hashMap.Dispose();
    }

    struct MergeSharedValues : IJobNativeMultiHashMapMergedSharedKeyIndices
    {
        [NativeDisableParallelForRestriction] public NativeArray<int> sharedCount;
        [NativeDisableParallelForRestriction] public NativeArray<int> sharedIndices;

        public void ExecuteFirst(int index)
        {
            sharedIndices[index] = index;
        }

        public void ExecuteNext(int firstIndex, int index)
        {
            sharedIndices[index] = firstIndex;
            sharedCount[firstIndex]++;
        }
    }

    [Test]
    public void NativeHashMapMergeCountShared()
    {
        var count = 1024;
        var sharedKeyCount = 16;
        var sharedCount = new NativeArray<int>(count,Allocator.TempJob);
        var sharedIndices = new NativeArray<int>(count,Allocator.TempJob);
        var totalSharedCount = new NativeArray<int>(1,Allocator.TempJob);
        var hashMap = new NativeMultiHashMap<int,int>(count,Allocator.TempJob);

        for (int i = 0; i < count; i++)
        {
            hashMap.Add(i&(sharedKeyCount-1),i);
            sharedCount[i] = 1;
        }

        var mergeSharedValuesJob = new MergeSharedValues
        {
            sharedCount = sharedCount,
            sharedIndices = sharedIndices,
        };
        var mergetedSharedValuesJobHandle = mergeSharedValuesJob.Schedule(hashMap, 64);
        mergetedSharedValuesJobHandle.Complete();

        for (int i = 0; i < count; i++)
        {
            Assert.AreEqual(count/sharedKeyCount,sharedCount[sharedIndices[i]]);
        }

        sharedCount.Dispose();
        sharedIndices.Dispose();
        totalSharedCount.Dispose();
        hashMap.Dispose();
    }

}
