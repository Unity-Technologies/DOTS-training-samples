namespace Unity.Serialization
{
    /// <summary>
    /// A view on top of the <see cref="PackedBinaryStream"/> that represents a key-value pair.
    /// </summary>
    public readonly struct SerializedMemberView
    {
        readonly PackedBinaryStream m_Stream;
        readonly Handle m_Handle;

        internal SerializedMemberView(PackedBinaryStream stream, Handle handle)
        {
            m_Stream = stream;
            m_Handle = handle;
        }

        /// <summary>
        /// Returns a <see cref="SerializedStringView"/> over the name of this member.
        /// </summary>
        /// <returns>A view over the name.</returns>
        public SerializedStringView Name() => new SerializedStringView(m_Stream, m_Handle);

        /// <summary>
        /// Returns a <see cref="SerializedValueView"/> over the value of this member.
        /// </summary>
        /// <returns>A view over the value.</returns>
        public SerializedValueView Value() => new SerializedValueView(m_Stream, m_Stream.GetChild(m_Handle));
    }
}