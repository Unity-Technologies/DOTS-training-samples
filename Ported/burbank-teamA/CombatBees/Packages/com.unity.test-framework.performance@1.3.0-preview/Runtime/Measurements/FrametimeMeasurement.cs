using UnityEngine;

namespace Unity.PerformanceTesting.Measurements
{
    internal class FrameTimeMeasurement : MonoBehaviour
    {
        public SampleGroupDefinition SampleGroupDefinition;

        void Update()
        {
            Measure.Custom(SampleGroupDefinition, Time.unscaledDeltaTime * 1000);
        }
    }
}