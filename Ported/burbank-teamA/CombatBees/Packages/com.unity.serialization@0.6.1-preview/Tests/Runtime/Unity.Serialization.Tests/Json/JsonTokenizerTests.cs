using NUnit.Framework;
using Unity.Collections;
using UnityEngine;

namespace Unity.Serialization.Json.Tests
{
    [TestFixture]
    class JsonTokenizerTests
    {
        [Test]
        [TestCase("{}")]
        [TestCase("{ }")]
        [TestCase(" \n{ \t}")]
        public unsafe void JsonTokenizer_Write_EmptyObject(string json)
        {
            fixed (char* ptr = json)
            {
                using (var tokenizer = new JsonTokenizer())
                {
                    tokenizer.Write(new UnsafeBuffer<char> {Buffer = ptr, Length = json.Length}, 0, json.Length);

                    Assert.AreEqual(1, tokenizer.TokenNextIndex);
                    Assert.AreEqual(TokenType.Object, tokenizer.Tokens[0].Type);
                    Assert.AreEqual(-1, tokenizer.Tokens[0].Parent);
                    Assert.AreNotEqual(-1, tokenizer.Tokens[0].End);
                }
            }
        }

        [Test]
        [TestCase("[]")]
        [TestCase("[ ]")]
        [TestCase(" \n[ \t]")]
        public unsafe void JsonTokenizer_Write_EmptyArray(string json)
        {
            using (var tokenizer = new JsonTokenizer())
            {
                fixed (char* ptr = json)
                {
                    tokenizer.Write(new UnsafeBuffer<char> {Buffer = ptr, Length = json.Length}, 0, json.Length);
                }

                Assert.AreEqual(1, tokenizer.TokenNextIndex);
                Assert.AreEqual(TokenType.Array, tokenizer.Tokens[0].Type);
                Assert.AreEqual(-1, tokenizer.Tokens[0].Parent);
                Assert.AreNotEqual(-1, tokenizer.Tokens[0].End);
            }
        }

        [Test]
        [TestCase(@"{""test"": 10}")]
        [TestCase(@"{""foo"": 0.0}")]
        public unsafe void JsonTokenizer_Write_ObjectWithMember(string json)
        {
            using (var tokenizer = new JsonTokenizer())
            {
                fixed (char* ptr = json)
                {
                    tokenizer.Write(new UnsafeBuffer<char> {Buffer = ptr, Length = json.Length}, 0, json.Length);
                }

                Assert.AreEqual(3, tokenizer.TokenNextIndex);
                Assert.AreEqual(TokenType.Object, tokenizer.Tokens[0].Type);
                Assert.AreNotEqual(-1, tokenizer.Tokens[0].End);
                Assert.AreEqual(TokenType.String, tokenizer.Tokens[1].Type);
                Assert.AreEqual(TokenType.Primitive, tokenizer.Tokens[2].Type);
            }
        }

