using System;
using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.Tests;

public class NativeHashMapTests
{
#pragma warning disable 0649 // always defaul value
    struct NonBlittableStruct : IEquatable<NonBlittableStruct>
    {
         object o;

         public bool Equals(NonBlittableStruct other)
         {
             return Equals(o, other.o);
         }

         public override bool Equals(object obj)
         {
             if (ReferenceEquals(null, obj)) return false;
             return obj is NonBlittableStruct other && Equals(other);
         }

         public override int GetHashCode()
         {
             return (o != null ? o.GetHashCode() : 0);
         }
    }
#pragma warning restore 0649

    [Test]
    public void Non_Blittable_Throws()
    {
#pragma warning disable 0219 // assigned but its value is never used
        Assert.Throws<System.ArgumentException> (() => { var hashMap = new NativeHashMap<NonBlittableStruct, int>(16, Allocator.Temp); });
        Assert.Throws<System.ArgumentException> (() => { var hashMap = new NativeHashMap<int, NonBlittableStruct>(16, Allocator.Temp); });
#pragma warning restore 0219
    }

    [Test]
    public void TryAdd_TryGetValue_Clear()
    {
        var hashMap = new NativeHashMap<int, int> (16, Allocator.Temp);
        int iSquared;
        // Make sure GetValue fails if hash map is empty
        Assert.IsFalse(hashMap.TryGetValue(0, out iSquared), "TryGetValue on empty hash map did not fail");
        // Make sure inserting values work
        for (int i = 0; i < 16; ++i)
            Assert.IsTrue(hashMap.TryAdd(i, i*i), "Failed to add value");
        // Make sure inserting duplicate keys fails
        for (int i = 0; i < 16; ++i)
            Assert.IsFalse(hashMap.TryAdd(i, i), "Adding duplicate keys did not fail");
        // Make sure reading the inserted values work
        for (int i = 0; i < 16; ++i)
        {
            Assert.IsTrue(hashMap.TryGetValue(i, out iSquared), "Failed get value from hash table");
            Assert.AreEqual(iSquared, i*i, "Got the wrong value from the hash table");
        }
        // Make sure clearing removes all keys
        hashMap.Clear();
        for (int i = 0; i < 16; ++i)
            Assert.IsFalse(hashMap.TryGetValue(i, out iSquared), "Got value from hash table after clearing");
        hashMap.Dispose ();
    }

    [Test]
    public void Full_HashMap_Throws()
    {
        var hashMap = new NativeHashMap<int, int> (16, Allocator.Temp);
        // Fill the hash map
        for (int i = 0; i < 16; ++i)
            Assert.IsTrue(hashMap.TryAdd(i, i), "Failed to add value");
        // Make sure overallocating throws and exception if using the Concurrent version - normal hash map would grow
        var cHashMap = hashMap.AsParallelWriter();
        Assert.Throws<System.InvalidOperationException> (()=> {cHashMap.TryAdd(100, 100); });
        hashMap.Dispose ();
    }

    [Test]
    public void Double_Deallocate_Throws()
    {
        var hashMap = new NativeHashMap<int, int> (16, Allocator.TempJob);
        hashMap.Dispose ();
        Assert.Throws<System.InvalidOperationException> (() => { hashMap.Dispose (); });
    }

    [Test]
    public void Key_Collisions()
    {
        var hashMap = new NativeHashMap<int, int> (16, Allocator.Temp);
        int iSquared;
        // Make sure GetValue fails if hash map is empty
        Assert.IsFalse(hashMap.TryGetValue(0, out iSquared), "TryGetValue on empty hash map did not fail");
        // Make sure inserting values work
        for (int i = 0; i < 8; ++i)
            Assert.IsTrue(hashMap.TryAdd(i, i*i), "Failed to add value");
        // The bucket size is capacity * 2, adding that number should result in hash collisions
        for (int i = 0; i < 8; ++i)
            Assert.IsTrue(hashMap.TryAdd(i + 32, i), "Failed to add value with potential hash collision");
        // Make sure reading the inserted values work
        for (int i = 0; i < 8; ++i)
        {
            Assert.IsTrue(hashMap.TryGetValue(i, out iSquared), "Failed get value from hash table");
            Assert.AreEqual(iSquared, i*i, "Got the wrong value from the hash table");
        }
        for (int i = 0; i < 8; ++i)
        {
            Assert.IsTrue(hashMap.TryGetValue(i+32, out iSquared), "Failed get value from hash table");
            Assert.AreEqual(iSquared, i, "Got the wrong value from the hash table");
        }
        hashMap.Dispose ();
    }
    
