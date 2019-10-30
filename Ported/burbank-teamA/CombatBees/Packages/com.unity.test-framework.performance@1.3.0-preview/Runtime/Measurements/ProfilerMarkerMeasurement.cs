using System.Collections.Generic;
using Unity.PerformanceTesting.Runtime;
using UnityEngine;

namespace Unity.PerformanceTesting.Measurements
{
    internal class ProfilerMarkerMeasurement : MonoBehaviour
    {
        private bool m_MeasurementStarted;

        private readonly List<SampleGroup> m_SampleGroups = new List<SampleGroup>();

        public void AddProfilerSample(SampleGroupDefinition[] samplesGroup)
        {
            foreach (var sample in samplesGroup)
            {
                AddProfilerSample(new SampleGroup(sample));
            }
        }

        private void AddProfilerSample(SampleGroup sampleGroup)
        {
            sampleGroup.GetRecorder();
            sampleGroup.Recorder.enabled = true;
            m_SampleGroups.Add(sampleGroup);
        }

        private void SampleProfilerSamples()
        {
            foreach (var sampleGroup in m_SampleGroups)
            {
                Measure.Custom(sampleGroup.Definition,
                    Utils.ConvertSample(SampleUnit.Nanosecond, sampleGroup.Definition.SampleUnit,
                        sampleGroup.Recorder.elapsedNanoseconds));
            }
        }

        public void StopAndSampleRecorders()
        {
            foreach (var sampleGroup in m_SampleGroups)
            {
                sampleGroup.Recorder.enabled = false;
                Measure.Custom(sampleGroup.Definition,
                    Utils.ConvertSample(SampleUnit.Nanosecond, sampleGroup.Definition.SampleUnit,
                        sampleGroup.Recorder.elapsedNanoseconds));
            }
        }

        public void Update()
        {
            if (!m_MeasurementStarted)
            {
                m_MeasurementStarted = true;
            }
            else
            {
                SampleProfilerSamples();
            }
        }
    }
}