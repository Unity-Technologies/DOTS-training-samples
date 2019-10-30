using System;
using System.IO;
using System.Text;
using Unity.Properties;

namespace Unity.Serialization.Json
{
    /// <summary>
    /// Helper class that generically writes any property container as a JSON string.
    ///
    /// @NOTE This class makes heavy use of `StringBuilder` and `.ToString` on primitives, which allocates large amounts of memory. Use it sparingly.
    ///
    /// @TODO
    ///    * Optimization
    /// </summary>
    /// <remarks>
    /// The deserialization methods will not construct type instances. All object fields must be initialized in the default constructor.
    /// </remarks>
    public static class JsonSerialization
    {
        static readonly JsonVisitor s_DefaultVisitor = new JsonVisitor();

        /// <summary>
        /// Deserializes the given file path and writes the data to the given container.
        /// </summary>
        /// <param name="path">The file path to read from.</param>
        /// <param name="container">The container to deserialize data in to.</param>
        /// <typeparam name="TContainer">The type to deserialize.</typeparam>
        public static VisitResult DeserializeFromPath<TContainer>(string path, ref TContainer container)
        {
            using (var reader = new SerializedObjectReader(path))
            {
                return Deserialize(reader, ref container);
            }
        }

        /// <summary>
        /// Deserializes the given file path and returns a new instance of the container.
        /// </summary>
        /// <param name="path">The file path to read from.</param>
        /// <typeparam name="TContainer">The type to deserialize.</typeparam>
        /// <returns>A new instance of the container with based on the serialized data.</returns>
        public static TContainer DeserializeFromPath<TContainer>(string path)
            where TContainer : new()
        {
            var container = new TContainer();
            using (var reader = new SerializedObjectReader(path))
            {
                using (var result = Deserialize(reader, ref container))
                {
                    result.Throw();
                }
            }
            return container;
        }

        /// <summary>
        /// Deserializes the given json string and writes the data to the given container.
        /// </summary>
        /// <param name="jsonString">The json data as a string.</param>
        /// <param name="container">The container to deserialize data in to.</param>
        /// <typeparam name="TContainer">The type to deserialize.</typeparam>
        public static VisitResult DeserializeFromString<TContainer>(string jsonString, ref TContainer container)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                return DeserializeFromStream(stream, ref container);
            }
        }

        /// <summary>
        /// Deserializes the given json string and returns a new instance of the container.
        /// </summary>
        /// <param name="jsonString">The json data as a string.</param>
        /// <typeparam name="TContainer">The type to deserialize.</typeparam>
        /// <returns>A new instance of the container with based on the serialized data.</returns>
        public static TContainer DeserializeFromString<TContainer>(string jsonString)
            where TContainer : new()
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                return DeserializeFromStream<TContainer>(stream);
            }
        }

        /// <summary>
        /// Deserializes the given stream and writes the data to the given container.
        /// </summary>
        /// <typeparam name="TContainer">The type to deserialize.</typeparam>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="container">The container to deserialize data in to.</param>
        public static VisitResult DeserializeFromStream<TContainer>(Stream stream, ref TContainer container)
        {
            using (var reader = new SerializedObjectReader(stream))
            {
                return Deserialize(reader, ref container);
            }
        }

        /// <summary>
        /// Deserializes the given stream and returns a new instance of the container.
        /// </summary>
        /// <typeparam name="TContainer">The type to deserialize.</typeparam>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>A new instance of the container with based on the serialized data.</returns>
        public static TContainer DeserializeFromStream<TContainer>(Stream stream)
            where TContainer : new()
        {
            var container = new TContainer();
            using (var reader = new SerializedObjectReader(stream))
            {
                using (var result = Deserialize(reader, ref container))
                {
                    result.Throw();
                }
            }
            return container;
        }

        static VisitResult Deserialize<TContainer>(SerializedObjectReader reader, ref TContainer container)
        {
            var source = reader.ReadObject();
            var result = VisitResult.GetPooled();
            try
            {
                using (var construction = PropertyContainer.Construct(ref container, ref source, new PropertyContainerConstructOptions {TypeIdentifierKey = JsonVisitor.Style.TypeInfoKey}))
                    result.TransferEvents(construction);

                using (var transfer = PropertyContainer.Transfer(ref container, ref source))
                    result.TransferEvents(transfer);
            }
            catch (Exception)
            {
                reader.Dispose();
                throw;
            }

            return result;
        }

        /// <summary>
        /// Writes a property container to a file path.
        /// </summary>
        /// <param name="path">The file path to write to.</param>
        /// <param name="target">The struct or class to serialize.</param>
        /// <typeparam name="TContainer">The type to serialize.</typeparam>
        public static void Serialize<TContainer>(string path, TContainer target)
        {
            File.WriteAllText(path, Serialize(target));
        }

        /// <summary>
        /// Writes a property container to a json string.
        /// </summary>
        /// <param name="container">The container to write.</param>
        /// <param name="visitor">The visitor to use. If none is provided, the default one is used.</param>
        /// <typeparam name="TContainer">The type to serialize.</typeparam>
        /// <returns>A json string.</returns>
        public static string Serialize<TContainer>(TContainer container, JsonVisitor visitor = null)
        {
            if (null == visitor)
            {
                visitor = s_DefaultVisitor;
            }

            visitor.Builder.Clear();

            WritePrefix(visitor);
            PropertyContainer.Visit(container, visitor);
            WriteSuffix(visitor);

            return visitor.Builder.ToString();
        }

        static void WritePrefix(JsonVisitor visitor)
        {
            visitor.Builder.Append(' ', JsonVisitor.Style.Space * visitor.Indent);
            visitor.Builder.Append("{\n");
            visitor.Indent++;
        }

        static void WriteSuffix(JsonVisitor visitor)
        {
            visitor.Indent--;

            if (visitor.Builder[visitor.Builder.Length - 2] == '{')
            {
                visitor.Builder.Length -= 1;
            }
            else
            {
                visitor.Builder.Length -= 2;
            }

            visitor.Builder.Append("\n");
            visitor.Builder.Append(' ', JsonVisitor.Style.Space * visitor.Indent);
            visitor.Builder.Append("}");
        }
    }
}