    [Test]
    public void HashMapSupportsAutomaticCapacityChange()
    {
        var hashMap = new NativeHashMap<int, int> (1, Allocator.Temp);
        int iSquared;
        // Make sure inserting values work and grows the capacity
        for (int i = 0; i < 8; ++i)
            Assert.IsTrue(hashMap.TryAdd(i, i*i), "Failed to add value");
        Assert.IsTrue(hashMap.Capacity >= 8, "Capacity was not updated correctly");
        // Make sure reading the inserted values work
        for (int i = 0; i < 8; ++i)
        {
            Assert.IsTrue(hashMap.TryGetValue(i, out iSquared), "Failed get value from hash table");
            Assert.AreEqual(iSquared, i*i, "Got the wrong value from the hash table");
        }
        hashMap.Dispose ();
    }

    [Test]
    public void HashMapSameKey()
    {
        using (var hashMap = new NativeHashMap<int, int>(0, Allocator.Persistent))
        {
            Assert.DoesNotThrow(() => hashMap.Add(0, 0));
            Assert.Throws<ArgumentException>(() => hashMap.Add(0, 0));
        }
        
        using (var hashMap = new NativeHashMap<int, int>(0, Allocator.Persistent))
        {
            Assert.IsTrue(hashMap.TryAdd(0, 0));
            Assert.IsFalse(hashMap.TryAdd(0, 0));
        }
    }

    [Test]
    public void HashMapEmptyCapacity()
    {
        var hashMap = new NativeHashMap<int, int> (0, Allocator.Persistent);
        hashMap.TryAdd (0, 0);
        Assert.AreEqual (1, hashMap.Capacity);
        Assert.AreEqual (1, hashMap.Length);
        hashMap.Dispose ();
    }

    [Test]
    public void Remove()
    {
        var hashMap = new NativeHashMap<int, int> (8, Allocator.Temp);
        int iSquared;
        // Make sure inserting values work
        for (int i = 0; i < 8; ++i)
            Assert.IsTrue(hashMap.TryAdd(i, i*i), "Failed to add value");
        Assert.AreEqual(8, hashMap.Capacity, "HashMap grew larger than expected");
        // Make sure reading the inserted values work
        for (int i = 0; i < 8; ++i)
        {
            Assert.IsTrue(hashMap.TryGetValue(i, out iSquared), "Failed get value from hash table");
            Assert.AreEqual(iSquared, i*i, "Got the wrong value from the hash table");
        }
        for (int rm = 0; rm < 8; ++rm)
        {
            Assert.IsTrue(hashMap.Remove(rm));
            Assert.IsFalse(hashMap.TryGetValue(rm, out iSquared), "Failed to remove value from hash table");
            for (int i = rm+1; i < 8; ++i)
            {
                Assert.IsTrue(hashMap.TryGetValue(i, out iSquared), "Failed get value from hash table");
                Assert.AreEqual(iSquared, i*i, "Got the wrong value from the hash table");
            }
        }
        // Make sure entries were freed
        for (int i = 0; i < 8; ++i)
            Assert.IsTrue(hashMap.TryAdd(i, i*i), "Failed to add value");
        Assert.AreEqual(8, hashMap.Capacity, "HashMap grew larger than expected");
        hashMap.Dispose ();
    }
    [Test]
    public void RemoveFromMultiHashMap()
    {
        var hashMap = new NativeMultiHashMap<int, int> (16, Allocator.Temp);
        int iSquared;
        // Make sure inserting values work
        for (int i = 0; i < 8; ++i)
            hashMap.Add(i, i*i);
        for (int i = 0; i < 8; ++i)
            hashMap.Add(i, i);
        Assert.AreEqual(16, hashMap.Capacity, "HashMap grew larger than expected");
        // Make sure reading the inserted values work
        for (int i = 0; i < 8; ++i)
        {
            NativeMultiHashMapIterator<int> it;
            Assert.IsTrue(hashMap.TryGetFirstValue(i, out iSquared, out it), "Failed get value from hash table");
            Assert.AreEqual(iSquared, i, "Got the wrong value from the hash table");
            Assert.IsTrue(hashMap.TryGetNextValue(out iSquared, ref it), "Failed get value from hash table");
            Assert.AreEqual(iSquared, i*i, "Got the wrong value from the hash table");
        }
        for (int rm = 0; rm < 8; ++rm)
        {
            Assert.AreEqual(2, hashMap.Remove(rm));
            NativeMultiHashMapIterator<int> it;
            Assert.IsFalse(hashMap.TryGetFirstValue(rm, out iSquared, out it), "Failed to remove value from hash table");
            for (int i = rm+1; i < 8; ++i)
            {
                Assert.IsTrue(hashMap.TryGetFirstValue(i, out iSquared, out it), "Failed get value from hash table");
                Assert.AreEqual(iSquared, i, "Got the wrong value from the hash table");
                Assert.IsTrue(hashMap.TryGetNextValue(out iSquared, ref it), "Failed get value from hash table");
                Assert.AreEqual(iSquared, i*i, "Got the wrong value from the hash table");
            }
        }
        // Make sure entries were freed
        for (int i = 0; i < 8; ++i)
            hashMap.Add(i, i*i);
        for (int i = 0; i < 8; ++i)
            hashMap.Add(i, i);
        Assert.AreEqual(16, hashMap.Capacity, "HashMap grew larger than expected");
        hashMap.Dispose ();
    }

