using System.IO;
using System.Text;

namespace Unity.Serialization.PerformanceTests
{
	static class JsonTestData
    {
        /// <summary>
        /// NOTE! This is not way represents the asset format that will be used. This is simply structurally valid json that we can consume to test raw performance.
        /// </summary>
        const string k_MockEntity = @"{
    ""id"": ""cbb65a2e0d1546cfa52f43d9f1441348"",
    ""components"": [{
        ""$type"": { ""name"": ""Parent"", ""namespace"": ""Unity.Tiny.Core2D"" },
        ""parent"": { ""id"": ""bce48ab5c02b4e96a23f499cea27b8e1"" }
    },{
        ""$type"": { ""name"": ""Translation"", ""namespace"": ""Unity.Tiny.Core2D"" },
        ""Value"": {
            ""x"": 0,
            ""y"": 0,
            ""z"": 0
        }
    },{
        ""$type"": { ""name"": ""Rotation"", ""namespace"": ""Unity.Tiny.Core2D"" },
        ""Value"": {
            ""x"": 0,
            ""y"": 0,
            ""z"": 0,
            ""w"": 1
        }
    },{
        ""$type"": { ""name"": ""NonUniformScale"", ""namespace"": ""Unity.Tiny.Core2D"" },
        ""Value"": {
            ""x"": 1,
            ""y"": 1,
            ""z"": 1
        }
    },{
        ""$type"": { ""name"": ""Sprite2DRenderer"", ""namespace"": ""Unity.Tiny.Core2D"" },
        ""sprite"": { ""guid"": ""1c1f88ea2e994195aa69fdcb8e89c32d"", ""fileId"": 120000, ""type"": 3 },
        ""color"": {
            ""r"": 1,
            ""g"": 1,
            ""b"": 1,
            ""a"": 1
        }
    }]
}";

        public static string GetMockEntities(int count)
        {
            var builder = new StringBuilder();
            builder.Append('[');

            for (var i = 0; i < count; i++)
            {
                builder.Append(k_MockEntity);

                if (i != count - 1)
                {
                    builder.Append(",\n");
                }
            }

            builder.Append("]");
            return builder.ToString();
        }

        /// <summary>
        /// Fills the stream with the given number of entities.
        /// </summary>
        public static void FillStreamWithMockEntities(Stream stream, int count)
        {
            stream.Seek(0, SeekOrigin.Begin);

            using (var writer = new StreamWriter(stream, Encoding.Default, 1024, true))
            {
                writer.Write(GetMockEntities(count));
                writer.Flush();
            }

            stream.Seek(0, SeekOrigin.Begin);
        }
    }
}
