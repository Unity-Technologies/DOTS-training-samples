using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;

namespace Unity.Serialization
{
    /// <summary>
    /// A view on top of the <see cref="PackedBinaryStream"/> that represents a set of key-values.
    /// </summary>
    public readonly struct SerializedObjectView : ISerializedView, IEnumerable<SerializedMemberView>
    {
        /// <summary>
        /// Enumerates the elements of <see cref="SerializedObjectView"/>.
        /// </summary>
        public struct Enumerator : IEnumerator<SerializedMemberView>
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
            /// Advances the enumerator to the next element of the <see cref="SerializedObjectView"/>.
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
            /// The element in the <see cref="SerializedObjectView"/> at the current position of the enumerator.
            /// </summary>
            /// <exception cref="InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>
            public SerializedMemberView Current
            {
                get
                {
                    if (m_Current.Index < 0)
                    {
                        throw new InvalidOperationException();
                    }
                    return new SerializedMemberView(m_Stream, m_Current);
                }
            }

            object IEnumerator.Current => Current;

            /// <summary>
            /// Releases all resources used by the <see cref="SerializedObjectView.Enumerator" />.
            /// </summary>
            public void Dispose()
            {
            }
        }

        static SerializedObjectView() 
        {
            PropertyBagResolver.Register(new SerializedObjectViewPropertyBag());
        }

        readonly PackedBinaryStream m_Stream;
        readonly Handle m_Handle;

        internal SerializedObjectView(PackedBinaryStream stream, Handle handle)
        {
            m_Stream = stream;
            m_Handle = handle;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="name">The key of the value to get.</param>
        /// <exception cref="KeyNotFoundException">The key does not exist in the collection.</exception>
        public SerializedValueView this[string name]
        {
            get
            {
                if (TryGetValue(name, out var value))
                {
                    return value;
                }

                throw new KeyNotFoundException();
            }
        }

        /// <summary>
        /// Gets the member associated with the specified key.
        /// </summary>
        /// <param name="name">The key of the member to get.</param>
        /// <param name="member">When this method returns, contains the member associated with the specified key, if the key is found; otherwise, the default value.</param>
        /// <returns>true if the <see cref="SerializedObjectView"/> contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetMember(string name, out SerializedMemberView member)
        {
            foreach (var m in this)
            {
                if (!m.Name().Equals(name))
                {
                    continue;
                }

                member = m;
                return true;
            }

            member = default;
            return false;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="name">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value.</param>
        /// <returns>true if the <see cref="SerializedObjectView"/> contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue(string name, out SerializedValueView value)
        {
            foreach (var m in this)
            {
                if (!m.Name().Equals(name))
                {
                    continue;
                }

                value = m.Value();
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SerializedObjectView"/>.
        /// </summary>
        /// <returns>A <see cref="SerializedObjectView.Enumerator"/> for the <see cref="SerializedObjectView"/>.</returns>
        public Enumerator GetEnumerator() => new Enumerator(m_Stream, m_Handle);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SerializedObjectView"/>.
        /// </summary>
        /// <returns>A <see cref="SerializedObjectView.Enumerator"/> for the <see cref="SerializedObjectView"/>.</returns>
        IEnumerator<SerializedMemberView> IEnumerable<SerializedMemberView>.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SerializedObjectView"/>.
        /// </summary>
        /// <returns>A <see cref="SerializedObjectView.Enumerator"/> for the <see cref="SerializedObjectView"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