        [Test]
        [TestCase(@"{""test"":""ab", @"c""}")]
        [TestCase(@"{""test"":""", @"abc""}")]
        [TestCase(@"{""test"":""abc", @"""}")]
        [TestCase(@"{""test"":""a", @"b", @"c""}")]
        public unsafe void JsonTokenizer_Write_PartialString(params object[] parts)
        {
            using (var tokenizer = new JsonTokenizer())
            {
                foreach (string json in parts)
                {
                    Assert.IsNotNull(json);

                    fixed (char* ptr = json)
                    {
                        tokenizer.Write(new UnsafeBuffer<char> {Buffer = ptr, Length = json.Length}, 0, json.Length);
                    }
                }

                Assert.AreEqual(parts.Length + 2, tokenizer.TokenNextIndex);
                Assert.AreEqual(TokenType.Object, tokenizer.Tokens[0].Type);
                Assert.AreEqual(TokenType.String, tokenizer.Tokens[1].Type);

                for (var i = 0; i < parts.Length; i++)
                {
                    var token = tokenizer.Tokens[i + 2];

                    Assert.AreEqual(i + 1, token.Parent);
                    Assert.AreEqual(TokenType.String, token.Type);

                    if (i == 0)
                    {
                        Assert.AreNotEqual(-1, token.Start);
                    }
                    else
                    {
                        Assert.AreEqual(-1, token.Start);
                    }

                    if (i == parts.Length - 1)
                    {
                        Assert.AreNotEqual(-1, token.End);
                    }
                    else
                    {
                        Assert.AreEqual(-1, token.End);
                    }
                }
            }
        }

        [Test]
        [TestCase(@"{""test"": 1", @"23 }")]
        [TestCase(@"{""test"": 12", @"3 }")]
        [TestCase(@"{""test"": 1", "2", @"3 }")]
        public unsafe void JsonTokenizer_Write_PartialNumber(params object[] parts)
        {
            using (var tokenizer = new JsonTokenizer())
            {
                foreach (string json in parts)
                {
                    Assert.IsNotNull(json);

                    fixed (char* ptr = json)
                    {
                        tokenizer.Write(new UnsafeBuffer<char> {Buffer = ptr, Length = json.Length}, 0, json.Length);
                    }
                }

                Assert.AreEqual(parts.Length + 2, tokenizer.TokenNextIndex);
                Assert.AreEqual(TokenType.Object, tokenizer.Tokens[0].Type);
                Assert.AreEqual(TokenType.String, tokenizer.Tokens[1].Type);

                for (var i = 0; i < parts.Length; i++)
                {
                    var token = tokenizer.Tokens[i + 2];

                    Assert.AreEqual(i + 1, token.Parent);
                    Assert.AreEqual(TokenType.Primitive, token.Type);

                    if (i == 0)
                    {
                        Assert.AreNotEqual(-1, token.Start);
                    }
                    else
                    {
                        Assert.AreEqual(-1, token.Start);
                    }

                    if (i == parts.Length - 1)
                    {
                        Assert.AreNotEqual(-1, token.End);
                    }
                    else
                    {
                        Assert.AreEqual(-1, token.End);
                    }
                }
            }
        }

        [Test]
        [TestCase(@"{""te",@"st""", @": 42}")]
        [TestCase(@"{""",@"test""", @": 42}")]
        [TestCase(@"{""test",@"""", @": 42}")]
        [TestCase(@"{""t","e","s",@"t""", @": 42}")]
        public unsafe void JsonTokenizer_Write_PartialKey(params string[] parts)
        {
            using (var tokenizer = new JsonTokenizer())
            {
                foreach (var json in parts)
                {
                    fixed (char* ptr = json)
                    {
                        tokenizer.Write(new UnsafeBuffer<char> {Buffer = ptr, Length = json.Length}, 0, json.Length);
                    }
                }

                Assert.AreEqual(parts.Length + 1, tokenizer.TokenNextIndex);
                Assert.AreEqual(TokenType.Object, tokenizer.Tokens[0].Type);

                for (var i = 0; i < parts.Length - 1; i++)
                {
                    var token = tokenizer.Tokens[i + 1];

                    Assert.AreEqual(i, token.Parent);
                    Assert.AreEqual(TokenType.String, token.Type);

                    if (i == 0)
                    {
                        Assert.AreNotEqual(-1, token.Start);
                    }
                    else
                    {
                        Assert.AreEqual(-1, token.Start);
                    }

                    if (i == parts.Length - 2)
                    {
                        Assert.AreNotEqual(-1, token.End);
                    }
                    else
                    {
                        Assert.AreEqual(-1, token.End);
                    }
                }
            }
        }

