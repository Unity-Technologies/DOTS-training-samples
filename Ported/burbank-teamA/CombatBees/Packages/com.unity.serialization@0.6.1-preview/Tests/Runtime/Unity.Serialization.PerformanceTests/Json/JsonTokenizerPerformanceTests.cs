using System.Linq;
using NUnit.Framework;
using Unity.PerformanceTesting;
using Unity.Serialization.Json;
using UnityEngine;

namespace Unity.Serialization.PerformanceTests
{
    [TestFixture]
    [Category("Performance")]
    class JsonTokenizerPerformanceTests
    {
#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        [TestCase(100, 1024)]
        [TestCase(1000, 1024)]
        [TestCase(10000, 1024)]
        public unsafe void PerformanceTest_JsonTokenizer_WriteWithNoValidation_MockEntities(int count, int initialTokenBuffer)
        {
            var json = JsonTestData.GetMockEntities(count);

            Measure.Method(() =>
                   {
                       fixed (char* ptr = json)
                       {
                           using (var tokenizer = new JsonTokenizer(initialTokenBuffer, JsonValidationType.None) { AllowTokenBufferResize = true })
                           {
                               tokenizer.Write(new UnsafeBuffer<char> {Buffer = ptr, Length = json.Length}, 0, json.Length);
                           }
                       }
                   })
                   .Definition("JsonTokenizerWrite")
                   .WarmupCount(1)
                   .MeasurementCount(100)
                   .Run();

            PerformanceTest.Active.CalculateStatisticalValues();

            var size = json.Length / (double) 1024 / 1024;
            Debug.Log($"MB/s=[{size / (PerformanceTest.Active.SampleGroups.First().Median / 1000)}]");
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        [TestCase(100, 1024)]
        [TestCase(1000, 1024)]
        [TestCase(10000, 1024)]
        public unsafe void PerformanceTest_JsonTokenizer_WriteWithStandardValidation_MockEntities(int count, int initialTokenBuffer)
        {
            var json = JsonTestData.GetMockEntities(count);

            Measure.Method(() =>
                   {
                       fixed (char* ptr = json)
                       {
                           using (var tokenizer = new JsonTokenizer(initialTokenBuffer, JsonValidationType.Standard) { AllowTokenBufferResize = true })
                           {
                               tokenizer.Write(new UnsafeBuffer<char> {Buffer = ptr, Length = json.Length}, 0, json.Length);
                           }
                       }
                   })
                   .Definition("JsonTokenizerWrite")
                   .WarmupCount(1)
                   .MeasurementCount(100)
                   .Run();

            PerformanceTest.Active.CalculateStatisticalValues();

            var size = json.Length / (double) 1024 / 1024;
            Debug.Log($"MB/s=[{size / (PerformanceTest.Active.SampleGroups.First().Median / 1000)}]");
        }
    }
}
