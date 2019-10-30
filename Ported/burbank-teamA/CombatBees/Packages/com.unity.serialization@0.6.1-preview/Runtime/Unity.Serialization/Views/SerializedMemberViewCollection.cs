using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace Unity.Serialization
{
    /// <summary>
    /// A structure to hold a collection of <see cref="SerializedMemberView"/>.
    /// </summary>
    /// <remarks>
    /// This structure is not a view itself but rather a container for views.
    /// </remarks>
    public readonly struct SerializedMemberViewCollection : IDisposable, IEnumerable<SerializedMemberView>
    {
        /// <summary>
        /// Enumerates the elements of <see cref="SerializedMemberViewCollection"/>.
        /// </summary>
        public struct Enumerator : IEnumerator<SerializedMemberView>
        {
            readonly SerializedMemberViewCollection m_Collection;
            int m_Index;

            internal Enumerator(SerializedMemberViewCollection collection)
            {
                m_Collection = collection;
                m_Index = -1;
            }


            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="SerializedMemberViewCollection"/>.
            /// </summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                m_Index++;
                return m_Index < m_Collection.m_Members.Length;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                m_Index = -1;
            }


            /// <summary>
            /// The element in the <see cref="SerializedMemberViewCollection"/> at the current position of the enumerator.
            /// </summary>
            /// <exception cref="InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>
            public SerializedMemberView Current
            {
                get
                {
                    if (m_Index < 0)
                    {
                        throw new InvalidOperationException();
                    }
                    return m_Collection.m_Members[m_Index];
                }
            }

            object IEnumerator.Current => Current;

            /// <summary>
            /// Releases all resources used by the <see cref="SerializedMemberViewCollection.Enumerator" />.
            /// </summary>
            public void Dispose()
            {

            }
        }

        readonly NativeList<SerializedMemberView> m_Members;

        /// <summary>
        /// Initializes a new instance of <see cref="SerializedMemberViewCollection"/> using the given allocator.
        /// </summary>
        /// <param name="label">The memory allocator label.</param>
        public SerializedMemberViewCollection(Allocator label)
        {
            m_Members = new NativeList<SerializedMemberView>(label);
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
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="name">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value.</param>
        /// <returns>true if the <see cref="SerializedMemberViewCollection"/> contains an element with the specified key; otherwise, false.</returns>
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
        /// Adds the specified <see cref="SerializedMemberView"/> to the collection.
        /// </summary>
        /// <param name="view">The value of to add.</param>
        public void Add(SerializedMemberView view)
        {
            m_Members.Add(view);
        }
        
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SerializedMemberViewCollection"/>.
        /// </summary>
        /// <returns>A <see cref="SerializedMemberViewCollection.Enumerator"/> for the <see cref="SerializedMemberViewCollection"/>.</returns>
        public Enumerator GetEnumerator() => new Enumerator(this);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SerializedMemberViewCollection"/>.
        /// </summary>
        /// <returns>A <see cref="SerializedMemberViewCollection.Enumerator"/> for the <see cref="SerializedMemberViewCollection"/>.</returns>
        IEnumerator<SerializedMemberView> IEnumerable<SerializedMemberView>.GetEnumerator() => new Enumerator(this);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SerializedMemberViewCollection"/>.
        /// </summary>
        /// <returns>A <see cref="SerializedMemberViewCollection.Enumerator"/> for the <see cref="SerializedMemberViewCollection"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Releases all resources used by the <see cref="SerializedMemberViewCollection" />.
        /// </summary>
        public void Dispose()
        {
            m_Members.Dispose();
        }
    }
}
