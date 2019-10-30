using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Unity.Collections;
using Unity.Serialization.Json;
using UnityEngine;

namespace Unity.Serialization.Tests
{
    [TestFixture]
    public class SerializedObjectReaderTests
    {
        MemoryStream m_Stream;
        SerializedObjectReaderConfiguration m_ConfigWithNoValidation;

        [SetUp]
        public void SetUp()
        {
            m_Stream = new MemoryStream();
            m_ConfigWithNoValidation = SerializedObjectReaderConfiguration.Default;
            m_ConfigWithNoValidation.ValidationType = JsonValidationType.None;
        }

        void SetJson(string json)
        {
            m_Stream.Seek(0, SeekOrigin.Begin);

            using (var writer = new StreamWriter(m_Stream, Encoding.UTF8, 1024, true))
            {
                writer.Write(json);
            }

            m_Stream.Seek(0, SeekOrigin.Begin);
        }

        [Test]
        [TestCase("{}")]
        public void SerializedObjectReader_Step_EmptyObject(string json)
        {
            SetJson(json);

            using (var reader = new SerializedObjectReader(m_Stream, m_ConfigWithNoValidation))
            {
                Assert.AreEqual(NodeType.BeginObject, reader.Step(out var view));
                Assert.AreEqual(TokenType.Object,view.Type);
                Assert.AreEqual(NodeType.EndObject, reader.Step());
            }
        }

        [Test]
        [TestCase(@"{a:b}", 1)]
        [TestCase("{a:b, c:d}", 2)]
        public void SerializedObjectReader_Step_ObjectWithMember(string json, int expectedCount)
        {
            SetJson(json);

            using (var reader = new SerializedObjectReader(m_Stream, m_ConfigWithNoValidation))
            {
                Assert.AreEqual(NodeType.BeginObject, reader.Step(out var view));
                Assert.AreEqual(0,view.AsObjectView().Count());
                Assert.AreEqual(NodeType.EndObject, reader.Step(NodeType.EndObject));
                Assert.AreEqual(expectedCount,view.AsObjectView().Count());
            }
        }

        [Test]
        [TestCase("[]", new int[] {})]
        [TestCase("[1,2,3]", new [] { 1,2,3 })]
        [TestCase("[2 \n,5,7\n,9]", new [] { 2,5,7,9 })]
        public void SerializedObjectReader_ReadArrayElement_Numbers(string json, int[] expectedValues)
        {
            SetJson(json);

            using (var reader = new SerializedObjectReader(m_Stream, m_ConfigWithNoValidation))
            {
                Assert.AreEqual(NodeType.BeginArray, reader.Step());

                foreach (var expectedValue in expectedValues)
                {
                    reader.ReadArrayElement(out var element);
                    Assert.AreEqual(expectedValue, element.AsDouble());
                }

                Assert.AreEqual(NodeType.EndArray, reader.Step());
            }
        }

        [Test]
        [TestCase("[{\"foo\":10,\"bar\":\"test\"},{\"foo\":10,\"bar\":\"test\"}]")]
        public void SerializedObjectReader_ReadArrayElement_Objects(string json)
        {
            SetJson(json);

            using (var reader = new SerializedObjectReader(m_Stream, m_ConfigWithNoValidation))
            {
                Assert.AreEqual(NodeType.BeginArray, reader.Step());

                while (reader.ReadArrayElement(out var element))
                {
                    var obj = element.AsObjectView();

                    Assert.AreEqual(10, obj["foo"].AsInt64());
                    Assert.AreEqual("test", obj["bar"].AsStringView().ToString());
                }

                Assert.AreEqual(NodeType.EndArray, reader.Step());
            }
        }

