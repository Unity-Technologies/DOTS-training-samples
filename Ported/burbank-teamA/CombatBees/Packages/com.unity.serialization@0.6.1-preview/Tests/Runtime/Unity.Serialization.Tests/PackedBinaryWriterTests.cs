using NUnit.Framework;
using Unity.Collections;
using Unity.Serialization.Json;
using UnityEngine;

namespace Unity.Serialization.Tests
{
    [TestFixture]
    class PackedBinaryWriterTests
    {
        [Test]
        [TestCase(@"{}")]
        [TestCase(@"{""foo"": {}, ""bar"": ""hello world""}")]
        public unsafe void PackedBinaryWriter_Write(string json)
        {
            using (var tokenizer = new JsonTokenizer(Allocator.TempJob))
            using (var stream = new PackedBinaryStream(Allocator.TempJob))
            using (var writer = new PackedBinaryWriter(stream, tokenizer))
            {
                fixed (char* ptr = json)
                {
                    var buffer = new UnsafeBuffer<char>
                    {
                        Buffer = ptr, Length = json.Length
                    };

                    tokenizer.Write(buffer, 0, json.Length);
                    writer.Write(buffer, tokenizer.TokenNextIndex);
                }
            }
        }

        [Test]
        [TestCase(@"{""t","e",@"st""", @":""a", @"b", @"c""}")]
        public unsafe void PackedBinaryWriter_Write_PartialKey(params object[] parts)
        {
            using (var tokenizer = new JsonTokenizer(Allocator.TempJob))
            using (var stream = new PackedBinaryStream(Allocator.TempJob))
            using (var writer = new PackedBinaryWriter(stream, tokenizer))
            {
                foreach (string json in parts)
                {
                    fixed (char* ptr = json)
                    {
                        var buffer = new UnsafeBuffer<char> {Buffer = ptr, Length = json.Length};
                        tokenizer.Write(buffer, 0, json.Length);
                        writer.Write(buffer, tokenizer.TokenNextIndex);
                    }
                }

                stream.DiscardCompleted();
            }

        }
    }
}
