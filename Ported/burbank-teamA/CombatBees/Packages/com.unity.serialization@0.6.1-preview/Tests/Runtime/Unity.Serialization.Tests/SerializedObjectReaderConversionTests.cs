using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Unity.Serialization.Json;

namespace Unity.Serialization.Tests
{
    [TestFixture]
    public class SerializedObjectReaderConversionTests
    {
        MemoryStream m_Stream;
        SerializedObjectReaderConfiguration m_ConfigWithNoValidation;

        [SetUp]
        public void SetUp()
        {
            m_Stream = new MemoryStream();
            m_ConfigWithNoValidation = SerializedObjectReaderConfiguration.Default;
            m_ConfigWithNoValidation.ValidationType = JsonValidationType.None;
        }

        void SetJson(string json)
        {
            m_Stream.Seek(0, SeekOrigin.Begin);

            using (var writer = new StreamWriter(m_Stream, Encoding.UTF8, 1024, true))
            {
                writer.Write(json);
            }

            m_Stream.Seek(0, SeekOrigin.Begin);
        }

        [Test]
        [TestCase(@"{x:0}", 0)]
        [TestCase(@"{x:-0}", 0)]
        [TestCase(@"{x:-1}", -1)]
        [TestCase(@"{x:100}", 100)]
        [TestCase(@"{x:-100}", -100)]
        [TestCase(@"{x:2147483647}", int.MaxValue)]
        [TestCase(@"{x:-2147483648}", int.MinValue)]
        [TestCase(@"{x:9223372036854775807}", long.MaxValue)]
        [TestCase(@"{x:-9223372036854775808}", long.MinValue)]
        public void SerializedObjectReader_Convert_Int64(string json, long expected)
        {
            SetJson(json);

            using (var reader = new SerializedObjectReader(m_Stream, m_ConfigWithNoValidation))
            {
                var obj = reader.ReadObject();
                var value = obj["x"];
                Assert.AreEqual(expected, value.AsInt64());
            }
        }

        [Test]
        [TestCase(@"{x:9223372036854775808}")]
        [TestCase(@"{x:-9223372036854775809}")]
        public void SerializedObjectReader_Convert_Int64_Throws(string json)
        {
            SetJson(json);

            using (var reader = new SerializedObjectReader(m_Stream, m_ConfigWithNoValidation))
            {
                var obj = reader.ReadObject();
                var value = obj["x"];
                Assert.Throws<ParseErrorException>(() => { value.AsInt64(); });
            }
        }

        [Test]
        [TestCase(@"{x:0}", 0ul)]
        [TestCase(@"{x:100}", 100ul)]
        [TestCase(@"{x:2147483647}",  (ulong) int.MaxValue)]
        [TestCase(@"{x:9223372036854775807}", (ulong) long.MaxValue)]
        [TestCase(@"{x:18446744073709551615}", ulong.MaxValue)]
        public void SerializedObjectReader_Convert_UInt64(string json, ulong expected)
        {
            SetJson(json);

            using (var reader = new SerializedObjectReader(m_Stream, m_ConfigWithNoValidation))
            {
                var obj = reader.ReadObject();
                var value = obj["x"];
                Assert.AreEqual(expected, value.AsUInt64());
            }
        }

        [Test]
        [TestCase(@"{x:-0}")]
        [TestCase(@"{x:-1}")]
        [TestCase(@"{x:-100}")]
        [TestCase(@"{x:-2147483647}")]
        [TestCase(@"{x:-9223372036854775808}")]
        [TestCase(@"{x:18446744073709551616}")]
        public void SerializedObjectReader_Convert_UInt64_Throws(string json)
        {
            SetJson(json);

            using (var reader = new SerializedObjectReader(m_Stream, m_ConfigWithNoValidation))
            {
                var obj = reader.ReadObject();
                var value = obj["x"];
                Assert.Throws<ParseErrorException>(() => { value.AsUInt64(); });
            }
        }

        [Test]
        [TestCase(@"{x:0}", 0f)]
        [TestCase(@"{x:-0}", 0f)]
        [TestCase(@"{x:-1}", -1f)]
        [TestCase(@"{x:100}", 100f)]
        [TestCase(@"{x:-100}", -100f)]
        [TestCase(@"{x:2147483647}", (float) int.MaxValue)]
        [TestCase(@"{x:-2147483648}", (float) int.MinValue)]
        [TestCase(@"{x:9223372036854775808}", (float) long.MaxValue)]
        [TestCase(@"{x:-9223372036854775808}", (float) long.MinValue)]
        public void SerializedObjectReader_Convert_Float32(string json, float expected)
        {
            SetJson(json);

            using (var reader = new SerializedObjectReader(m_Stream, m_ConfigWithNoValidation))
            {
                var obj = reader.ReadObject();
                var value = obj["x"];
                Assert.AreEqual(expected, value.AsFloat());
            }
        }

        [Test]
        [TestCase(@"{x:0}", 0d)]
        [TestCase(@"{x:-0}", 0d)]
        [TestCase(@"{x:-1}", -1d)]
        [TestCase(@"{x:100}", 100d)]
        [TestCase(@"{x:-100}", -100d)]
        [TestCase(@"{x:2147483647}", (double) int.MaxValue)]
        [TestCase(@"{x:-2147483648}", (double) int.MinValue)]
        [TestCase(@"{x:9223372036854775808}", (double) long.MaxValue)]
        [TestCase(@"{x:-9223372036854775808}", (double) long.MinValue)]
        public void SerializedObjectReader_Convert_Float64(string json, double expected)
        {
            SetJson(json);

            using (var reader = new SerializedObjectReader(m_Stream, m_ConfigWithNoValidation))
            {
                var obj = reader.ReadObject();
                var value = obj["x"];
                Assert.AreEqual(expected, value.AsDouble());
            }
        }

        [Test]
        [TestCase(@"{x:Infinity}", float.PositiveInfinity)]
        [TestCase(@"{x:-Infinity}", float.NegativeInfinity)]
        [TestCase(@"{x:NaN}", float.NaN)]
        public void SerializedObjectReader_Convert_SpecialValues(string json, float expected)
        {
            SetJson(json);

            using (var reader = new SerializedObjectReader(m_Stream, m_ConfigWithNoValidation))
            {
                var obj = reader.ReadObject();
                var value = obj["x"];
                Assert.AreEqual(expected, value.AsFloat());
            }
        }
    }
}
