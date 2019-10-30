using NUnit.Framework;
using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class UnsafeListTests
{
    [Test]
    public unsafe void UnsafeList_Init_ClearMemory()
    {
        UnsafeList list = new UnsafeList(UnsafeUtility.SizeOf<int>(), UnsafeUtility.AlignOf<int>(), 10, Allocator.Persistent, NativeArrayOptions.ClearMemory);

        for (var i = 0; i < list.Length; ++i)
        {
            Assert.AreEqual(0, UnsafeUtility.ReadArrayElement<int>(list.Ptr, i));
        }

        list.Dispose();
    }

    [Test]
	public unsafe void UnsafeList_Allocate_Deallocate_Read_Write()
	{
		var list = new UnsafeList(Allocator.Persistent);

		list.Add(1);
		list.Add(2);

		Assert.AreEqual(2, list.Length);
		Assert.AreEqual(1, UnsafeUtility.ReadArrayElement<int>(list.Ptr, 0));
		Assert.AreEqual(2, UnsafeUtility.ReadArrayElement<int>(list.Ptr, 1));

		list.Dispose();
	}

    [Test]
    public unsafe void UnsafeList_Resize_ClearMemory()
    {
        var sizeOf = UnsafeUtility.SizeOf<int>();
        var alignOf = UnsafeUtility.AlignOf<int>();

        UnsafeList list = new UnsafeList(sizeOf, alignOf, 5, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        list.Resize(sizeOf, alignOf, 5, NativeArrayOptions.UninitializedMemory);
        list.Resize(sizeOf, alignOf, 10, NativeArrayOptions.ClearMemory);

        for (var i = 0; i < list.Length; ++i)
        {
            Assert.AreEqual(0, UnsafeUtility.ReadArrayElement<int>(list.Ptr, i));
        }

        list.Dispose();
    }

    [Test]
    public unsafe void UnsafeList_DisposeJob()
    {
        var sizeOf = UnsafeUtility.SizeOf<int>();
        var alignOf = UnsafeUtility.AlignOf<int>();

        UnsafeList list = new UnsafeList(sizeOf, alignOf, 5, Allocator.Persistent, NativeArrayOptions.ClearMemory);

        var disposeJob = list.Dispose(default);

        Assert.IsTrue(list.Ptr == null);

        disposeJob.Complete();
    }

    unsafe void Expected(ref UnsafeList list, int expectedLength, int[] expected)
    {
        Assert.AreEqual(list.Length, expectedLength);
        for (var i = 0; i < list.Length; ++i)
        {
            var value = UnsafeUtility.ReadArrayElement<int>(list.Ptr, i);
            Assert.AreEqual(expected[i], value);
        }
    }

    [Test]
    public unsafe void UnsafeList_AddNoResize()
    {
        var sizeOf = UnsafeUtility.SizeOf<int>();
        var alignOf = UnsafeUtility.AlignOf<int>();

        UnsafeList list = new UnsafeList(sizeOf, alignOf, 1, Allocator.Persistent, NativeArrayOptions.ClearMemory);

        // List's capacity is always cache-line aligned, number of items fills up whole cache-line.
        int[] range = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        Assert.Throws<Exception>(() => { fixed (int* r = range) list.AddRangeNoResize<int>(r, 17); });

        list.SetCapacity<int>(17);
        Assert.DoesNotThrow(() => { fixed (int* r = range) list.AddRangeNoResize<int>(r, 17); });

        list.SetCapacity<int>(16);
        Assert.Throws<Exception>(() => { list.AddNoResize(16); });
    }

    [Test]
    public unsafe void UnsafeList_AddNoResize_Read()
    {
        var sizeOf = UnsafeUtility.SizeOf<int>();
        var alignOf = UnsafeUtility.AlignOf<int>();

        UnsafeList list = new UnsafeList(sizeOf, alignOf, 4, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        list.AddNoResize(4);
        list.AddNoResize(6);
        list.AddNoResize(4);
        list.AddNoResize(9);
        Expected(ref list, 4, new int[]{ 4, 6, 4, 9 });

        list.Dispose();
    }

    [Test]
    public unsafe void UnsafeList_RemoveRangeSwapBack()
    {
        var sizeOf = UnsafeUtility.SizeOf<int>();
        var alignOf = UnsafeUtility.AlignOf<int>();

        UnsafeList list = new UnsafeList(sizeOf, alignOf, 10, Allocator.Persistent, NativeArrayOptions.ClearMemory);

        int[] range = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        // test removing from the end
        fixed (int* r = range) list.AddRange<int>(r, 10);
        list.RemoveRangeSwapBack<int>(6, 9);
        Expected(ref list, 7, new int[]{ 0, 1, 2, 3, 4, 5, 9 });
        list.Clear();

        // test removing all but one
        fixed (int* r = range) list.AddRange<int>(r, 10);
        list.RemoveRangeSwapBack<int>(0, 9);
        Expected(ref list, 1, new int[] { 9 });
        list.Clear();

        // test removing from the front
        fixed (int* r = range) list.AddRange<int>(r, 10);
        list.RemoveRangeSwapBack<int>(0, 3);
        Expected(ref list, 7, new int[] { 7, 8, 9, 3, 4, 5, 6 });
        list.Clear();

        // test removing from the middle
        fixed (int* r = range) list.AddRange<int>(r, 10);
        list.RemoveRangeSwapBack<int>(0, 3);
        Expected(ref list, 7, new int[] { 7, 8, 9, 3, 4, 5, 6 });
        list.Clear();

        // test removing whole range
        fixed (int* r = range) list.AddRange<int>(r, 10);
        list.RemoveRangeSwapBack<int>(0, 10);
        Expected(ref list, 0, new int[] { 0 });
        list.Clear();

        list.Dispose();
    }

    [Test]
    public unsafe void UnsafeList_PtrLength()
    {
        var sizeOf = UnsafeUtility.SizeOf<int>();
        var alignOf = UnsafeUtility.AlignOf<int>();

        var list = new UnsafeList(sizeOf, alignOf, 10, Allocator.Persistent, NativeArrayOptions.ClearMemory);

        int[] range = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        fixed (int* r = range) list.AddRange<int>(r, 10);

        var listView = new UnsafeList((int*)list.Ptr + 4, 2);
        Expected(ref listView, 2, new int[] { 4, 5 });

        listView.Dispose();
        list.Dispose();
    }
}
