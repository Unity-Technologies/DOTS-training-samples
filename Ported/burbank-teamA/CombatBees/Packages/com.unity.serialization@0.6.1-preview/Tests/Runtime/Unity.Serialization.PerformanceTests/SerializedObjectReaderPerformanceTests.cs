using System.IO;
using System.Linq;
using NUnit.Framework;
using Unity.PerformanceTesting;
using Unity.Serialization.Json;
using UnityEngine;

namespace Unity.Serialization.PerformanceTests
{
    [TestFixture]
    class SerializedObjectReaderPerformanceTests
    {
        #if UNITY_2019_2_OR_NEWER
        [Test, Performance]
        #else
        [PerformanceTest]
        #endif
        [TestCase(100, 10)]
        [TestCase(1000, 100)]
        [TestCase(10000, 500)]
        public unsafe void PerformanceTest_SerializedObjectReader_Read_MockEntities(int count, int batchSize)
        {
            File.WriteAllText("test.json", JsonTestData.GetMockEntities(count));

            try
            {
                Measure.Method(() =>
                       {
                           var views = stackalloc SerializedValueView[batchSize];

                           var config = SerializedObjectReaderConfiguration.Default;

                           config.BlockBufferSize = 512 << 10;
                           config.NodeBufferSize = batchSize;
                           config.ValidationType = JsonValidationType.None;
                           config.UseReadAsync = true;
                           config.OutputBufferSize = 4096 << 10;

                           using (var stream = new FileStream("test.json", FileMode.Open, FileAccess.Read, FileShare.Read, config.BlockBufferSize, FileOptions.Asynchronous))
                           using (var reader = new SerializedObjectReader(stream, config))
                           {
                               reader.Step();

                               while (reader.ReadArrayElementBatch(views, batchSize) != 0)
                               {
                                   reader.DiscardCompleted();
                               }

                               reader.Step();
                           }
                       })
                       .Definition("SerializedObjectReaderRead")
                       .WarmupCount(1)
                       .MeasurementCount(100)
                       .Run();

                PerformanceTest.Active.CalculateStatisticalValues();

                var size = new FileInfo("test.json").Length / (double) 1024 / 1024;
                Debug.Log($"MB/s=[{size / (PerformanceTest.Active.SampleGroups.First().Median / 1000)}]");
            }
            finally
            {
                File.Delete("test.json");
            }
        }
    }
}
