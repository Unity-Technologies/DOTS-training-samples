using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Collections;
using Unity.PerformanceTesting;

public class MathTests
{
    [Test]
    public void Tests()
    {
        Assert.AreEqual(0, CollectionHelper.Log2Floor(1));
        Assert.AreEqual(1, CollectionHelper.Log2Floor(2));
        Assert.AreEqual(1, CollectionHelper.Log2Floor(3));
        Assert.AreEqual(2, CollectionHelper.Log2Floor(4));
        
        Assert.AreEqual(3, CollectionHelper.Log2Floor(15));
        Assert.AreEqual(4, CollectionHelper.Log2Floor(16));
        Assert.AreEqual(4, CollectionHelper.Log2Floor(19));

        Assert.AreEqual(30, CollectionHelper.Log2Floor(int.MaxValue));
        Assert.AreEqual(16, CollectionHelper.Log2Floor(1 << 16));
        
        Assert.AreEqual(-1, CollectionHelper.Log2Floor(0));
    }
}


public class NativeArraySortTests
{
    [Test]
    public void SortNativeArray_RandomInts_ReturnSorted([Values(0, 1, 10, 1000, 10000)] int size)
    {
        var random = new System.Random();
        NativeArray<int> array = new NativeArray<int>(size, Allocator.Persistent);
        Assert.IsTrue(array.IsCreated);

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = random.Next(int.MinValue, int.MaxValue);
        }

        array.Sort();

