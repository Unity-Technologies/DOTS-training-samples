using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using Unity.Collections;

public class ResizableArrayTests
{
    [Test]
    public void TestResizableArray64Byte()
    {
        var array = new ResizableArray64Byte<int>();
        for(var i = 0; i < array.Capacity; ++i)
            array.Add(i);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        Assert.Throws<IndexOutOfRangeException>(() => array.Add(array.Capacity));
#endif
        for(var i = 0; i < array.Capacity; ++i)
            Assert.AreEqual(array[i], i);
    }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    struct LargeStruct { int i0, i1, i2, i3, i4, i5, i6; }

    [Test, SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public void CtorCalled_WithDataBeyondCapacity_Throws()
    {
        var (a, b, c) = (new LargeStruct(), new LargeStruct(), new LargeStruct());
        Assert.DoesNotThrow(() => new ResizableArray64Byte<LargeStruct>(a, b));
        Assert.Throws<IndexOutOfRangeException>(() => new ResizableArray64Byte<LargeStruct>(a, b, c));
    }
#endif

    [Test]
    public void GetHashCode_OnlyHashesLengthBytes()
    {
        var array = new ResizableArray64Byte<int>();
        var hashPre = array.GetHashCode();
        ++array.Length;
        var hashPost = array.GetHashCode();

        Assert.AreNotEqual(hashPre, hashPost);
    }

    [Test]
    public void ObjectEquals_AlwaysThrows()
    {
        var array0 = new ResizableArray64Byte<int>();
        var array1 = new ResizableArray64Byte<int>();

        Assert.Throws<InvalidOperationException>(() => array0.Equals(array1));
    }

    [Test]
    public void ArrayEquals_OnlyTestsLengthBytes()
    {
        var array0 = new ResizableArray64Byte<long>();
        array0.Add(20);
        array0.Add(30);
        --array0.Length;
        var array1 = new ResizableArray64Byte<long>();
        array1.Add(20);

        Assert.IsTrue(array0.Equals(ref array1));
    }

    [Test]
    public void Ctors_WriteDataToCorrectSlots()
    {
        {
            var array1 = new ResizableArray64Byte<int>(1);
            Assert.AreEqual(1, array1.Length);
            Assert.AreEqual(1, array1[0]);
        }

        {
            var array2 = new ResizableArray64Byte<int>(2, 3);
            Assert.AreEqual(2, array2.Length);
            Assert.AreEqual(2, array2[0]);
            Assert.AreEqual(3, array2[1]);
        }

        {
            var array3 = new ResizableArray64Byte<int>(4, 5, 6);
            Assert.AreEqual(3, array3.Length);
            Assert.AreEqual(4, array3[0]);
            Assert.AreEqual(5, array3[1]);
            Assert.AreEqual(6, array3[2]);
        }

        {
            var array4 = new ResizableArray64Byte<int>(7, 8, 9, 10);
            Assert.AreEqual(4, array4.Length);
            Assert.AreEqual(7, array4[0]);
            Assert.AreEqual(8, array4[1]);
            Assert.AreEqual(9, array4[2]);
            Assert.AreEqual(10, array4[3]);
        }

        {
            var array5 = new ResizableArray64Byte<int>(11, 12, 13, 14, 15);
            Assert.AreEqual(5, array5.Length);
            Assert.AreEqual(11, array5[0]);
            Assert.AreEqual(12, array5[1]);
            Assert.AreEqual(13, array5[2]);
            Assert.AreEqual(14, array5[3]);
            Assert.AreEqual(15, array5[4]);
        }
    }
}
