using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.Serialization
{
    /// <summary>
    /// A view on top of the <see cref="PackedBinaryStream"/> that represents an array of values.
    /// </summary>
    public readonly struct SerializedArrayView : ISerializedView, IEnumerable<SerializedValueView>
    {
        /// <summary>
        /// Enumerates the elements of <see cref="SerializedArrayView"/>.
        /// </summary>
        public struct Enumerator : IEnumerator<SerializedValueView>
        {
            readonly PackedBinaryStream m_Stream;
            readonly Handle m_Start;
            Handle m_Current;

            internal Enumerator(PackedBinaryStream stream, Handle start)
            {
                m_Stream = stream;
                m_Start = start;
                m_Current = new Handle {Index = -1, Version = -1};
            }

            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="SerializedArrayView"/>.
            /// </summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                var startIndex = m_Stream.GetTokenIndex(m_Start);
                var startToken = m_Stream.GetToken(startIndex);

                if (startToken.Length == 1)
                {
                    return false;
                }

                if (m_Current.Index == -1)
                {
                    m_Current = m_Stream.GetChild(m_Start);
                    return true;
                }

                if (!m_Stream.IsValid(m_Current))
                {
                    return false;
                }
                
                var currentIndex = m_Stream.GetTokenIndex(m_Current);
                var currentToken = m_Stream.GetToken(currentIndex);

                if (currentIndex + currentToken.Length >= startIndex + startToken.Length)
                {
                    return false;
                }
                
                m_Current = m_Stream.GetHandle(currentIndex + currentToken.Length);
                return true;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                m_Current = new Handle {Index = -1, Version = -1};
            }

            /// <summary>
            /// The element in the <see cref="SerializedArrayView"/> at the current position of the enumerator.
            /// </summary>
            /// <exception cref="InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>
            public SerializedValueView Current
            {
                get
                {
                    if (m_Current.Index < 0)
                    {
                        throw new InvalidOperationException();
                    }
                    return new SerializedValueView(m_Stream, m_Current);
                }
            }

            object IEnumerator.Current => Current;

            /// <summary>
            /// Releases all resources used by the <see cref="SerializedArrayView.Enumerator" />.
            /// </summary>
            public void Dispose()
            {
            }
        }
        
        readonly PackedBinaryStream m_Stream;
        readonly Handle m_Handle;
      
        internal SerializedArrayView(PackedBinaryStream stream, Handle handle)
        {
            m_Stream = stream;
            m_Handle = handle;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SerializedArrayView"/>.
        /// </summary>
        /// <returns>A <see cref="SerializedArrayView.Enumerator"/> for the <see cref="SerializedArrayView"/>.</returns>
        public Enumerator GetEnumerator() => new Enumerator(m_Stream, m_Handle);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SerializedArrayView"/>.
        /// </summary>
        /// <returns>A <see cref="SerializedArrayView.Enumerator"/> for the <see cref="SerializedArrayView"/>.</returns>
        IEnumerator<SerializedValueView> IEnumerable<SerializedValueView>.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SerializedArrayView"/>.
        /// </summary>
        /// <returns>A <see cref="SerializedArrayView.Enumerator"/> for the <see cref="SerializedArrayView"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}