using System;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

// https://preshing.com/20120522/lightweight-in-memory-logging/
public class InMemoryLoggerExample : MonoBehaviour
{
    [SerializeField]
    int m_ArrayLength = 1000;
    static ProfilerMarker s_Jobs = new ProfilerMarker("TestJobs");

    [SerializeField]
    bool m_AlwaysDump;
    [FormerlySerializedAs("m_AppendToLogger")]
    [SerializeField]
    bool m_CreateTestLogEntries = true;
    string m_LastLogDump = "Click to Dump";
    InMemoryLogger m_Logger;
    NativeArray<FixedString128> m_loggerBuffer;
    NativeArray<int> m_loggerDataBuffer;

    [SerializeField]
    int m_DumpStartOffset = 0;
    
    [SerializeField]
    int m_DumpLogCount = 10;
    [SerializeField]
    float m_WritePerc = 0.5f;

    void Awake()
    {
        m_loggerBuffer = new NativeArray<FixedString128>(1024*2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        m_loggerDataBuffer = new NativeArray<int>(2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        m_Logger = new InMemoryLogger(m_loggerBuffer, m_loggerDataBuffer);
        Random.InitState(DateTime.Now.Second);
    }

    void Update()
    {
        if (m_CreateTestLogEntries)
            using (s_Jobs.Auto())
            {
                var a = new TestJob
                {
                    JobSeed = (uint)((double)Random.value * uint.MaxValue),
                    Logger = m_Logger,
                    JobName = new FixedString32("A"),
                    WritePerc = m_WritePerc,
                }.Schedule(m_ArrayLength, 16);
                var b = new TestJob
                {
                    JobSeed = (uint)(Random.value * uint.MaxValue),
                    Logger = m_Logger,
                    JobName = new FixedString32("B"),
                    WritePerc = m_WritePerc,
                }.Schedule(m_ArrayLength, 16);
                var c = new TestJob
                {
                    JobSeed = (uint)(Random.value * uint.MaxValue),
                    Logger = m_Logger,
                    JobName = new FixedString32("C"),
                    WritePerc = m_WritePerc,
                }.Schedule(m_ArrayLength, 16);
                
                JobHandle.ScheduleBatchedJobs();
                JobHandle.CompleteAll(ref a, ref b, ref c);
            }

        if (m_AlwaysDump) m_LastLogDump = m_Logger.Dump(m_DumpStartOffset, m_DumpLogCount);
    }

    void OnDestroy()
    {
        m_loggerBuffer.Dispose();
        m_loggerDataBuffer.Dispose();
        m_Logger.Dispose();
    }

    void OnGUI()
    {
        GUI.color = Color.cyan;
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button($"Always Dump?\nCurrently: {(m_AlwaysDump ? "YES" : "NO")}")) m_AlwaysDump ^= true;
            if (GUILayout.Button("Dump Once\n")) m_LastLogDump = m_Logger.Dump();
            if (GUILayout.Button($"Create Test Log Entries?\nCurrently: {(m_CreateTestLogEntries ? "YES" : "NO")}")) m_CreateTestLogEntries ^= true;
        }
        GUILayout.EndHorizontal();

        m_DumpStartOffset = (int)GUILayout.HorizontalSlider(m_DumpStartOffset, -1, m_loggerBuffer.Length);
        m_DumpLogCount = (int)GUILayout.HorizontalSlider(m_DumpLogCount, 1, 200);
        if (GUILayout.Button($"Logger: {m_Logger.LogCount:n0} of {m_Logger.Capacity:n0} | Current Logger Index: {m_Logger.CurrentIndex:n0} | Showing logs: {(m_DumpStartOffset < 0 ? "CURRENT" : $"{m_DumpStartOffset:n0} to {m_DumpStartOffset + m_DumpLogCount:n0}")}")) m_DumpStartOffset = m_Logger.CurrentIndex;
        m_WritePerc = GUILayout.HorizontalSlider(m_WritePerc, 0, 1f);
        m_ArrayLength = (int)GUILayout.HorizontalSlider(m_ArrayLength, 0, 1_000_000f);
        GUILayout.Box($"Write Log % Chance: {m_WritePerc:p1}, A, B, C Job Log Iterations: {m_ArrayLength:n0}");
        GUI.color = Color.white;

        if (GUILayout.Button(m_LastLogDump, GUILayout.Width(Screen.width - 10))) { }
    }

    [BurstCompile]
    struct TestJob : IJobParallelFor
    {
        public uint JobSeed;

        [NativeDisableUnsafePtrRestriction]
        [NativeDisableContainerSafetyRestriction]
        public InMemoryLogger Logger;
        public FixedString32 JobName;
        public float WritePerc;

