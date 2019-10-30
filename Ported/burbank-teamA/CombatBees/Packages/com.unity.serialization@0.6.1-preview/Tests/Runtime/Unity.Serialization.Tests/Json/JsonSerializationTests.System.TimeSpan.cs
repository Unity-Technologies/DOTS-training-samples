using NUnit.Framework;
using System;
using UnityEngine;

namespace Unity.Serialization.Json.Tests
{
    partial class JsonSerializationTests
    {
        class SystemTimeSpanContainer
        {
            public TimeSpan Value;
        }

        [Test]
        public void JsonSerialization_Serialize_SystemTimeSpan()
        {
            var src = new SystemTimeSpanContainer { Value = new TimeSpan(1, 2, 3, 4, 5) };
            var json = JsonSerialization.Serialize(src);
            Debug.Log(json);
            Assert.That(json, Does.Match(@".*""(-?\d+[.])?\d{2}:\d{2}:\d{2}([.]\d{7})?"".*"));

            var dst = new SystemTimeSpanContainer();
            using (JsonSerialization.DeserializeFromString(json, ref dst))
            {
                Assert.That(dst.Value, Is.EqualTo(src.Value));
            }
        }
    }
}
