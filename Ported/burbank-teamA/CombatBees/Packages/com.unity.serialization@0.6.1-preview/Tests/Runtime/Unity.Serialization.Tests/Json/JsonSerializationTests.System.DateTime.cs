using NUnit.Framework;
using System;
using UnityEngine;

namespace Unity.Serialization.Json.Tests
{
    partial class JsonSerializationTests
    {
        class SystemDateTimeContainer
        {
            public DateTime Value;
        }

        [Test]
        public void JsonSerialization_Serialize_SystemDateTime()
        {
            var src = new SystemDateTimeContainer { Value = DateTime.Now };
            var json = JsonSerialization.Serialize(src);
            Debug.Log(json);
            Assert.That(json, Does.Match(@".*""\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{7}Z"".*"));

            var dst = new SystemDateTimeContainer();
            using (JsonSerialization.DeserializeFromString(json, ref dst))
            {
                Assert.That(dst.Value, Is.EqualTo(src.Value));
            }
        }
    }
}
