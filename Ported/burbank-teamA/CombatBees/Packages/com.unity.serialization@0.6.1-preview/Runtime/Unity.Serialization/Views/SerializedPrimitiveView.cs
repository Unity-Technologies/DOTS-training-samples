using System.Globalization;

namespace Unity.Serialization
{
    /// <summary>
    /// A view on top of the <see cref="PackedBinaryStream"/> that represents any unquoted value.
    /// </summary>
    public readonly struct SerializedPrimitiveView : ISerializedView
    {
        readonly PackedBinaryStream m_Stream;
        readonly Handle m_Handle;

        internal SerializedPrimitiveView(PackedBinaryStream stream, Handle handle)
        {
            m_Stream = stream;
            m_Handle = handle;
        }

        /// <summary>
        /// Returns true if the primitive represents infinity.
        /// </summary>
        /// <returns>True if this primitive is infinity; false otherwise.</returns>
        public unsafe bool IsInfinity()
        {
            var ptr = m_Stream.GetBufferPtr<byte>(m_Handle);
            var len = *(int*) ptr;
            var chars = (char*) (ptr + sizeof(int));
            if (Convert.IsSigned(chars, len))
            {
                chars++;
                len--;
            }

            return Convert.MatchesInfinity(chars, len);
        }

        /// <summary>
        /// Returns true if the primitive represents a value that is not a number.
        /// </summary>
        /// <returns>True if this primitive is nan; false otherwise.</returns>
        public unsafe bool IsNaN()
        {
            var ptr = m_Stream.GetBufferPtr<byte>(m_Handle);
            var len = *(int*) ptr;
            var chars = (char*) (ptr + sizeof(int));
            return Convert.MatchesNaN(chars, len);
        }

        /// <summary>
        /// Returns true if the primitive is an integral type.
        /// </summary>
        /// <returns>True if this primitive is an integral type; false otherwise.</returns>
        public unsafe bool IsIntegral()
        {
            var ptr = m_Stream.GetBufferPtr<byte>(m_Handle);
            return Convert.IsIntegral((char*) (ptr + sizeof(int)), *(int*) ptr);
        }

        /// <summary>
        /// Returns true if the primitive is a decimal type.
        /// </summary>
        /// <returns>True if this primitive is an decimal type; false otherwise.</returns>
        public unsafe bool IsDecimal()
        {
            var ptr = m_Stream.GetBufferPtr<byte>(m_Handle);
            return Convert.IsDecimal((char*) (ptr + sizeof(int)), *(int*) ptr);
        }

        /// <summary>
        /// Returns true if the primitive is a signed type.
        /// </summary>
        /// <returns>True if this primitive is an signed type; false otherwise.</returns>
        public unsafe bool IsSigned()
        {
            var ptr = m_Stream.GetBufferPtr<byte>(m_Handle);
            return Convert.IsSigned((char*) (ptr + sizeof(int)), *(int*) ptr);
        }

        /// <summary>
        /// Returns true if the primitive is a boolean type.
        /// </summary>
        /// <returns>True if this primitive is an boolean type; false otherwise.</returns>
        public unsafe bool IsBoolean()
        {
            var ptr = m_Stream.GetBufferPtr<byte>(m_Handle);
            var length = *(int*) ptr;
            var chars = (char*) (ptr + sizeof(int));
            return Convert.MatchesTrue(chars, length) || Convert.MatchesFalse(chars, length);
        }

        /// <summary>
        /// Returns a string view over the primitive.
        /// </summary>
        /// <returns>A <see cref="SerializedStringView"/> over this primitive.</returns>
        public SerializedStringView AsStringView() => new SerializedStringView(m_Stream, m_Handle);

        /// <summary>
        /// Allocates and returns a new <see cref="string"/> for the primitive.
        /// </summary>
        /// <returns>A <see cref="string"/> copy of the primitive.</returns>
        public string AsString() => AsStringView().ToString();

        /// <summary>
        /// Reinterprets the primitive as a long.
        /// </summary>
        /// <returns>The primitive as a long.</returns>
        /// <exception cref="ParseErrorException">The parser failed to convert the characters.</exception>
        public unsafe long AsInt64()
        {
            var ptr = m_Stream.GetBufferPtr<byte>(m_Handle);
            var result = Convert.StrToInt64((char*) (ptr + sizeof(int)), *(int*) ptr, out var value);
            if (result != Convert.ParseError.None)
            {
                throw new ParseErrorException($"Failed to parse Value=[{AsString()}] as Type=[{typeof(long)}] ParseError=[{result}]");
            }

            return value;
        }

        /// <summary>
        /// Reinterprets the primitive as a ulong.
        /// </summary>
        /// <returns>The primitive as a ulong.</returns>
        /// <exception cref="ParseErrorException">The parser failed to convert the characters.</exception>
        public unsafe ulong AsUInt64()
        {
            var ptr = m_Stream.GetBufferPtr<byte>(m_Handle);
            var result = Convert.StrToUInt64((char*) (ptr + sizeof(int)), *(int*) ptr, out var value);
            if (result != Convert.ParseError.None)
            {
                throw new ParseErrorException($"Failed to parse Value=[{AsString()}] as Type=[{typeof(ulong)}] ParseError=[{result}]");
            }

            return value;
        }

        /// <summary>
        /// Reinterprets the primitive as a float.
        /// </summary>
        /// <returns>The primitive as a float.</returns>
        /// <exception cref="ParseErrorException">The parser failed to convert the characters.</exception>
        public unsafe float AsFloat()
        {
            var ptr = m_Stream.GetBufferPtr<byte>(m_Handle);
            var result = Convert.StrToFloat32((char*) (ptr + sizeof(int)), *(int*) ptr, out var value);

            if (result != Convert.ParseError.None)
            {
                throw new ParseErrorException($"Failed to parse Value=[{AsString()}] as Type=[{typeof(float)}] ParseError=[{result}]");
            }

            return value;
        }

        /// <summary>
        /// Reinterprets the primitive as a double.
        /// </summary>
        /// <remarks>
        /// This method relies on a string allocation for <see cref="double.Parse(string)"/>. 
        /// </remarks>
        /// <returns>The primitive as a double.</returns>
        public double AsDouble()
        {
            return double.Parse(AsString(), NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Reinterprets the primitive as a bool.
        /// </summary>
        /// <returns>The primitive as a bool.</returns>
        /// <exception cref="ParseErrorException">The parser failed to convert the characters.</exception>
        public unsafe bool AsBoolean()
        {
            var ptr = m_Stream.GetBufferPtr<byte>(m_Handle);
            var length = *(int*) ptr;
            var chars = (char*) (ptr + sizeof(int));

            if (Convert.MatchesTrue(chars, length))
            {
                return true;
            }

            if (Convert.MatchesFalse(chars, length))
            {
                return false;
            }

            throw new ParseErrorException($"Failed to parse Value=[{AsString()}] as Type=[{typeof(bool)}]");
        }
    }
}