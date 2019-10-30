using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Unity.Serialization.Json.Tests
{
    [TestFixture]
    partial class JsonSerializationTests
    {
        struct TestStruct
        {
            public int A;
            public int B;
        }

        static string UnFormat(string json)
        {
            return Regex.Replace(json, @"(""[^""\\]*(?:\\.[^""\\]*)*"")|\s+", "$1");
        }

        [Test]
        public void JsonSerialization_SerializeAsString_Struct()
        {
            var src = new TestStruct { A = 10, B = 32 };
            var dst = new TestStruct();

            var json = JsonSerialization.Serialize(src);
            using (JsonSerialization.DeserializeFromString(json, ref dst))
            {
                Assert.That(src, Is.EqualTo(dst));
            }
        }

        [Test]
        public void JsonSerialization_SerializeToFile_Struct()
        {
            var src = new TestStruct { A = 10, B = 32 };
            var dst = new TestStruct();

            JsonSerialization.Serialize("test.json", src);
            using (JsonSerialization.DeserializeFromPath("test.json", ref dst))
            {
                Assert.That(src, Is.EqualTo(dst));
            }

            File.Delete("test.json");
        }
    }
}