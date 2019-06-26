
using System;
using NUnit.Framework;
using Unity.Collections.Tests;
 
public class GcAllocRecorderTest
{
    [Test] 
    public void Test ()
    { 
        GCAllocRecorder.BeginNoGCAlloc();
        GCAllocRecorder.EndNoGCAlloc();
    }
}


namespace Unity.Collections.Tests
{
//#if !UNITY_DOTSPLAYER
#if false

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

        public static void ValidateNoGCAllocs(Action action)
        {
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
