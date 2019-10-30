using System;
using System.Text;
using Unity.Properties;

namespace Unity.Serialization.Json
{
    /// <summary>
    /// The <see cref="JsonVisitorAdapter"/> can be inherited to implement custom serializers for user-defined types.
    /// </summary>
    public abstract class JsonVisitorAdapter : IPropertyVisitorAdapter
    {
        readonly JsonVisitor m_Visitor;

        /// <summary>
        /// Initializes a new instance of <see cref="JsonVisitorAdapter"/>.
        /// </summary>
        /// <param name="visitor">The <see cref="JsonVisitor"/> this adapter was added to.</param>
        protected JsonVisitorAdapter(JsonVisitor visitor)
        {
            m_Visitor = visitor;
        }

        /// <summary>
        /// This method handles all of the json boilerplate for writing a property.
        ///
        /// The <paramref name="write"/> callback will be invoked during this call and should be used to write the actual value.
        /// </summary>
        /// <param name="property">The property being written.</param>
        /// <param name="value">The value being written.</param>
        /// <param name="write">Callback invoked to write the value in a strongly typed way.</param>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        protected void Append<TProperty, TValue>(TProperty property, TValue value, Action<StringBuilder, TValue> write)
            where TProperty : IProperty
        {
            if (property is ICollectionElementProperty)
            {
                m_Visitor.Builder.Append(' ', JsonVisitor.Style.Space * m_Visitor.Indent);
                write(m_Visitor.Builder, value);
                m_Visitor.Builder.Append(",\n");
            }
            else
            {
                m_Visitor.Builder.Append(' ', JsonVisitor.Style.Space * m_Visitor.Indent);
                m_Visitor.Builder.Append("\"");
                m_Visitor.Builder.Append(property.GetName());
                m_Visitor.Builder.Append("\": ");
                write(m_Visitor.Builder, value);
                m_Visitor.Builder.Append(",\n");
            }
        }

        /// <summary>
        /// Utility method to write a json encoded string.
        /// </summary>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="property">The property being written.</param>
        /// <param name="value">The string to append.</param>
        protected void AppendJsonString<TProperty>(TProperty property, string value)
            where TProperty : IProperty
        {
            Append(property, value, (builder, v) => builder.Append(EncodeJsonString(v)));
        }

        static readonly StringBuilder s_Builder = new StringBuilder(64);

        /// <summary>
        /// Encodes the given string to be written as json. This will add any necessary escape characters.
        /// </summary>
        /// <param name="value">The string value to encode.</param>
        /// <returns>The encoded string value.</returns>
        protected static string EncodeJsonString(string value)
        {
            if (value == null)
            {
                return "null";
            }

            var b = s_Builder;
            b.Clear();
            b.Append("\"");

            foreach (var c in value)
            {
                switch (c)
                {
                    case '\\':
                        b.Append("\\\\");
                        break; // @TODO Unicode look-ahead \u1234
                    case '\"':
                        b.Append("\\\"");
                        break;
                    case '\t':
                        b.Append("\\t");
                        break;
                    case '\r':
                        b.Append("\\r");
                        break;
                    case '\n':
                        b.Append("\\n");
                        break;
                    case '\b':
                        b.Append("\\b");
                        break;
                    default:
                        b.Append(c);
                        break;
                }
            }

            b.Append("\"");
            return s_Builder.ToString();
        }
    }
}
