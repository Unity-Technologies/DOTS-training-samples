using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Unity.PerformanceTesting.Measurements
{
    public class FramesMeasurement
    {
        private const int k_MinTestTimeMs = 500;
        private const int k_MinWarmupTimeMs = 80;
        private const int k_ProbingMultiplier = 4;
        private const int k_MinIterations = 7;

        private SampleGroupDefinition[] m_ProfilerDefinitions;
        private SampleGroupDefinition m_Definition;
        private int m_DesiredFrameCount;
        private int m_Executions;
        private int m_Warmup = -1;
        private bool m_RecordFrametime = true;

        public FramesMeasurement ProfilerMarkers(params SampleGroupDefinition[] profilerDefinitions)
        {
            m_ProfilerDefinitions = profilerDefinitions;
            return this;
        }
        
        public FramesMeasurement ProfilerMarkers(params string[] profilerMarkerNames)
        {
            var definitions = new SampleGroupDefinition[profilerMarkerNames.Length];
            for (int i = 0; i < profilerMarkerNames.Length; i++)
                definitions[i] = new SampleGroupDefinition(profilerMarkerNames[i]);
            m_ProfilerDefinitions = definitions;
            return this;
        }

        public FramesMeasurement Definition(SampleGroupDefinition definition)
        {
            m_Definition = definition;
            return this;
        }

        public FramesMeasurement Definition(string name = "Time", SampleUnit sampleUnit = SampleUnit.Millisecond,
            AggregationType aggregationType = AggregationType.Median, double threshold = 0.1D,
            bool increaseIsBetter = false, bool failOnBaseline = true)
        {
            return Definition(new SampleGroupDefinition(name, sampleUnit, aggregationType, threshold, increaseIsBetter,
                failOnBaseline));
        }

        public FramesMeasurement Definition(string name, SampleUnit sampleUnit, AggregationType aggregationType,
            double percentile, double threshold = 0.1D, bool increaseIsBetter = false, bool failOnBaseline = true)
        {
            return Definition(new SampleGroupDefinition(name, sampleUnit, aggregationType, percentile, threshold,
                increaseIsBetter, failOnBaseline));
        }

        public FramesMeasurement MeasurementCount(int count)
        {
            m_Executions = count;
            return this;
        }

        public FramesMeasurement WarmupCount(int count)
        {
            m_Warmup = count;
            return this;
        }

        public FramesMeasurement DontRecordFrametime()
        {
            m_RecordFrametime = false;
            return this;
        }

        public ScopedFrameTimeMeasurement Scope()
        {
            return Scope(new SampleGroupDefinition("FrameTime"));
        }

        public ScopedFrameTimeMeasurement Scope(SampleGroupDefinition sampleGroupDefinition)
        {
            return new ScopedFrameTimeMeasurement(sampleGroupDefinition);
        }

        public IEnumerator Run()
        {
            if (m_Executions == 0 && m_Warmup >= 0)
            {
                Debug.LogError("Provide execution count or remove warmup count from frames measurement.");
                yield break;
            }

            UpdateSampleGroupDefinition();
            yield return m_Warmup > -1 ? WaitFor(m_Warmup) : GetDesiredIterationCount();
            m_DesiredFrameCount = m_Executions > 0 ? m_Executions : m_DesiredFrameCount;


            using (Measure.ProfilerMarkers(m_ProfilerDefinitions))
            {
                for (var i = 0; i < m_DesiredFrameCount; i++)
                {
                    if (m_RecordFrametime)
                    {
                        using (Measure.Scope(m_Definition))
                        {
                            yield return null;
                        }
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }
        }

        private IEnumerator GetDesiredIterationCount()
        {
            var executionTime = 0.0D;
            var iterations = 1;

            while (executionTime < k_MinWarmupTimeMs)
            {
                var sw = Stopwatch.GetTimestamp();

                yield return WaitFor(iterations);

                executionTime = TimeSpan.FromTicks(Stopwatch.GetTimestamp()-sw).TotalMilliseconds;

                if (iterations == 1 && executionTime > 40)
                {
                    m_DesiredFrameCount = k_MinIterations;
                    yield break;
                }

                if (iterations == 64)
                {
                    m_DesiredFrameCount = 120;
                    yield break;
                }

                if (executionTime < k_MinWarmupTimeMs)
                {
                    iterations *= k_ProbingMultiplier;
                }
            }

            m_DesiredFrameCount = (int) (k_MinTestTimeMs * iterations / executionTime);
        }

        private IEnumerator WaitFor(int iterations)
        {
            for (var i = 0; i < iterations; i++)
            {
                yield return null;
            }
        }


        private void UpdateSampleGroupDefinition()
        {
            if (m_Definition.Name == null)
            {
                m_Definition = new SampleGroupDefinition("Time");
            }

            if (m_ProfilerDefinitions == null)
            {
                m_ProfilerDefinitions = new SampleGroupDefinition[0];
            }
        }
    }

    public struct ScopedFrameTimeMeasurement : IDisposable
    {
        private readonly FrameTimeMeasurement m_Test;

        public ScopedFrameTimeMeasurement(SampleGroupDefinition sampleGroupDefinition)
        {
            var go = new GameObject("Recorder");
            if (Application.isPlaying) Object.DontDestroyOnLoad(go);
            m_Test = go.AddComponent<FrameTimeMeasurement>();
            m_Test.SampleGroupDefinition = sampleGroupDefinition;
            PerformanceTest.Disposables.Add(this);
        }

        public void Dispose()
        {
            PerformanceTest.Disposables.Remove(this);
            Object.DestroyImmediate(m_Test.gameObject);
        }
    }
}