        [Test]
        [TestCase("[]", 0, 0)]
        [TestCase("[]", 0, 1)]
        [TestCase("[1,2,3,4,5]", 5, 2)]
        [TestCase("[1,2,3,4,5]", 5, 3)]
        [TestCase("[1,2,3,4,5]", 5, 10)]
        [TestCase("[{a:[]},{a:[]},{a:[]},{a:[]},{a:[]}]", 5, 10)]
        [TestCase("[{a:b},{c:d,e:f},{},{},{}]", 5, 5)]
        [TestCase("[1,{},\"foo\"]", 3, 1)]
        public unsafe void SerializedObjectReader_ReadArrayElementBatch(string json, int expectedCount, int batchSize)
        {
            SetJson(json);

            using (var reader = new SerializedObjectReader(m_Stream, m_ConfigWithNoValidation))
            {
                Assert.AreEqual(NodeType.BeginArray, reader.Step());

                var views = stackalloc SerializedValueView[batchSize];

                var actualCount = 0;

                int count;
                while ((count = reader.ReadArrayElementBatch(views, batchSize)) != 0)
                {
                    actualCount += count;
                }

                Assert.AreEqual(expectedCount, actualCount);
                Assert.AreEqual(NodeType.EndArray, reader.Step());
            }
        }

        [Test]
        [TestCase("[]", 0)]
        [TestCase("[1,2,3,4,5]", 5)]
        [TestCase("[{a:b},{c:d,e:f},{},{},{}]", 5)]
        [TestCase("[1,{},\"foo\"]", 3)]
        public void SerializedObjectReader_Read_Array(string json, int expectedCount)
        {
            SetJson(json);

            using (var reader = new SerializedObjectReader(m_Stream, m_ConfigWithNoValidation))
            {
                reader.Read(out var view);

                Assert.AreEqual(TokenType.Array, view.Type);
                Assert.AreEqual(expectedCount, view.AsArrayView().Count());
            }
        }