    void ExpectValues(NativeMultiHashMap<int, long> hashMap, int key, long[] expectedValues)
    {
        var list = new NativeList<long>(Allocator.TempJob);
        foreach (var value in hashMap.GetValuesForKey(key))
            list.Add(value);

        list.Sort();
        Assert.AreEqual(list.ToArray(), expectedValues);
        list.Dispose();
    }


    [Test]
    public void TryAddScalability()
    {
        var hashMap = new NativeHashMap<int, int> (1, Allocator.Persistent);
        for (int i = 0; i != 1000 * 100; i++)
        {
            hashMap.TryAdd (i, i);
        }

        int value;
        Assert.IsFalse(hashMap.TryGetValue (-1, out value));
        Assert.IsFalse(hashMap.TryGetValue (1000 * 1000, out value));

        for (int i = 0; i != 1000 * 100; i++)
        {
            Assert.IsTrue (hashMap.TryGetValue (i, out value));
            Assert.AreEqual (i, value);
        }

        hashMap.Dispose ();
    }

    [Test]
    public void GetKeysEmpty()
    {
        var hashMap = new NativeHashMap<int, int> (1, Allocator.Temp);
        var keys = hashMap.GetKeyArray(Allocator.Temp);
        hashMap.Dispose();

        Assert.AreEqual(0, keys.Length);
        keys.Dispose ();
    }

    [Test]
    public void NativeHashMapGetKeys()
    {
        var hashMap = new NativeHashMap<int, int> (1, Allocator.Temp);
        for (int i = 0; i < 30; ++i)
        {
            hashMap.TryAdd(i, 2 * i);
        }
        var keys = hashMap.GetKeyArray(Allocator.Temp);
        hashMap.Dispose();

        Assert.AreEqual(30, keys.Length);
        keys.Sort();
        for (int i = 0; i < 30; ++i)
        {
            Assert.AreEqual(i, keys[i]);
        }
        keys.Dispose ();
    }

    [Test]
    public void NativeHashMapGetValues()
    {
        var hashMap = new NativeHashMap<int, int> (1, Allocator.Temp);
        for (int i = 0; i < 30; ++i)
        {
            hashMap.TryAdd(i, 2 * i);
        }
        var values = hashMap.GetValueArray(Allocator.Temp);
        hashMap.Dispose();

        Assert.AreEqual(30, values.Length);
        values.Sort();
        for (int i = 0; i < 30; ++i)
        {
            Assert.AreEqual(2 * i, values[i]);
        }
        values.Dispose();
    }

