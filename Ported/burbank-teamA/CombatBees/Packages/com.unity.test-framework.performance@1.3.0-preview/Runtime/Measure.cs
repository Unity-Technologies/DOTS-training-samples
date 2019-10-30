using System;
using System.Diagnostics;
using Unity.PerformanceTesting.Runtime;
using Unity.PerformanceTesting.Measurements;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.PerformanceTesting
{
    public static class Measure
    {
        public static void Custom(SampleGroupDefinition sampleGroupDefinition, double value)
        {
            var sampleGroup = PerformanceTest.GetSampleGroup(sampleGroupDefinition);
            if (sampleGroup == null)
            {
                sampleGroup = new SampleGroup(sampleGroupDefinition);
                PerformanceTest.Active.SampleGroups.Add(sampleGroup);
            }

            sampleGroup.Samples.Add(value);
        }

        public static ScopeMeasurement Scope(SampleGroupDefinition sampleGroupDefinition)
        {
            return new ScopeMeasurement(sampleGroupDefinition);
        }

        public static ProfilerMeasurement ProfilerMarkers(params SampleGroupDefinition[] sampleGroupDefinitions)
        {
            return new ProfilerMeasurement(sampleGroupDefinitions);
        }

        public static ProfilerMeasurement ProfilerMarkers(params string[] profilerMarkerNames)
        {
            var definitions = new SampleGroupDefinition[profilerMarkerNames.Length];
            for (var i = 0; i < profilerMarkerNames.Length; i++)
                definitions[i] = new SampleGroupDefinition(profilerMarkerNames[i]);

            return new ProfilerMeasurement(definitions);
        }

        public static MethodMeasurement Method(Action action)
        {
            return new MethodMeasurement(action);
        }

        public static FramesMeasurement Frames()
        {
            return new FramesMeasurement();
        }

        // Overloads

        public static ScopeMeasurement Scope()
        {
            return new ScopeMeasurement(new SampleGroupDefinition("Time"));
        }
    }

    public struct ScopeMeasurement : IDisposable
    {
        private readonly SampleGroup m_TimeSampleGroup;
        private readonly long m_StartTicks;

        public ScopeMeasurement(SampleGroupDefinition sampleGroupDefinition)
        {
            m_TimeSampleGroup = new SampleGroup(sampleGroupDefinition);
            m_StartTicks = Stopwatch.GetTimestamp();
            PerformanceTest.Disposables.Add(this);
        }

        public void Dispose()
        {
            var elapsedTicks = Stopwatch.GetTimestamp() - m_StartTicks;
            PerformanceTest.Disposables.Remove(this);
            var delta = TimeSpan.FromTicks(elapsedTicks).TotalMilliseconds;

            Measure.Custom(m_TimeSampleGroup.Definition,
                Utils.ConvertSample(SampleUnit.Millisecond, m_TimeSampleGroup.Definition.SampleUnit, delta));
        }
    }

    public struct ProfilerMeasurement : IDisposable
    {
        private readonly ProfilerMarkerMeasurement m_Test;

        public ProfilerMeasurement(SampleGroupDefinition[] sampleGroupDefinitions)
        {
            if (sampleGroupDefinitions == null)
            {
                m_Test = null;
                return;
            }

            if (sampleGroupDefinitions.Length == 0)
            {
                m_Test = null;
                return;
            }

            var go = new GameObject("Recorder");
            if (Application.isPlaying) Object.DontDestroyOnLoad(go);
            m_Test = go.AddComponent<ProfilerMarkerMeasurement>();
            m_Test.AddProfilerSample(sampleGroupDefinitions);
            PerformanceTest.Disposables.Add(this);
        }

        public void Dispose()
        {
            PerformanceTest.Disposables.Remove(this);
            if (m_Test == null) return;
            m_Test.StopAndSampleRecorders();
            Object.DestroyImmediate(m_Test.gameObject);
        }
    }
}