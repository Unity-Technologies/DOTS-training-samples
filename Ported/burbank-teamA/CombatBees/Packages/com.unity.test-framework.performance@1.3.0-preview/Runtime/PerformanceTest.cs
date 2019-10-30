using System;
using System.Collections.Generic;
using System.Text;
using Unity.PerformanceTesting.Runtime;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Unity.PerformanceTesting.Exceptions;
using UnityEngine;
using UnityEngine.TestRunner.NUnitExtensions;

namespace Unity.PerformanceTesting
{
    [Serializable]
    public class PerformanceTest
    {
        // Serialized fields
        public string TestName;
        public List<string> TestCategories = new List<string>();
        public string TestVersion;
        public double StartTime;
        public double EndTime;
        public List<SampleGroup> SampleGroups = new List<SampleGroup>();

        public static PerformanceTest Active { get; private set; }
        internal static List<IDisposable> Disposables = new List<IDisposable>(1024);
        PerformanceTestHelper m_PerformanceTestHelper;

        public delegate void Callback();

        public static Callback OnTestEnded;

        public PerformanceTest()
        {
            Active = this;
        }
        
        class PerformanceTestHelper : MonoBehaviour
        {
            public PerformanceTest ActiveTest;

            void OnEnable()
            {
                if (PerformanceTest.Active == null)
                    PerformanceTest.Active = ActiveTest;
            }
        }

        internal static void StartTest(ITest currentTest)
        {
            if (currentTest.IsSuite) return;
            
            var go = new GameObject("PerformanceTestHelper");
            go.hideFlags = HideFlags.HideAndDontSave;
            var performanceTestHelper = go.AddComponent<PerformanceTestHelper>();

            var test = new PerformanceTest
            {
                TestName = currentTest.FullName,
                TestCategories = currentTest.GetAllCategoriesFromTest(),
                TestVersion = GetVersion(currentTest),
                StartTime = Utils.DateToInt(DateTime.Now),
                m_PerformanceTestHelper = performanceTestHelper
            };

            Active = test;
            performanceTestHelper.ActiveTest = test;
        }

        private static string GetVersion(ITest currentTest)
        {
            string version = "";
            var methodVersions = currentTest.Method.GetCustomAttributes<VersionAttribute>(false);
            var classVersion = currentTest.TypeInfo.Type.GetCustomAttributes(typeof(VersionAttribute), true);

            if (classVersion.Length > 0)
                version = ((VersionAttribute) classVersion[0]).Version+".";
            if (methodVersions.Length > 0)
                version += methodVersions[0].Version;
            else
                version += "1";

            return version;
        }

        internal static void EndTest(ITest test)
        {
            if (test.IsSuite) return;
            if (test.FullName != Active.TestName) return;

            if (Active.m_PerformanceTestHelper != null && Active.m_PerformanceTestHelper.gameObject != null)
                UnityEngine.Object.DestroyImmediate(Active.m_PerformanceTestHelper.gameObject);
            
            DisposeMeasurements();
            Active.CalculateStatisticalValues();
            Active.EndTime = Utils.DateToInt(DateTime.Now);
            if (OnTestEnded != null) OnTestEnded();
            Active.LogOutput();

            TestContext.Out.WriteLine("##performancetestresult:" + JsonUtility.ToJson(Active));
            Active = null;
            GC.Collect();
        }
        
        private static void DisposeMeasurements()
        {
            for (var i = 0; i < Disposables.Count; i++)
            {
                Disposables[i].Dispose();
            }

            Disposables.Clear();
        }

        public static SampleGroup GetSampleGroup(SampleGroupDefinition definition)
        {
            if (Active == null) throw new PerformanceTestException("Trying to record samples but there is no active performance tests.");
            foreach (var sampleGroup in Active.SampleGroups)
            {
                if (sampleGroup.Definition.Name == definition.Name)
                    return sampleGroup;
            }

            return null;
        }

        public void CalculateStatisticalValues()
        {
            foreach (var sampleGroup in SampleGroups)
            {
                CalculateStatisticalValue(sampleGroup);
            }
        }

        private static void CalculateStatisticalValue(SampleGroup sampleGroup)
        {
            if (sampleGroup.Samples == null) return;
            var samples = sampleGroup.Samples;
            if (samples.Count < 2)
            {
                sampleGroup.Min = samples[0];
                sampleGroup.Max = samples[0];
                sampleGroup.Median = samples[0];
                sampleGroup.Average = samples[0];
                sampleGroup.PercentileValue = 0.0D;
                sampleGroup.Zeroes = Utils.GetZeroValueCount(samples);
                sampleGroup.SampleCount = sampleGroup.Samples.Count;
                sampleGroup.Sum = samples[0];
                sampleGroup.StandardDeviation = 0;
            }
            else
            {
                sampleGroup.Min = Utils.Min(samples);
                sampleGroup.Max = Utils.Max(samples);
                sampleGroup.Median = Utils.GetMedianValue(samples);
                sampleGroup.Average = Utils.Average(samples);
                sampleGroup.PercentileValue = Utils.GetPercentile(samples, sampleGroup.Definition.Percentile);
                sampleGroup.Zeroes = Utils.GetZeroValueCount(samples);
                sampleGroup.SampleCount = sampleGroup.Samples.Count;
                sampleGroup.Sum = Utils.Sum(samples);
                sampleGroup.StandardDeviation = Utils.GetStandardDeviation(samples, sampleGroup.Average);
            }
        }

        private void LogOutput()
        {
            TestContext.Out.WriteLine(ToString());
        }

        public override string ToString()
        {
            var logString = new StringBuilder();

            foreach (var sampleGroup in SampleGroups)
            {
                logString.Append(sampleGroup.Definition.Name);

                if (sampleGroup.Samples.Count == 1)
                {
                    logString.AppendFormat(" {0:0.00} {1}", sampleGroup.Samples[0],
                        sampleGroup.Definition.SampleUnit);
                }
                else
                {
                    logString.AppendFormat(
                        " {0} Median:{1:0.00} Min:{2:0.00} Max:{3:0.00} Avg:{4:0.00} Std:{5:0.00} Zeroes:{6} SampleCount: {7} Sum: {8:0.00}",
                        sampleGroup.Definition.SampleUnit, sampleGroup.Median, sampleGroup.Min, sampleGroup.Max,
                        sampleGroup.Average,
                        sampleGroup.StandardDeviation, sampleGroup.Zeroes, sampleGroup.SampleCount, sampleGroup.Sum
                    );
                }

                logString.Append("\n");
            }

            return logString.ToString();
        }

        private static double? GetAggregationValue(SampleGroup sampleGroup)
        {
            switch (sampleGroup.Definition.AggregationType)
            {
                case AggregationType.Average:
                    return sampleGroup.Average;
                case AggregationType.Min:
                    return sampleGroup.Min;
                case AggregationType.Max:
                    return sampleGroup.Max;
                case AggregationType.Median:
                    return sampleGroup.Median;
                case AggregationType.Percentile:
                    return sampleGroup.PercentileValue;
                default:
                    throw new ArgumentOutOfRangeException("sampleGroup");
            }
        }
    }
}
