using NUnit.Framework;
using System;
using UnityEngine;

namespace Unity.Serialization.Json.Tests
{
    partial class JsonSerializationTests
    {
        class SystemGuidContainer
        {
            public Guid Value;
        }

        [Test]
        public void JsonSerialization_Serialize_SystemGuid()
        {
            var src = new SystemGuidContainer { Value = Guid.NewGuid() };
            var json = JsonSerialization.Serialize(src);
            Debug.Log(json);
            Assert.That(json, Does.Match(@".*""[\da-f]{32}"".*"));

            var dst = new SystemGuidContainer();
            using (JsonSerialization.DeserializeFromString(json, ref dst))
            {
                Assert.That(dst.Value, Is.EqualTo(src.Value));
            }
        }
    }
}
