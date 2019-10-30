using System;
using System.Collections.Generic;
using Unity.PerformanceTesting.Exceptions;
using UnityEngine.Profiling;

namespace Unity.PerformanceTesting
{
    [Serializable]
    public class SampleGroup
    {
        // Serialized fields
        public SampleGroupDefinition Definition;

        public List<double> Samples;
        public double Min;
        public double Max;
        public double Median;
        public double Average;
        public double StandardDeviation;
        public double PercentileValue;
        public double Sum;
        public int Zeroes;
        public int SampleCount;

        public Recorder Recorder { get; set; }

        public SampleGroup(SampleGroupDefinition definition)
        {
            Definition = definition;
            Samples = new List<double>();
        }

        public void GetRecorder()
        {
            if (Recorder == null)
                Recorder = Recorder.Get(Definition.Name);
        }

        public void GetAggregateValue()
        {
        }
    }

    [Serializable]
    public struct SampleGroupDefinition
    {
        public string Name;
        public SampleUnit SampleUnit;
        public AggregationType AggregationType;
        public double Threshold;
        public bool IncreaseIsBetter;
        public double Percentile;
        public bool FailOnBaseline;

        public SampleGroupDefinition(string name = "Time", SampleUnit sampleUnit = SampleUnit.Millisecond,
            AggregationType aggregationType = AggregationType.Median, double threshold = 0.15D,
            bool increaseIsBetter = false, bool failOnBaseline = true)
        {
            Threshold = threshold;
            Name = name;
            SampleUnit = sampleUnit;
            AggregationType = aggregationType;
            IncreaseIsBetter = increaseIsBetter;
            Percentile = 0;
            FailOnBaseline = failOnBaseline;
        }

        public SampleGroupDefinition(string name, SampleUnit sampleUnit, AggregationType aggregationType,
            double percentile,
            double threshold = 0.15D, bool increaseIsBetter = false, bool failOnBaseline = true)
        {
            Threshold = threshold;
            Name = name;
            SampleUnit = sampleUnit;
            AggregationType = aggregationType;
            Percentile = percentile;
            IncreaseIsBetter = increaseIsBetter;
            FailOnBaseline = failOnBaseline;
            if (Percentile > 1D || Percentile < 0D)
                throw new PerformanceTestException("Percentile has to be defined in range [0:1].");
        }
    }

    public enum AggregationType
    {
        Average = 0,
        Min = 1,
        Max = 2,
        Median = 3,
        Percentile = 4
    }

    public enum SampleUnit
    {
        Nanosecond,
        Microsecond,
        Millisecond,
        Second,
        Byte,
        Kilobyte,
        Megabyte,
        Gigabyte,
        None
    }
}