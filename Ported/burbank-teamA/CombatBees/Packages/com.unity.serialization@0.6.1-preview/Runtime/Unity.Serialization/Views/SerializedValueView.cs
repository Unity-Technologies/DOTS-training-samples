using System;

namespace Unity.Serialization
{
    /// <summary>
    /// A view on top of the <see cref="PackedBinaryStream"/> that represents any value.
    /// </summary>
    public readonly struct SerializedValueView : ISerializedView
    {
        readonly PackedBinaryStream m_Stream;
        readonly Handle m_Handle;

        internal SerializedValueView(PackedBinaryStream stream, Handle handle)
        {
            m_Stream = stream;
            m_Handle = handle;
        }

        /// <summary>
        /// The <see cref="TokenType"/> for this view. Use this to check which conversions are valid.
        /// </summary>
        public TokenType Type => m_Stream.GetToken(m_Handle).Type;

        /// <summary>
        /// Returns true if the value represents a member.
        /// </summary>
        /// <returns>True if the value is a member.</returns>
        public bool IsMember()
        {
            var token = m_Stream.GetToken(m_Handle);

            if (token.Parent != -1 && m_Stream.GetToken(token.Parent).Type != TokenType.Object)
            {
                return false;
            }

            return token.Type == TokenType.String || token.Type == TokenType.Primitive;
        }

        /// <summary>
        /// Reinterprets the value as an array.
        /// </summary>
        /// <returns>The value as a <see cref="SerializedArrayView"/>.</returns>
        public SerializedArrayView AsArrayView()
        {
            CheckValueType(TokenType.Array);
            return new SerializedArrayView(m_Stream, m_Handle);
        }

        /// <summary>
        /// Reinterprets the value as an object.
        /// </summary>
        /// <returns>The value as a <see cref="SerializedObjectView"/>.</returns>
        public SerializedObjectView AsObjectView()
        {
            CheckValueType(TokenType.Object);
            return new SerializedObjectView(m_Stream, m_Handle);
        }

        /// <summary>
        /// Reinterprets the value as an string.
        /// </summary>
        /// <returns>The value as a <see cref="SerializedStringView"/>.</returns>
        /// <exception cref="InvalidOperationException">The value could not be reinterpreted.</exception>
        public SerializedStringView AsStringView()
        {
            var token = m_Stream.GetToken(m_Handle);

            if (token.Type != TokenType.String && token.Type != TokenType.Primitive)
            {
                throw new InvalidOperationException($"Failed to read value RequestedType=[{TokenType.String}|{TokenType.Primitive}] ActualType=[{token.Type}]");
            }

            return new SerializedStringView(m_Stream, m_Handle);
        }

        /// <summary>
        /// Reinterprets the value as a member.
        /// </summary>
        /// <returns>The value as a <see cref="SerializedMemberView"/>.</returns>
        /// <exception cref="InvalidOperationException">The value could not be reinterpreted.</exception>
        public SerializedMemberView AsMemberView()
        {
            if (!IsMember())
            {
                throw new InvalidOperationException($"Failed to read value as member");
            }

            return new SerializedMemberView(m_Stream, m_Handle);
        }

        /// <summary>
        /// Reinterprets the value as a primitive.
        /// </summary>
        /// <returns>The value as a <see cref="SerializedPrimitiveView"/>.</returns>
        public SerializedPrimitiveView AsPrimitiveView()
        {
            CheckValueType(TokenType.Primitive);
            return new SerializedPrimitiveView(m_Stream, m_Handle);
        }

        /// <summary>
        /// Reinterprets the value as a long.
        /// </summary>
        /// <returns>The value as a long.</returns>
        public long AsInt64()
        {
            return AsPrimitiveView().AsInt64();
        }

        /// <summary>
        /// Reinterprets the value as a ulong.
        /// </summary>
        /// <returns>The value as a ulong.</returns>
        public ulong AsUInt64()
        {
            return AsPrimitiveView().AsUInt64();
        }

        /// <summary>
        /// Reinterprets the value as a float.
        /// </summary>
        /// <returns>The value as a float.</returns>
        public float AsFloat()
        {
            return AsPrimitiveView().AsFloat();
        }

        /// <summary>
        /// Reinterprets the value as a double.
        /// </summary>
        /// <returns>The value as a double.</returns>
        public double AsDouble()
        {
            return AsPrimitiveView().AsDouble();
        }

        /// <summary>
        /// Reinterprets the value as a bool.
        /// </summary>
        /// <returns>The value as a bool.</returns>
        public bool AsBoolean()
        {
            return AsPrimitiveView().AsBoolean();
        }

        void CheckValueType(TokenType type)
        {
            var token = m_Stream.GetToken(m_Handle);

            if (token.Type != type)
            {
                throw new InvalidOperationException($"Failed to read value RequestedType=[{type}] ActualType=[{token.Type}]");
            }
        }

        /// <summary>
        /// Returns the debug string for this view.
        /// </summary>
        /// <returns>The debug string for this view.</returns>
        public override string ToString()
        {
            var token = m_Stream.GetToken(m_Handle);
            return token.ToString();
        }
    }
}
