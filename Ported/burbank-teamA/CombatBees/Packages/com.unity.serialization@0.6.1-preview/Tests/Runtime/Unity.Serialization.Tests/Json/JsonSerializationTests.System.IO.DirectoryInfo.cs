using NUnit.Framework;
using System.IO;
using UnityEngine;

namespace Unity.Serialization.Json.Tests
{
    partial class JsonSerializationTests
    {
        const string k_AbsoluteDirectory = "/Absolute/Path/With Spaces";
        const string k_RelativeDirectory = "Relative/Path/With Spaces";

        class SystemIODirectoryInfoContainer
        {
            public DirectoryInfo AbsoluteDirectory;
            public DirectoryInfo RelativeDirectory;
        }

        [Test]
        public void JsonSerialization_Serialize_SystemIODirectoryInfo()
        {
            var src = new SystemIODirectoryInfoContainer
            {
                AbsoluteDirectory = new DirectoryInfo(k_AbsoluteDirectory),
                RelativeDirectory = new DirectoryInfo(k_RelativeDirectory)
            };

            var json = JsonSerialization.Serialize(src);
            Debug.Log(json);
            Assert.That(json, Does.Contain(k_AbsoluteDirectory + '\"'));
            Assert.That(json, Does.Contain('\"' + k_RelativeDirectory + '\"'));

            var dst = new SystemIODirectoryInfoContainer();
            using (JsonSerialization.DeserializeFromString(json, ref dst))
            {
                Assert.That(dst.AbsoluteDirectory, Is.Not.Null);
                Assert.That(dst.AbsoluteDirectory.FullName, Is.EqualTo(src.AbsoluteDirectory.FullName));
                Assert.That(dst.RelativeDirectory, Is.Not.Null);
                Assert.That(dst.RelativeDirectory.FullName, Is.EqualTo(src.RelativeDirectory.FullName));
            }
        }
    }
}
