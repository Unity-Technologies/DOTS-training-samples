using System;
using NUnit.Framework;
using Unity.Collections;

public class NativeArrayTests
{
#if UNITY_2020_1_OR_NEWER
    [Test]
    public void NativeArray_DisposeJob()
    {
        var container = new NativeArray<int>(1, Allocator.Persistent);
        Assert.True(container.IsCreated);
        Assert.DoesNotThrow(() => { container[0] = 1; });

        var disposeJob = container.Dispose(default);
        Assert.False(container.IsCreated);
        Assert.Throws<InvalidOperationException>(() => { container[0] = 2; });

        disposeJob.Complete();
    }
#endif
}