        int min = int.MinValue;
        foreach (var i in array)
        {
            Assert.LessOrEqual(min, i);
            min = i;
        }
        array.Dispose();
    }

    [Test]
    public void SortNativeArray_SortedInts_ReturnSorted([Values(0, 1, 10, 1000, 10000)] int size)
    {
        NativeArray<int> array = new NativeArray<int>(size, Allocator.Persistent);
        Assert.IsTrue(array.IsCreated);

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = i;
        }

        array.Sort();

        int min = int.MinValue;
        foreach (var i in array)
        {
            Assert.LessOrEqual(min, i);
            min = i;
        }
        array.Dispose();
    }

    [Test]
    public void SortNativeArray_RandomBytes_ReturnSorted([Values(0, 1, 10, 1000, 10000, 100000)] int size)
    {
        var random = new System.Random();
        NativeArray<byte> array = new NativeArray<byte>(size, Allocator.Persistent);
        Assert.IsTrue(array.IsCreated);

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = (byte)random.Next(byte.MinValue, byte.MinValue);
        }

        array.Sort();

        int min = int.MinValue;
        foreach (var i in array)
        {
            Assert.LessOrEqual(min, i);
            min = i;
        }
        array.Dispose();
    }

    [Test]
    public void SortNativeArray_RandomShorts_ReturnSorted([Values(0, 1, 10, 1000, 10000)] int size)
    {
        var random = new System.Random();
        NativeArray<short> array = new NativeArray<short>(size, Allocator.Persistent);
        Assert.IsTrue(array.IsCreated);

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = (short)random.Next(short.MinValue, short.MaxValue);
        }

        array.Sort();

        int min = int.MinValue;
        foreach (var i in array)
        {
            Assert.LessOrEqual(min, i);
            min = i;
        }
        array.Dispose();
    }

    [Test]
    public void SortNativeArray_RandomFloats_ReturnSorted([Values(0, 1, 10, 1000, 10000)] int size)
    {
        var random = new System.Random();
        NativeArray<float> array = new NativeArray<float>(size, Allocator.Persistent);
        Assert.IsTrue(array.IsCreated);

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = (float)random.NextDouble();
        }

        array.Sort();

        float min = float.MinValue;
        foreach (var i in array)
        {
            Assert.LessOrEqual(min, i);
            min = i;
        }
        array.Dispose();
    }

    struct ComparableType : IComparable<ComparableType>
    {
        public int value;
        public int CompareTo(ComparableType other) => value.CompareTo(other.value);
    }

    [Test]
    public void SortNativeArray_RandomComparableType_ReturnSorted([Values(0, 1, 10, 1000, 10000)] int size)
    {
        var random = new System.Random();
        NativeArray<ComparableType> array = new NativeArray<ComparableType>(size, Allocator.Persistent);
        Assert.IsTrue(array.IsCreated);

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = new ComparableType
            {
                value = random.Next(int.MinValue, int.MaxValue)
            };
        }

        array.Sort();

        int min = int.MinValue;
        foreach (var i in array)
        {
            Assert.LessOrEqual(min, i.value);
            min = i.value;
        }
        array.Dispose();
    }

    struct NonComparableType
    {
        public int value;
    }

    struct NonComparableTypeComparator : IComparer<NonComparableType>
    {
        public int Compare(NonComparableType lhs, NonComparableType rhs)
        {
            return lhs.value.CompareTo(rhs.value);
        }
    }

    [Test]
    public void SortNativeArray_RandomNonComparableType_ReturnSorted([Values(0, 1, 10, 1000, 10000)] int size)
    {
        var random = new System.Random();
        NativeArray<NonComparableType> array = new NativeArray<NonComparableType>(size, Allocator.Persistent);
        Assert.IsTrue(array.IsCreated);

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = new NonComparableType
            {
                value = random.Next(int.MinValue, int.MaxValue)
            };
        }

        array.Sort(new NonComparableTypeComparator());

        int min = int.MinValue;
        foreach (var i in array)
        {
            Assert.LessOrEqual(min, i.value);
            min = i.value;
        }
        array.Dispose();
    }

    [Test]
    public void SortNativeSlice_ReturnSorted()
    {
        var random = new System.Random();
        NativeArray<int> array = new NativeArray<int>(1000, Allocator.Persistent);
        Assert.IsTrue(array.IsCreated);

        for (int i = 0; i < array.Length; ++i)
        {
            array[i] = random.Next(int.MinValue, int.MaxValue);
        }

        var slice = new NativeSlice<int>(array, 200, 600);

        slice.Sort();

        int min = int.MinValue;
        foreach (var i in slice)
        {
            Assert.LessOrEqual(min, i);
            min = i;
        }

        array.Dispose();
    }

    [Test]
    public void SortNativeSlice_DoesNotChangeArrayBeyondLimits()
    {
        var random = new System.Random();
        NativeArray<int> array = new NativeArray<int>(1000, Allocator.Persistent);
        Assert.IsTrue(array.IsCreated);

        for (int i = 0; i < array.Length; ++i)
        {
            array[i] = random.Next(int.MinValue, int.MaxValue);
        }
        var backupArray = new NativeArray<int>(array.Length, Allocator.Persistent);
        backupArray.CopyFrom(array);

        var slice = new NativeSlice<int>(array, 200, 600);

        slice.Sort();

        for (var i = 0; i < 200; ++i)
        {
            Assert.AreEqual(backupArray[i], array[i]);
        }

        for (var i = 800; i < 1000; ++i)
        {
            Assert.AreEqual(backupArray[i], array[i]);
        }

        array.Dispose();
        backupArray.Dispose();
    }

    [Test]
    public void SortNativeSlice_WithCustomStride_ThrowsInvalidOperationException()
    {
        var random = new System.Random();
        NativeArray<int> array = new NativeArray<int>(10, Allocator.Persistent);
        for (int i = 0; i < array.Length; ++i)
        {
            array[i] = random.Next(int.MinValue, int.MaxValue);
        }

        var slice = new NativeSlice<int>(array, 2, 6);
        var sliceWithCustomStride = slice.SliceWithStride<short>();

        Assert.DoesNotThrow(() => slice.Sort());
        Assert.Throws<InvalidOperationException>(() => sliceWithCustomStride.Sort());

        array.Dispose();
    }
}


public class NativeSliceTests
{
    [Test]
    public void NativeSlice_CopyTo()
    {
        NativeArray<int> array = new NativeArray<int>(1000, Allocator.Persistent);

        for (int i = 0; i < array.Length; ++i)
        {
            array[i] = i;
        }

        var copyToArray = new int[600];

        for (int i = 0; i < copyToArray.Length; ++i)
        {
            copyToArray[i] = 0x12345678;
        }

        var slice = new NativeSlice<int>(array, 200, 600);
        slice.CopyTo(copyToArray);

        for (var i = 0; i < 600; ++i)
        {
            Assert.AreEqual(copyToArray[i], array[i + 200]);
        }

        array.Dispose();
    }