        public void Execute(int index)
        {
            var randValue = Squirrel3.NextDouble((uint)index, JobSeed, 0, 1f);
            if (randValue < WritePerc)
            {
                var log = new FixedString128(FixedString.Format("{0}: Squirrel3({1}, {2}) = {3}", JobName, JobSeed, index, (float)randValue));
                Logger.Append(log);
            }
        }
    }
}

public struct InMemoryLogger : IDisposable
{
    static ProfilerMarker s_DumpStackAlloc = new ProfilerMarker("InMemoryLogger.DumpStackAlloc");
    
    LockFreeRingBuffer<FixedString128> m_RingBuffer;

    public int LogCount => m_RingBuffer.Count;

    public int CurrentIndex => m_RingBuffer.CurrentIndex;
    
    public int Capacity => m_RingBuffer.Capacity;
    
    public InMemoryLogger(NativeArray<FixedString128> buffer, NativeArray<int> dataBuffer)
    {
        m_RingBuffer = new LockFreeRingBuffer<FixedString128>(buffer, dataBuffer);
        // NWALKER - Replace with Blit.
        for (var i = 0; i < buffer.Length; i++) buffer[i] = default;
    }

    public void Dispose()
    {
        // var buff = m_RingBuffer.BufferAsNativeArray;
        // if(buff.IsCreated)
        // {
        //     buff.Dispose();
        // }
    }

    public void Append(FixedString128 log)
    {
        m_RingBuffer.Append(log);
    }
    
    /// <summary>
    ///     Dump a bunch of logs from the logger.
    /// </summary>
    /// <param name="startOffset">Start log index. Default 0.</param>
    /// <param name="count">Number of log lines. Default -1 (all)</param>
    /// <returns></returns>
    public unsafe string Dump(int startOffset = 0, int count = -1)
    {
        using (s_DumpStackAlloc.Auto())
        {
            var reverseFromCurrent = startOffset < 0;
            if (count < 0) count = m_RingBuffer.Capacity;
            if (reverseFromCurrent) startOffset = m_RingBuffer.CurrentIndex;
            var buffer = m_RingBuffer.Buffer;
            var tempArr = new NativeList<char>(FixedString128.UTF8MaxLengthInBytes * count + 1, Allocator.Temp);
            var resultCharPtr = (char*)tempArr.GetUnsafePtr();
            var finalLength = 0;
            
            for (var i = 0; i < count; i++)
            {
                var logIndex = (m_RingBuffer.Capacity + startOffset + (reverseFromCurrent ? -i : i)) % m_RingBuffer.Capacity;
                var log = buffer[logIndex];

                // NW: Copy the conversion directly into the output array.
                var destOffsetPtr = resultCharPtr + finalLength;
                Unicode.Utf8ToUtf16(log.GetUnsafePtr(), log.Length, destOffsetPtr, out var lengthOfTempUtf16Str, log.Length * 2);
                
                // Replace the null terminators with a newline:
                finalLength += lengthOfTempUtf16Str + 1;
                resultCharPtr[finalLength - 1] = '\n';
                resultCharPtr[finalLength] = '\n';
            }

            

            // Crop last \n.
            var resultStr = new string(resultCharPtr, 0, finalLength - 1);
            tempArr.Dispose();
            return resultStr;
        }
    }
}

public unsafe struct LockFreeRingBuffer<T> where T : unmanaged
{
    [NativeDisableUnsafePtrRestriction]
    public T* Buffer;
    public int Capacity;

    [NativeDisableUnsafePtrRestriction]
    public int* Data;

    public int Count => Data[1] != 0 ? Capacity : Data[0];

    public int CurrentIndex => Data[0];

    public LockFreeRingBuffer(NativeArray<T> buffer, NativeArray<int> data)
    {
        Buffer = (T*)buffer.GetUnsafePtr();
        Capacity = buffer.Length;
        if (data.Length != 2) throw new InvalidOperationException();

        Data = (int*)data.GetUnsafePtr();
        Data[0] = 0;
        Data[1] = 0;
    }

    public void Clear()
    {
        Data[1] = 0;
        Thread.MemoryBarrier();
        Interlocked.Exchange(ref Data[0], 0);
    }

    public void Append(T value)
    {
        int nextIndex;
        int lastValue;
        do
        {
            lastValue = CurrentIndex;
            nextIndex = (lastValue + 1) % Capacity;
        } while (Interlocked.CompareExchange(ref Data[0], nextIndex, lastValue) != lastValue);

        Buffer[lastValue] = value;

        Data[1] = math.max(Data[1], math.select(1, 0, nextIndex < lastValue));
    }
}
