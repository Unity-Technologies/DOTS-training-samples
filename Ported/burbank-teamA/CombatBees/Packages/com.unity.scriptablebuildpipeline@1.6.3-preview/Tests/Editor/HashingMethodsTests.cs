using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Utilities;

namespace UnityEditor.Build.Pipeline.Tests
{
    [TestFixture]
    class HashingMethodsTests
    {
        [Test]
        public void CSharpMD4GeneratesCppMD4IdenticalFileNames()
        {
            var sourceNames = new[]
            {
                "basic_sprite",
                "audio",
                "prefabs",
                "shaderwithcollection",
                "multi_sprite_packed"
            };

            var expectedNames = new[]
            {
                "a67a7313ceb7840411094318a4aa7055",
                "d5ae2b5aa3edc0f73b4bb6b1ae125a53",
                "6fee5e41c4939eed80f81beb3e5e6ebc",
                "dc5d7d3a7d9efcf91a0314cdcc3af3c8",
                "ddc8dcea83a5ff418d94c6a1623e81ad"
            };

            for (var i = 0; i < 5; i++)
                Assert.AreEqual(expectedNames[i], HashingMethods.Calculate<MD4>(sourceNames[i]).ToString());
        }
        
        [Test]
        public void CSharpMD4GeneratesCppMD4IdenticalFileIDs()
        {
            Assert.AreEqual(-7588530676450950513, BitConverter.ToInt64(HashingMethods.Calculate<MD4>("fb3a9882e5510684697de78116693750", FileType.MetaAssetType, (long)21300000).ToBytes(), 0));
            Assert.AreEqual(-8666180608703991793, BitConverter.ToInt64(HashingMethods.Calculate<MD4>("library/atlascache/27/2799803afb660251e3b3049ba37cb15a", (long)2).ToBytes(), 0));
        }
        
        [Test]
        public void HashingMethodsCanConsumeArrays()
        {
            var sourceNames = new[]
            {
                "basic_sprite",
                "audio",
                "prefabs",
                "shaderwithcollection",
                "multi_sprite_packed"
            };

            // Use cast so it doesn't automatically expand the array to params object[] objects
            Assert.AreEqual("99944412d5093e431ba7ccdaf48f44f3", HashingMethods.Calculate<MD4>((object)sourceNames).ToString());
        }
        
        [Test]
        public void HashingMethodsCanConsumeLists()
        {
            var sourceNames = new List<string>
            {
                "basic_sprite",
                "audio",
                "prefabs",
                "shaderwithcollection",
                "multi_sprite_packed"
            };
            
            Assert.AreEqual("99944412d5093e431ba7ccdaf48f44f3", HashingMethods.Calculate<MD4>(sourceNames).ToString());
        }
        
        [Test]
        public void HashingMethodsCanConsumeHashSet()
        {
            var sourceNames = new HashSet<string>
            {
                "basic_sprite",
                "audio",
                "prefabs",
                "shaderwithcollection",
                "multi_sprite_packed"
            };
            
            Assert.AreEqual("99944412d5093e431ba7ccdaf48f44f3", HashingMethods.Calculate<MD4>(sourceNames).ToString());
        }
        
        [Test]
        public void HashingMethodsCanConsumeDictionaries()
        {
            var sourceNames = new Dictionary<string, string>
            {
                { "basic_sprite", "audio" },
                { "prefabs", "shaderwithcollection" }
            };
            
            Assert.AreEqual("34392e04ec079d34cd861df956db2099", HashingMethods.Calculate<MD4>(sourceNames).ToString());
        }
        
        [Test]
        public void CalculateStreamCanUseOffsets()
        {
            byte[] bytes = { 0xe1, 0x43, 0x2f, 0x83, 0xdf, 0xeb, 0xa8, 0x86, 0xfb, 0xfe, 0xc9, 0x97, 0x20, 0xfb, 0x53, 0x45,
                             0x24, 0x5d, 0x92, 0x8b, 0xa2, 0xc4, 0xe1, 0xe2, 0x48, 0x4a, 0xbb, 0x66, 0x43, 0x9a, 0xbc, 0x84 };

            using (var stream = new MemoryStream(bytes))
            {
                stream.Position = 16;
                RawHash hash1 = HashingMethods.CalculateStream(stream);

                stream.Position = 0;
                RawHash hash2 = HashingMethods.CalculateStream(stream);

                Assert.AreNotEqual(hash1.ToString(), hash2.ToString());
            }
        }
    }
}