    [Test, Performance]
    [Category("Performance")]
    public void NativeSlice_Performance_CopyTo()
    {
        const int numElements = 16 << 10;

        NativeArray<int> array = new NativeArray<int>(numElements, Allocator.Persistent);
        var slice = new NativeSlice<int>(array, 0, numElements);

        var copyToArray = new int[numElements];

        Measure.Method(() =>
            {
                slice.CopyTo(copyToArray);
            })
            .WarmupCount(100)
            .MeasurementCount(1000)
            .Run();

        array.Dispose();
    }

    [Test]
    public void NativeSlice_CopyFrom()
    {
        NativeArray<int> array = new NativeArray<int>(1000, Allocator.Persistent);

        for (int i = 0; i < array.Length; ++i)
        {
            array[i] = i;
        }

        var copyFromArray = new int[600];

        for (int i = 0; i < copyFromArray.Length; ++i)
        {
            copyFromArray[i] = 0x12345678;
        }

        var slice = new NativeSlice<int>(array, 200, 600);
        slice.CopyFrom(copyFromArray);

        for (var i = 0; i < 600; ++i)
        {
            Assert.AreEqual(slice[i], 0x12345678);
        }

        array.Dispose();
    }

    [Test, Performance]
    [Category("Performance")]
    public void NativeSlice_Performance_CopyFrom()
    {
        const int numElements = 16 << 10;

        NativeArray<int> array = new NativeArray<int>(numElements, Allocator.Persistent);
        var slice = new NativeSlice<int>(array, 0, numElements);

        var copyToArray = new int[numElements];

        Measure.Method(() =>
            {
                slice.CopyFrom(copyToArray);
            })
            .WarmupCount(100)
            .MeasurementCount(1000)
            .Run();

        array.Dispose();
    }
    
    [Test]
    public void SortJobNativeArray_RandomInts_ReturnSorted([Values(0, 1, 10, 1000, 10000)] int size)
    {
        var random = new System.Random();
        NativeArray<int> array = new NativeArray<int>(size, Allocator.Persistent);
        Assert.IsTrue(array.IsCreated);

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = random.Next(int.MinValue, int.MaxValue);
        }

        array.SortJob().Complete();

        int min = int.MinValue;
        foreach (var i in array)
        {
            Assert.LessOrEqual(min, i);
            min = i;
        }

        array.Dispose();
    }
    
    [Test]
    public void SortJbativeArray_SortedInts_ReturnSorted([Values(0, 1, 10, 1000, 10000)] int size)
    {
        NativeArray<int> array = new NativeArray<int>(size, Allocator.Persistent);
        Assert.IsTrue(array.IsCreated);

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = i;
        }

        array.SortJob().Complete();

        int min = int.MinValue;
        foreach (var i in array)
        {
            Assert.LessOrEqual(min, i);
            min = i;
        }
        array.Dispose();
    }

    [Test]
    public void SortJobNativeArray_RandomBytes_ReturnSorted([Values(0, 1, 10, 1000, 10000, 100000)] int size)
    {
        var random = new System.Random();
        NativeArray<byte> array = new NativeArray<byte>(size, Allocator.Persistent);
        Assert.IsTrue(array.IsCreated);

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = (byte)random.Next(byte.MinValue, byte.MinValue);
        }

        array.SortJob().Complete();

        int min = int.MinValue;
        foreach (var i in array)
        {
            Assert.LessOrEqual(min, i);
            min = i;
        }
        array.Dispose();
    }

    [Test]
    public void SortJobNativeArray_RandomShorts_ReturnSorted([Values(0, 1, 10, 1000, 10000)] int size)
    {
        var random = new System.Random();
        NativeArray<short> array = new NativeArray<short>(size, Allocator.Persistent);
        Assert.IsTrue(array.IsCreated);

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = (short)random.Next(short.MinValue, short.MaxValue);
        }

        array.SortJob().Complete();

        int min = int.MinValue;
        foreach (var i in array)
        {
            Assert.LessOrEqual(min, i);
            min = i;
        }
        array.Dispose();
    }

    [Test]
    public void SortJobNativeArray_RandomFloats_ReturnSorted([Values(0, 1, 10, 1000, 10000)] int size)
    {
        var random = new System.Random();
        NativeArray<float> array = new NativeArray<float>(size, Allocator.Persistent);
        Assert.IsTrue(array.IsCreated);

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = (float)random.NextDouble();
        }

        array.SortJob().Complete();

        float min = float.MinValue;
        foreach (var i in array)
        {
            Assert.LessOrEqual(min, i);
            min = i;
        }
        array.Dispose();
    }

}
