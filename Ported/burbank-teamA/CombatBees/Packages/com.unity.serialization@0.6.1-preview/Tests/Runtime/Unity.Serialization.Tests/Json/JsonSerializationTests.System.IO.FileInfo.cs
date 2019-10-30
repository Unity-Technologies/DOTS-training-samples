using NUnit.Framework;
using System.IO;
using UnityEngine;

namespace Unity.Serialization.Json.Tests
{
    partial class JsonSerializationTests
    {
        const string k_AbsoluteFile = "/Absolute/Path/With Spaces/Boring Text.txt";
        const string k_RelativeFile = "Relative/Path/With Spaces/My Awesome Texture.png";

        class SystemIOFileInfoContainer
        {
            public FileInfo AbsoluteFile;
            public FileInfo RelativeFile;
        }

        [Test]
        public void JsonSerialization_Serialize_SystemIOFileInfo()
        {
            var src = new SystemIOFileInfoContainer
            {
                AbsoluteFile = new FileInfo(k_AbsoluteFile),
                RelativeFile = new FileInfo(k_RelativeFile)
            };

            var json = JsonSerialization.Serialize(src);
            Debug.Log(json);
            Assert.That(json, Does.Contain(k_AbsoluteFile + '\"'));
            Assert.That(json, Does.Contain('\"' + k_RelativeFile + '\"'));

            var dst = new SystemIOFileInfoContainer();
            using (JsonSerialization.DeserializeFromString(json, ref dst))
            {
                Assert.That(dst.AbsoluteFile, Is.Not.Null);
                Assert.That(dst.AbsoluteFile.FullName, Is.EqualTo(src.AbsoluteFile.FullName));
                Assert.That(dst.RelativeFile, Is.Not.Null);
                Assert.That(dst.RelativeFile.FullName, Is.EqualTo(src.RelativeFile.FullName));
            }
        }
    }
}
