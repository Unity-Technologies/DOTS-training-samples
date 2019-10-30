using NUnit.Framework;
using UnityEngine;

namespace Unity.Serialization.Json.Tests
{
    partial class JsonSerializationTests
    {
        class UnityEditorGUIDContainer
        {
            public UnityEditor.GUID Value;
        }

        [Test]
        public void JsonSerialization_Serialize_UnityEditorGUID()
        {
            var src = new UnityEditorGUIDContainer { Value = UnityEditor.GUID.Generate() };
            var json = JsonSerialization.Serialize(src);
            Debug.Log(json);
            Assert.That(json, Does.Match(@".*""[\da-f]{32}"".*"));

            var dst = new UnityEditorGUIDContainer();
            using (JsonSerialization.DeserializeFromString(json, ref dst))
            {
                Assert.That(dst.Value, Is.EqualTo(src.Value));
            }
        }
    }
}
