using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.Serialization.Json;
using UnityEngine;

namespace Unity.Serialization.Tests
{
    [TestFixture]
    class NodeParserTests
    {
        class NodeComparer : IEqualityComparer<NodeType>
        {
            public bool Equals(NodeType x, NodeType y)
            {
                Assert.AreEqual(x, y);
                return x == y;
            }

            public int GetHashCode(NodeType obj)
            {
                return obj.GetHashCode();
            }
        }
        
        static IEnumerable<NodeType> StepNodes(string json)
        {
            using (var tokenizer = new JsonTokenizer())
            using (var parser = new NodeParser(tokenizer))
            {
                // Tokenize the entire input data.
                Write(tokenizer, json);

                // Read until we have no more input.
                while (parser.TokenNextIndex < tokenizer.TokenNextIndex)
                {
                    var node = parser.Step();

                    if (node == NodeType.None)
                    {
                        continue;
                    }
                    
                    yield return node;
                }
                
                // Flush the parser.
                while (parser.NodeType != NodeType.None)
                {
                    yield return parser.Step();
                }
            }
        }

        static IEnumerable<NodeType> StepNodes(IEnumerable<string> parts)
        {
            using (var tokenizer = new JsonTokenizer())
            using (var parser = new NodeParser(tokenizer))
            {
                foreach (var json in parts)
                {
                    // Tokenize a part of the input data.
                    Write(tokenizer, json);

                    // Read until we consume all input data.
                    while (parser.TokenNextIndex < tokenizer.TokenNextIndex)
                    {
                        var node = parser.Step();

                        if (node == NodeType.None)
                        {
                            continue;
                        }
                        
                        yield return node;
                    }
                }

                // Flush the parser.
                while (parser.NodeType != NodeType.None)
                {
                    yield return parser.Step();
                }
            }
        }

        static void Write(ITokenizer tokenizer, string json)
        {
            unsafe
            {
                fixed (char* ptr = json)
                {
                    tokenizer.Write(new UnsafeBuffer<char> {Buffer = ptr, Length = json.Length}, 0, json.Length);
                }
            }
        }

        /// <summary>
        /// Tests the parsers output against expected results.
        /// </summary>
        [Test]
        [TestCase(@"{}", new[] { NodeType.BeginObject, NodeType.EndObject, NodeType.None })]
        [TestCase(@"[]", new[] { NodeType.BeginArray, NodeType.EndArray, NodeType.None })]
        [TestCase(@"[1,2]", new[] { NodeType.BeginArray, NodeType.Primitive, NodeType.Primitive, NodeType.EndArray, NodeType.None })]
        public void NodeParser_Step(string json, NodeType[] expected)
        {
            Assert.IsTrue(expected.SequenceEqual(StepNodes(json), new NodeComparer()));
        }
        
        /// <summary>
        /// Tests the parsers output against expected results when streaming.
        /// </summary>
        [Test]
        [TestCase(new []{@"{", "}"}, new[] { NodeType.BeginObject, NodeType.EndObject, NodeType.None })]
        [TestCase(new []{@"[", "]"}, new[] { NodeType.BeginArray, NodeType.EndArray, NodeType.None })]
        [TestCase(new []{@"[", "1", ",2", "]"}, new[] { NodeType.BeginArray, NodeType.Primitive, NodeType.Primitive, NodeType.EndArray, NodeType.None })]
        public void NodeParser_Step_Streamed(string[] json, NodeType[] expected)
        {
            Assert.IsTrue(expected.SequenceEqual(StepNodes(json), new NodeComparer()));
        }
    }
}