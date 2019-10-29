using System;
using System.ComponentModel;
using NUnit.Framework;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

[BurstCompile]
public unsafe class BurstDelegateTest
{
    delegate void DoThingDelegate(ref int value);

    [BurstCompile]
    static void DoThing(ref int value)
    {
        value++;
    }

    static void DoThingMissingBurstCompile(ref int value)
    {
        value++;
    }

    
    
    [Test]
    public void ManagedDelegateTest()
    {
        var funcPtr = BurstCompiler.CompileFunctionPointer<DoThingDelegate>(DoThing);

        // NOTE: funcPtr.Invoke allocates GC memory,
        // so in real world use cases we want to cache the managed delegate, not the FunctionPointer
        DoThingDelegate cachableDelegate = funcPtr.Invoke;
        
        int value = 5;
        cachableDelegate(ref value);
        Assert.AreEqual(6, value);
    }

    [Test]
    public void JobFunctionPointerTest()
    {
        var funcPtr = BurstCompiler.CompileFunctionPointer<DoThingDelegate>(DoThing);

        var job = new MyJob();
        int value = 5;
        job.Blah = &value;
        job.FunctionPointer = funcPtr;
        
        job.Schedule().Complete();
        
        Assert.AreEqual(6, value);
    }
    
    [BurstCompile]
    struct MyJob : IJob
    {
        [NativeDisableUnsafePtrRestriction]
        public int* Blah;
        public FunctionPointer<DoThingDelegate> FunctionPointer;

        unsafe public void Execute()
        {
            FunctionPointer.Invoke(ref *Blah);
        }
    }
    
    [Test]
    public void CompileMissingBurstCompile()
    {
        Assert.Throws<InvalidOperationException>(()=>BurstCompiler.CompileFunctionPointer<DoThingDelegate>(DoThingMissingBurstCompile));
    }
}