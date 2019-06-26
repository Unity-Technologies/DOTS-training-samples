using System;
using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class UnsafeAppendOnlyBufferTests
{
    struct TestHeader
    {
        public int Type;
        public int PayloadSize;
    }
        
    [Test]
    public void DisposeEmpty()
    {
        var buffer = new UnsafeAppendOnlyBuffer(0, 8, Allocator.Temp);
        buffer.Dispose();
    }
    
    [Test]
    public void ThrowZeroAlignment()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var buffer = new UnsafeAppendOnlyBuffer(0, 0, Allocator.Temp);
        });
    }
    
    [Test]
    public unsafe void PushHeadersWithPackets()
    {
        var buffer = new UnsafeAppendOnlyBuffer(0, 8, Allocator.Temp);
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
        Assert.True(expectedSize <= buffer.Size);

        buffer.Dispose();
    }
    
    [Test]
    public unsafe void ReadHeadersWithPackets()
    {
        var buffer = new UnsafeAppendOnlyBuffer(0, 8, Allocator.Temp);
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
            
            UnsafeUtilityEx.MemSet(scratchPayload,(byte)(i & 0xff), packetSize);
            
            buffer.Add(scratchPayload, i);
        }

        var reader = new UnsafeAppendOnlyBufferReader(&buffer);
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
}