        /// <summary>
        /// Testing a mock configuration file using sjson style syntax
        /// </summary>
        [Test]
        public void SerializedObjectReader_Read_MockManifest()
        {
            SetJson(@"
{
    hash = 17bc734727d742b091491895812a6cea
    files = [
        ""c:/path/to/assets/image.jpg""
        ""c:/path/to/assets/image_a.png""
    ]
}");

            using (var reader = new SerializedObjectReader(m_Stream, m_ConfigWithNoValidation))
            {
                var obj = reader.ReadObject();
                var hash = obj["hash"].AsStringView();

                Assert.IsTrue(hash.Equals("17bc734727d742b091491895812a6cea"));
                Assert.AreEqual("17bc734727d742b091491895812a6cea", hash.ToString());

                var files = obj["files"].AsArrayView();

                Assert.AreEqual(2, files.Count());
                Assert.IsTrue(files.Select(view => view.AsStringView().ToString()).SequenceEqual(new [] { "c:/path/to/assets/image.jpg", "c:/path/to/assets/image_a.png"}));
            }
        }

        /// <summary>
        /// Testing mock settings file from Tiny.
        /// </summary>
        [Test]
        public void SerializedObjectReader_Read_MockEditorAssetFile()
        {
            SetJson(@"
{
    uxml_inspectors =
    [
        {
            target = ""Unity.Tiny.Core2D.Translation, Unity.Tiny.Core2D, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null""
        },
        {
            target = ""Unity.Tiny.Core2D.Camera2D, Unity.Tiny.Core2D, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null""
            uxml = { guid = a384dee09c50b0c478ccc1064fc29d9a fileId = 9197481836555534336 type = 3 }
            uss = { guid = a384dee09c50b0c478ccc1064fc29d9a fileId = 9197481836555534336 type = 3 }
        }
    ],
    code_inspectors =
    [
        {
            target = ""Unity.Tiny.Core2D.Camera2D, Unity.Tiny.Core2D, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null""
            type = ""Unity.Tiny.Core2D.Camera2DInspector, Unity.Tiny.Core2D, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null""
        }
    ]
}");

            using (var reader = new SerializedObjectReader(m_Stream, m_ConfigWithNoValidation))
            {
                var obj = reader.ReadObject();
                var uxmlInspectors = obj["uxml_inspectors"].AsArrayView().ToArray();

                Assert.AreEqual(2, uxmlInspectors.Length);

                var localPositionUxmlInspector = uxmlInspectors[0].AsObjectView();
                Assert.IsTrue(localPositionUxmlInspector.TryGetValue("target", out var target));
                Assert.AreEqual("Unity.Tiny.Core2D.Translation, Unity.Tiny.Core2D, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", target.AsStringView());
                Assert.IsFalse(localPositionUxmlInspector.TryGetValue("uxml", out _));
                Assert.IsFalse(localPositionUxmlInspector.TryGetValue("uss", out _));

                var camera2DUxmlInspector = uxmlInspectors[1].AsObjectView();
                Assert.IsTrue(camera2DUxmlInspector.TryGetValue("target", out target));
                Assert.AreEqual("Unity.Tiny.Core2D.Camera2D, Unity.Tiny.Core2D, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", target.AsStringView());
                Assert.IsTrue(camera2DUxmlInspector.TryGetValue("uxml", out var uxml));
                Assert.AreEqual(TokenType.Object, uxml.Type);
                Assert.IsTrue(camera2DUxmlInspector.TryGetValue("uss", out var uss));
                Assert.AreEqual(TokenType.Object, uss.Type);

                var codeInspectors = obj["code_inspectors"].AsArrayView().ToArray();
                Assert.AreEqual(1, codeInspectors.Length);

                var camera2DCodeInspector = codeInspectors[0].AsObjectView();
                Assert.IsTrue(camera2DCodeInspector.TryGetValue("target", out target));
                Assert.AreEqual("Unity.Tiny.Core2D.Camera2D, Unity.Tiny.Core2D, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", target.AsStringView());
                Assert.IsTrue(camera2DCodeInspector.TryGetValue("type", out var type));
                Assert.AreEqual("Unity.Tiny.Core2D.Camera2DInspector, Unity.Tiny.Core2D, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", type.AsStringView());
            }
        }

        /// <summary>
        /// Tests reading some mock settings files
        /// </summary>
        [Test]
        public void SerializedObjectReader_Read_MockSettings()
        {
            using (var output = new PackedBinaryStream(Allocator.TempJob))
            using (var members = new SerializedMemberViewCollection(Allocator.TempJob))
            {
                SetJson(@"
foo = {
    a = 10
    b = 20
    c = ""hello""
}
bar = [1,2,3,4,5]
");
                using (var reader = new SerializedObjectReader(m_Stream, output, m_ConfigWithNoValidation))
                {
                    reader.ReadMember(members);
                    reader.ReadMember(members);
                }

                SetJson(@"
test = ""string""
color = { r = 1 g = 1 b = 1 a = 1 }
");

                using (var reader = new SerializedObjectReader(m_Stream, output, m_ConfigWithNoValidation))
                {
                    reader.ReadMember(members);
                    reader.ReadMember(members);
                }

                Assert.AreEqual(4, members.Count());

                Assert.IsTrue(members.TryGetValue("foo", out _));
                Assert.IsTrue(members.TryGetValue("bar", out _));
                Assert.IsTrue(members.TryGetValue("test", out _));
                Assert.IsTrue(members.TryGetValue("color", out _));
                Assert.IsFalse(members.TryGetValue("baz", out _));
            }
        }

        [Test]
        [TestCase("{\"LongObjectKey\": 10, 1234567890: \"LongValueString\"}")]
        public void SerializedObjectReader_Read_PartialToken(string json)
        {
            SetJson(json);

            var config = SerializedObjectReaderConfiguration.Default;

            config.ValidationType = JsonValidationType.None;
            config.BlockBufferSize = 16; // 16 bytes | 8 characters

            using (var reader = new SerializedObjectReader(m_Stream, config))
            {
                var obj = reader.ReadObject();

                Assert.IsTrue(obj.TryGetValue("LongObjectKey", out var a));
                Assert.AreEqual(10, a.AsPrimitiveView().AsInt64());
                Assert.IsTrue(obj.TryGetValue("1234567890", out var b));
                Assert.AreEqual("LongValueString", b.AsStringView().ToString());
            }
        }
        
        [Test]
        public void SerializedObjectReader_Step_MatchIfAtLeastOneFlag()
        {
            SetJson("{\"a\":\"b\"}");

            using (var reader = new SerializedObjectReader(m_Stream, m_ConfigWithNoValidation, leaveInputOpen: false))
            {
                Assert.That(reader.Step(NodeType.ObjectKey), Is.EqualTo(NodeType.ObjectKey | NodeType.String));
            }
        }
    }
}
