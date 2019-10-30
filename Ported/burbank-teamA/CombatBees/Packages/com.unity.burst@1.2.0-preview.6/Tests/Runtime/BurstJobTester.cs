using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Shared class used for Unit tests and <see cref="MyBurstBehavior"/>
/// </summary>
[BurstCompile] // attribute added just to check that static methods are getting compiled
public class BurstJobTester : IDisposable
{
    private NativeArray<float> _array;
    private NativeArray<float> _arrayAsyncJobDefault;
    private NativeArray<float> _arrayAsyncJobFast;

    public BurstJobTester()
    {
        _array = new NativeArray<float>(10, Allocator.Persistent);
        _arrayAsyncJobDefault = new NativeArray<float>(10, Allocator.Persistent);
        _arrayAsyncJobFast = new NativeArray<float>(10, Allocator.Persistent);
    }

    public void Dispose()
    {
        _array.Dispose();
        _arrayAsyncJobDefault.Dispose();
        _arrayAsyncJobFast.Dispose();
    }

    public float Calculate()
    {
        // Schedule the job on each frame to make sure that it will be compiled async on the next frame
        _array[0] = 0.0f;
        // Launch synchronous job
        var job = new MyJob { Result = _array };
        job.Schedule().Complete();
        var rotation = job.Result[0];

        // Launch an async compilation
        var asyncJobNoOptim = new MyJobWithDefaultOptimizations() {Result = _arrayAsyncJobDefault};
        var asyncJobFastOptim = new MyJobWithFastOptimizations() {Result = _arrayAsyncJobFast};
        var asyncJobNoOptimHandle = asyncJobNoOptim.Schedule();
        var asyncJobFastOptimHandle = asyncJobFastOptim.Schedule();

        // Wait for async completion
        asyncJobNoOptimHandle.Complete();
        asyncJobFastOptimHandle.Complete();

        return rotation;
    }

    public float CheckFunctionPointer()
    {
        var functionPointer1 = BurstCompiler.CompileFunctionPointer<Add2NumbersDelegate>(Add2Numbers);
        var result = functionPointer1.Invoke(1.0f, 2.0f);

        var functionPointer2 = BurstCompiler.CompileFunctionPointer<Add2NumbersDelegate>(Add2NumbersThrows);
        return functionPointer2.Invoke(1.0f, 2.0f);
    }

    [BurstCompile(CompileSynchronously = true)] // attribute used for a static method
    public static float Add2Numbers(float a, float b)
    {
        DiscardFunction(ref a);
        DiscardFunction(ref b);
        return a + b;
    }

    [BurstCompile(CompileSynchronously = true)] // attribute used for a static method
    public static float Add2NumbersThrows(float a, float b)
    {
        DiscardFunction(ref a);
        DiscardFunction(ref b);
        if (a > 0) throw new ArgumentException("Invalid a must be < 0");
        return a + b;
    }

    [BurstDiscard]
    private static void DiscardFunction(ref float x)
    {
        x = 0;
    }

    public delegate float Add2NumbersDelegate(float a, float b);

    [BurstCompile(CompileSynchronously = true)]
    public struct MyJob : IJob
    {
        [WriteOnly]
        public NativeArray<float> Result;

        public void Execute()
        {
            Result[0] = ChangeValue();
            EraseRotation();
        }

        // Use an indirection: Execute -> instance method -> static method
        // (to check caching manually, change "1.0f" in ChangeValue() and 2.0f in ChangeValueStatic())
        private float ChangeValue()
        {
            return 1.0f + ChangeValueStatic();
        }

        private static float ChangeValueStatic()
        {
            return 2.0f;
        }

        // Use BurstDiscard, if burst is not available, this method will get executed and it will make the cube static on the screen.
        [BurstDiscard]
        private void EraseRotation()
        {
            Result[0] = 0.0f;
        }

        // static method in a burst job, but we still want to compile separately
        [BurstCompile(FloatMode = FloatMode.Deterministic, CompileSynchronously = true)]
        public static float CheckFmaSlow(float a, float b, float c)
        {
            return a * b + c + math.sin(c);
        }

        // static method in a burst job, but we still want to compile separately
        // Used only to check that compilation is working for different burst compile options
        [BurstCompile(FloatPrecision.Low, FloatMode.Fast, CompileSynchronously = true)]
        public static float CheckFmaFast(float a, float b, float c)
        {
            return a * b + c + math.sin(c);
        }
    }

    [BurstCompile]
    public struct MyJobWithDefaultOptimizations : IJob
    {
        public NativeArray<float> Result;

        public void Execute()
        {
            Result[0] = math.cos(Result[0]);
        }
    }

    // Used only to check that compilation is working for different burst compile options
    [BurstCompile(FloatPrecision.Low, FloatMode.Fast)]
    public struct MyJobWithFastOptimizations : IJob
    {
        public NativeArray<float> Result;

        public void Execute()
        {
            Result[0] = math.cos(Result[0]);
        }
    }
}
