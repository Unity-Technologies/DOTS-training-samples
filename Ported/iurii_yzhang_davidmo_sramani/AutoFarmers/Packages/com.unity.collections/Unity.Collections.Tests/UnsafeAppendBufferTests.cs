using System;
using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class UnsafeAppendBufferTests
{
    struct TestHeader
    {
        public int Type;
        public int PayloadSize;
    }
        
    [Test]
    public void DisposeEmpty()
    {
        var buffer = new UnsafeAppendBuffer(0, 8, Allocator.Temp);
        buffer.Dispose();
    }

    [Test]
    unsafe public void DisposeExternal()
    {
        var data = stackalloc int[1];
        var buffer = new UnsafeAppendBuffer(data, sizeof(int));
        buffer.Add(5);
        buffer.Dispose();
        Assert.AreEqual(5, data[0]);
    }

    
    [Test]
    public void ThrowZeroAlignment()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var buffer = new UnsafeAppendBuffer(0, 0, Allocator.Temp);
        });
    }
    
    [Test]
    public unsafe void PushHeadersWithPackets()
    {
        var buffer = new UnsafeAppendBuffer(0, 8, Allocator.Temp);
        var scratchPayload = stackalloc byte[1024];
        var expectedSize = 0;
        for (int i = 0; i < 1024; i++)
        {
            var packeType = i;
            var packetSize = i;
            
            buffer.Add(new TestHeader
            {
                Type = packeType,
                PayloadSize = packetSize
            });
            expectedSize += UnsafeUtility.SizeOf<TestHeader>();
            
            buffer.Add(scratchPayload, i);
            expectedSize += i;
        }
        Assert.True(expectedSize == buffer.Size);

        buffer.Dispose();
    }
    
    [Test]
    public unsafe void ReadHeadersWithPackets()
    {
        var buffer = new UnsafeAppendBuffer(0, 8, Allocator.Temp);
        var scratchPayload = stackalloc byte[1024];
        for (int i = 0; i < 1024; i++)
        {
            var packeType = i;
            var packetSize = i;
            
            buffer.Add(new TestHeader
            {
                Type = packeType,
                PayloadSize = packetSize
            });
            
            UnsafeUtility.MemSet(scratchPayload,(byte)(i & 0xff), packetSize);
            
            buffer.Add(scratchPayload, i);
        }

        var reader = buffer.AsReader();
        for (int i = 0; i < 1024; i++)
        {
            var packetHeader = reader.ReadNext<TestHeader>();
            Assert.AreEqual(i, packetHeader.Type);
            Assert.AreEqual(i, packetHeader.PayloadSize);
            if (packetHeader.PayloadSize > 0)
            {
                var packetPayload = reader.ReadNext(packetHeader.PayloadSize);
                Assert.AreEqual( (byte)(i&0xff), *(byte*)packetPayload);
            }
        }
        Assert.True(reader.EndOfBuffer);

        buffer.Dispose();
    }

    [Test]
    public unsafe void AddAndPop()
    {
        var buffer = new UnsafeAppendBuffer(0, 8, Allocator.Temp);

        buffer.Add<int>(123);
        buffer.Add<int>(234);
        buffer.Add<int>(345);

        {
            var array = new NativeArray<int>(3, Allocator.Temp);
            buffer.Pop(array.GetUnsafePtr(), 3 * UnsafeUtility.SizeOf<int>());
            CollectionAssert.AreEqual(new[] {123, 234, 345}, array);
        }

        {
            var array = new NativeArray<int>(4, Allocator.Temp);
            array.CopyFrom(new[] {987, 876, 765, 654});
            buffer.Add(array.GetUnsafePtr(), 4 * UnsafeUtility.SizeOf<int>());
        }

        Assert.AreEqual(654, buffer.Pop<int>());
        Assert.AreEqual(765, buffer.Pop<int>());
        Assert.AreEqual(876, buffer.Pop<int>());
        Assert.AreEqual(987, buffer.Pop<int>());
        
        buffer.Dispose();
    }
}