    [Test]
    public void NativeHashMapGetKeysAndValues()
    {
        var hashMap = new NativeHashMap<int, int> (1, Allocator.Temp);
        for (int i = 0; i < 30; ++i)
        {
            hashMap.TryAdd(i, 2 * i);
        }
        var keysValues = hashMap.GetKeyValueArrays(Allocator.Temp);
        hashMap.Dispose();

        Assert.AreEqual(30, keysValues.Keys.Length);
        Assert.AreEqual(30, keysValues.Values.Length);

        // ensure keys and matching values are aligned
        for (int i = 0; i < 30; ++i)
        {
            Assert.AreEqual(2 * keysValues.Keys[i], keysValues.Values[i]);
        }

        keysValues.Keys.Sort();
        for (int i = 0; i < 30; ++i)
        {
            Assert.AreEqual(i, keysValues.Keys[i]);
        }

        keysValues.Values.Sort();
        for (int i = 0; i < 30; ++i)
        {
            Assert.AreEqual(2 * i, keysValues.Values[i]);
        }

        keysValues.Dispose();
    }

    [Test]
    public void NativeMultiHashMapGetKeys()
    {
        var hashMap = new NativeMultiHashMap<int, int> (1, Allocator.Temp);
        for (int i = 0; i < 30; ++i)
        {
            hashMap.Add(i, 2 * i);
            hashMap.Add(i, 3 * i);
        }
        var keys = hashMap.GetKeyArray(Allocator.Temp);
        hashMap.Dispose();

        Assert.AreEqual(60, keys.Length);
        keys.Sort();
        for (int i = 0; i < 30; ++i)
        {
            Assert.AreEqual(i, keys[i * 2 + 0]);
            Assert.AreEqual(i, keys[i * 2 + 1]);
        }
        keys.Dispose ();
    }

#if !UNITY_DOTSPLAYER
    [Test]
    public void NativeMultiHashMapGetUniqueKeysEmpty()
    {
        var hashMap = new NativeMultiHashMap<int, int> (1, Allocator.Temp);
        var keys = hashMap.GetUniqueKeyArray(Allocator.Temp);

        Assert.AreEqual(0, keys.Item1.Length);
        Assert.AreEqual(0, keys.Item2);
    }

    [Test]
    public void NativeMultiHashMapGetUniqueKeys()
    {
        var hashMap = new NativeMultiHashMap<int, int> (1, Allocator.Temp);
        for (int i = 0; i < 30; ++i)
        {
            hashMap.Add(i, 2 * i);
            hashMap.Add(i, 3 * i);
        }
        var keys = hashMap.GetUniqueKeyArray(Allocator.Temp);
        hashMap.Dispose();
        Assert.AreEqual(30, keys.Item2);
        for (int i = 0; i < 30; ++i)
        {
            Assert.AreEqual(i, keys.Item1[i]);
        }
        keys.Item1.Dispose();
    }
#endif
    [Test]
    public void NativeMultiHashMapGetValues()
    {
        var hashMap = new NativeMultiHashMap<int, int> (1, Allocator.Temp);
        for (int i = 0; i < 30; ++i)
        {
            hashMap.Add(i, 30 + i);
            hashMap.Add(i, 60 + i);
        }
        var values = hashMap.GetValueArray(Allocator.Temp);
        hashMap.Dispose();

        Assert.AreEqual(60, values.Length);
        values.Sort();
        for (int i = 0; i < 60; ++i)
        {
            Assert.AreEqual(30  + i, values[i]);
        }
        values.Dispose();
    }

