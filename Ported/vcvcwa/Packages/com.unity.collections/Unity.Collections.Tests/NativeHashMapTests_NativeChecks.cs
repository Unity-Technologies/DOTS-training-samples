using System;
using NUnit.Framework;
using Unity.Collections;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
public class NativeHashMapTests_NativeChecks
{
	[Test]
	public void Double_Deallocate_Throws()
	{
		var hashMap = new NativeMultiHashMap<int, int> (16, Allocator.TempJob);
		hashMap.Dispose ();
		Assert.Throws<InvalidOperationException> (() => { hashMap.Dispose (); });
	}
    [Test]
	public void NativeMultiHashMapRemoveKeyValueThrowsInvalidParam()
	{
		var hashMap = new NativeMultiHashMap<int, long> (1, Allocator.Temp);
        Assert.Throws<ArgumentException>(() => hashMap.Remove(5, 5));
	    hashMap.Dispose();
	}
}
#endif