        [Test]
        [TestCase(@"{""foo"": 123, ""bar"": 456}", 5, 3)]
        public unsafe void JsonTokenizer_DiscardCompleted(string json, int expectedCountBeforeDiscard, int expectedCountAfterDiscard)
        {
            using (var tokenizer = new JsonTokenizer())
            {
                fixed (char* ptr = json)
                {
                    tokenizer.Write(new UnsafeBuffer<char> {Buffer = ptr, Length = json.Length}, 0, json.Length);
                }

                Assert.AreEqual(expectedCountBeforeDiscard, tokenizer.TokenNextIndex);

                tokenizer.DiscardCompleted();

                Assert.AreEqual(expectedCountAfterDiscard, tokenizer.TokenNextIndex);
            }
        }

        [Test]
        [TestCase(new []{@"{""tes", @"t"": 123, ""bar"": 456}"}, 6, 3)]
        [TestCase(new []{@"{""test"": 123, ""b", @"ar"": 456}"}, 6, 4)]
        [TestCase(new []{@"{""test"": 123, ""b", @"ar"": 456}"}, 6, 4)]
        [TestCase(new []{@"{""test"": a", "b",@"c"" "}, 1, 2)]
        public unsafe void JsonTokenizer_DiscardCompleted_Parts(string[] parts, int expectedCountBeforeDiscard, int expectedCountAfterDiscard)
        {
            using (var tokenizer = new JsonTokenizer())
            {
                foreach (var json in parts)
                {
                    fixed (char* ptr = json)
                    {
                        tokenizer.Write(new UnsafeBuffer<char> {Buffer = ptr, Length = json.Length}, 0, json.Length);
                    }
                }

                Print(tokenizer);
                Debug.Log(tokenizer.TokenParentIndex);
                // Assert.AreEqual(expectedCountBeforeDiscard, tokenizer.TokenNextIndex);
                tokenizer.DiscardCompleted();
                Print(tokenizer);
                Debug.Log(tokenizer.TokenParentIndex);
                //Assert.AreEqual(expectedCountAfterDiscard, tokenizer.TokenNextIndex);
            }
        }

        [Test]
        public unsafe void JsonTokenizer_Write_TokenBufferOverflow_Throws()
        {
            const string json = @"{""foo"": 123, ""bar"": 456}";

            using (var tokenizer = new JsonTokenizer(4) { AllowTokenBufferResize = false })
            {
                Assert.Throws<BufferOverflowException>(() =>
                {
                    fixed (char* ptr = json)
                    {
                        tokenizer.Write(new UnsafeBuffer<char> {Buffer = ptr, Length = json.Length}, 0, json.Length);
                    }
                });
            }
        }

        [Test]
        public unsafe void JsonTokenizer_Write_TokenBufferOverflow_DoesNotThrow()
        {
            const string json = @"{""foo"": 123, ""bar"": 456}";

            using (var tokenizer = new JsonTokenizer(4) { AllowTokenBufferResize = true })
            {
                Assert.DoesNotThrow(() =>
                {
                    fixed (char* ptr = json)
                    {
                        tokenizer.Write(new UnsafeBuffer<char> {Buffer = ptr, Length = json.Length}, 0, json.Length);
                    }
                });
            }
        }

        [Test]
        [TestCase(@"{}}")]
        public unsafe void JsonTokenizer_Write_InvalidJson(string json)
        {
            using (var tokenizer = new JsonTokenizer(4) { AllowTokenBufferResize = true })
            {
                Assert.Throws<InvalidJsonException>(() =>
                {
                    fixed (char* ptr = json)
                    {
                        tokenizer.Write(new UnsafeBuffer<char> {Buffer = ptr, Length = json.Length}, 0, json.Length);
                    }
                });
            }
        }

        void Print(JsonTokenizer tokenizer)
        {
            Print(tokenizer.Tokens, tokenizer.TokenNextIndex);
        }

        void Print(NativeArray<Token> tokens, int count)
        {
            for (var i = 0; i < count; i++)
            {
                Debug.Log($"[{i}] {tokens[i]}");
            }
        }
    }
}