    [Test]
    public void NativeMultiHashMapGetKeysAndValues()
    {
        var hashMap = new NativeMultiHashMap<int, int> (1, Allocator.Temp);
        for (int i = 0; i < 30; ++i)
        {
            hashMap.Add(i, 30 + i);
            hashMap.Add(i, 60 + i);
        }
        var keysValues = hashMap.GetKeyValueArrays(Allocator.Temp);
        hashMap.Dispose();

        Assert.AreEqual(60, keysValues.Keys.Length);
        Assert.AreEqual(60, keysValues.Values.Length);

        // ensure keys and matching values are aligned (though unordered)
        for (int i = 0; i < 30; ++i)
        {
            var k0 = keysValues.Keys[i * 2 + 0];
            var k1 = keysValues.Keys[i * 2 + 1];
            var v0 = keysValues.Values[i * 2 + 0];
            var v1 = keysValues.Values[i * 2 + 1];

            if (v0 > v1)
                (v0, v1) = (v1, v0);

            Assert.AreEqual(k0, k1);
            Assert.AreEqual(30 + k0, v0);
            Assert.AreEqual(60 + k0, v1);
        }

        keysValues.Keys.Sort();
        for (int i = 0; i < 30; ++i)
        {
            Assert.AreEqual(i, keysValues.Keys[i * 2 + 0]);
            Assert.AreEqual(i, keysValues.Keys[i * 2 + 1]);
        }

        keysValues.Values.Sort();
        for (int i = 0; i < 60; ++i)
        {
            Assert.AreEqual(30  + i, keysValues.Values[i]);
        }

        keysValues.Dispose();
    }

    public struct TestEntityGuid : IEquatable<TestEntityGuid>, IComparable<TestEntityGuid>
    {
        public ulong a;
        public ulong b;

        public bool Equals(TestEntityGuid other)
        {
            return a == other.a && b == other.b;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (a.GetHashCode() * 397) ^ b.GetHashCode();
            }
        }

