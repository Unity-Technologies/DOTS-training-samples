using System.Linq;
using NUnit.Framework;
using Unity.Collections;
using Unity.PerformanceTesting;
using Unity.Serialization.Json;
using UnityEngine;

namespace Unity.Serialization.PerformanceTests
{
    [TestFixture]
    [Category("Performance")]
    class PackedBinaryWriterPerformanceTests
    {
        JsonTokenizer m_Tokenizer;

        [SetUp]
        public void SetUp()
        {
            m_Tokenizer = new JsonTokenizer();
        }

        [TearDown]
        public void TearDown()
        {
            m_Tokenizer.Dispose();
        }

        #if UNITY_2019_2_OR_NEWER
        [Test, Performance]
        #else
        [PerformanceTest]
        #endif
        [TestCase(1000)]
        [TestCase(10000)]
        public unsafe void PerformanceTest_PackedBinaryWriter_Write_MockEntities(int count)
        {
            var json = JsonTestData.GetMockEntities(count);

            fixed (char* ptr = json)
            {
                m_Tokenizer.Write(new UnsafeBuffer<char> {Buffer = ptr, Length = json.Length}, 0, json.Length);
            }

            Measure.Method(() =>
                   {
                       using (var stream = new PackedBinaryStream(Allocator.TempJob))
                       using (var writer = new PackedBinaryWriter(stream, m_Tokenizer))
                       {
                           fixed (char* ptr = json)
                           {
                               writer.Write(new UnsafeBuffer<char> {Buffer = ptr, Length = json.Length}, m_Tokenizer.TokenNextIndex);
                           }
                       }
                   })
                   .Definition("PackedBinaryWriterWrite")
                   .WarmupCount(1)
                   .MeasurementCount(100)
                   .Run();

            PerformanceTest.Active.CalculateStatisticalValues();

            var size = json.Length / (double) 1024 / 1024;
            Debug.Log($"MB/s=[{size / (PerformanceTest.Active.SampleGroups.First().Median / 1000)}]");
        }
    }
}
