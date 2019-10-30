using System;
using NUnit.Framework;
using Unity.Collections.Tests;
 
#if !UNITY_DOTSPLAYER

public class GcAllocRecorderTest
{
    [Test] 
    public void TestBeginEnd ()
    { 
        GCAllocRecorder.BeginNoGCAlloc();
        GCAllocRecorder.EndNoGCAlloc();
    }

    // NOTE: Causing GC allocation with new requires an unused variable
#pragma warning disable 219
    [Test] 
    public void TestNoAlloc ()
    { 
        GCAllocRecorder.ValidateNoGCAllocs(() =>
        {
            var p = new int();
        });
    }
    
    [Test] 
    public void TestAlloc()
    {
        Assert.Throws<AssertionException>(() =>
        {
            GCAllocRecorder.ValidateNoGCAllocs(() =>
            {
                var p = new int[5];
            });
        });
    }
#pragma warning restore 219
}
#endif


namespace Unity.Collections.Tests
{
#if !UNITY_DOTSPLAYER

    public static class GCAllocRecorder
    {
        static UnityEngine.Profiling.Recorder AllocRecorder;

        static GCAllocRecorder()
        {
            AllocRecorder = UnityEngine.Profiling.Recorder.Get("GC.Alloc");
        }

        public static int CountGCAllocs(Action action)
        { 
            AllocRecorder.FilterToCurrentThread();
            AllocRecorder.enabled = false;
            AllocRecorder.enabled = true;

            action();
            
            AllocRecorder.enabled = false;
            return AllocRecorder.sampleBlockCount;
        }

        // NOTE: action is called twice to warmup any GC allocs that can happen due to static constructors etc. 
        public static void ValidateNoGCAllocs(Action action)
        {
            // warmup
            CountGCAllocs(action);
            
            // actual test
            var count = CountGCAllocs(action);
            if (count != 0)
                throw new AssertionException($"Expected 0 GC allocations but there were {count}");
        }

        public static void BeginNoGCAlloc()
        {
            AllocRecorder.FilterToCurrentThread();
            AllocRecorder.enabled = false;
            AllocRecorder.enabled = true;
        }

        public static void EndNoGCAlloc()
        {
            AllocRecorder.enabled = false;
            int count = AllocRecorder.sampleBlockCount;
            if (count != 0)
                throw new AssertionException($"Expected 0 GC allocations but there were {count}");
        }
    }
#else
    public static class GCAllocRecorder
    {
        public static void ValidateNoGCAllocs(Action action)
        {
            action();
        }

        public static void BeginNoGCAlloc()
        {
        }

        public static void EndNoGCAlloc()
        {
        }
    }
#endif
}