        public int CompareTo(TestEntityGuid other)
        {
            var aComparison = a.CompareTo(other.a);
            if (aComparison != 0) return aComparison;
            return b.CompareTo(other.b);
        }
    }


    [Test]
    public void GetKeysGuid()
    {
        var hashMap = new NativeHashMap<TestEntityGuid, int> (1, Allocator.Temp);
        for (int i = 0; i < 30; ++i)
        {
            var didAdd = hashMap.TryAdd(new TestEntityGuid() { a = (ulong)i * 5, b = 3 * (ulong)i }, 2 * i);
            Assert.IsTrue(didAdd);
        }

        // Validate Hashtable has all the expected values
        Assert.AreEqual(30, hashMap.Length);
        for (int i = 0; i < 30; ++i)
        {
            int output;
            var exists = hashMap.TryGetValue(new TestEntityGuid() { a = (ulong)i * 5, b = 3 * (ulong)i }, out output);
            Assert.IsTrue(exists);
            Assert.AreEqual(2 * i, output);
        }

        // Validate keys array
        var keys = hashMap.GetKeyArray(Allocator.Temp);
        Assert.AreEqual(30, keys.Length);

        keys.Sort();
        for (int i = 0; i < 30; ++i)
        {
            Assert.AreEqual(new TestEntityGuid() { a = (ulong)i * 5, b = 3 * (ulong)i }, keys[i]);
        }

        hashMap.Dispose();
        keys.Dispose ();
    }
    
    [Test]
    public void IndexerWorks()
    {
        var hashMap = new NativeHashMap<int, int> (1, Allocator.Temp);
        hashMap[5] = 7;
        Assert.AreEqual(7, hashMap[5]);

        hashMap[5] = 9;
        Assert.AreEqual(9, hashMap[5]);

        hashMap.Dispose();
    }
    
    
    [Test]
    public void ContainsKeyHashMap()
    {
        var hashMap = new NativeHashMap<int, int> (1, Allocator.Temp);
        hashMap[5] = 7;
        
        Assert.IsTrue(hashMap.ContainsKey(5));
        Assert.IsFalse(hashMap.ContainsKey(6));

        hashMap.Dispose();
    }
    
    [Test]
    public void ContainsKeyMultiHashMap()
    {
        var hashMap = new NativeMultiHashMap<int, int> (1, Allocator.Temp);
        hashMap.Add(5, 7);
        
        hashMap.Add(6, 9);
        hashMap.Add(6, 10);
        
        Assert.IsTrue(hashMap.ContainsKey(5));
        Assert.IsTrue(hashMap.ContainsKey(6));
        Assert.IsFalse(hashMap.ContainsKey(4));

        hashMap.Dispose();
    }
    
    [Test]
    public void CountValuesForKey()
    {
        var hashMap = new NativeMultiHashMap<int, int> (1, Allocator.Temp);
        hashMap.Add(5, 7);
        hashMap.Add(6, 9);
        hashMap.Add(6, 10);
        
        Assert.AreEqual(1, hashMap.CountValuesForKey(5));
        Assert.AreEqual(2, hashMap.CountValuesForKey(6));
        Assert.AreEqual(0, hashMap.CountValuesForKey(7));

        hashMap.Dispose();
    }
    
    [Test]
    public void RemoveKeyAndValue()
    {
        var hashMap = new NativeMultiHashMap<int, long> (1, Allocator.Temp);
        hashMap.Add(10, 0);
        hashMap.Add(10, 1);
        hashMap.Add(10, 2);

        hashMap.Add(20, 2);
        hashMap.Add(20, 2);
        hashMap.Add(20, 1);
        hashMap.Add(20, 2);
        hashMap.Add(20, 1);

        hashMap.Remove(10, 1L);
        ExpectValues(hashMap, 10, new [] { 0L, 2L });
        ExpectValues(hashMap, 20, new [] { 1L, 1L, 2L, 2L, 2L });
        
        hashMap.Remove(20, 2L);
        ExpectValues(hashMap, 10, new [] { 0L, 2L });
        ExpectValues(hashMap, 20, new [] { 1L , 1L});

        hashMap.Remove(20, 1L);
        ExpectValues(hashMap, 10, new [] { 0L, 2L });
        ExpectValues(hashMap, 20, new long [0]);

        hashMap.Dispose();
    }
    

    [Test]
    public void MultiHashMapValueIterator()
    {
        var hashMap = new NativeMultiHashMap<int, int> (1, Allocator.Temp);
        hashMap.Add(5, 0);
        hashMap.Add(5, 1);
        hashMap.Add(5, 2);

        var list = new NativeList<int>(Allocator.TempJob);

        GCAllocRecorder.ValidateNoGCAllocs(() =>
        {
            list.Clear();
            foreach (var value in hashMap.GetValuesForKey(5))
                list.Add(value);
        });

        list.Sort();
        Assert.AreEqual(list.ToArray(), new int[] { 0, 1, 2 });
        
        foreach (var value in hashMap.GetValuesForKey(6))
            Assert.Fail();
        
        list.Dispose();
        hashMap.Dispose();
    }

    [Test]
    public void NativeHashMap_DisposeJob()
    {
        var container0 = new NativeHashMap<int, int>(1, Allocator.Persistent);
        Assert.True(container0.IsCreated);
        Assert.DoesNotThrow(() => { container0.Add(0, 1); });
        Assert.True(container0.ContainsKey(0));

        var container1 = new NativeMultiHashMap<int, int>(1, Allocator.Persistent);
        Assert.True(container1.IsCreated);
        Assert.DoesNotThrow(() => { container1.Add(1, 2); });
        Assert.True(container1.ContainsKey(1));

        var disposeJob0 = container0.Dispose(default);
        Assert.False(container0.IsCreated);
        Assert.Throws<InvalidOperationException>(() => { container0.ContainsKey(0); });

        var disposeJob = container1.Dispose(disposeJob0);
        Assert.False(container1.IsCreated);
        Assert.Throws<InvalidOperationException>(() => { container1.ContainsKey(1); });

        disposeJob.Complete();
    }
    
    [Test]
    public void RemoveKeyValueDoesntDeallocate()
    {
        var hashMap = new NativeMultiHashMap<int, int> (1, Allocator.Temp);
        hashMap.Add(5, 1);

        hashMap.Remove(5, 5);
        GCAllocRecorder.ValidateNoGCAllocs(() =>
        {
            hashMap.Remove(5, 1);
        });
        
        hashMap.Dispose();
    }
    
}
