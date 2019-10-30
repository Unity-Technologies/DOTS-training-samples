using System;
using Unity.Collections;

namespace Unity.Serialization
{
    interface ITokenizer : IDisposable
    {
        /// <summary>
        /// Output buffer containing all written tokens.
        /// </summary>
        NativeArray<Token> Tokens { get; }

        /// <summary>
        /// The token state for the tokenizer. This is also the number of tokens in the buffer.
        /// </summary>
        int TokenNextIndex { get; }

        /// <summary>
        /// The current parent state for the tokenizer.
        /// </summary>
        int TokenParentIndex { get; }

        /// <summary>
        /// Writes <see cref="Token"/> objects to the internal buffer.
        /// </summary>s
        /// <param name="buffer">A character buffer containing the input data to tokenize.</param>
        /// <param name="start">The position in the buffer at which to start reading data.</param>
        /// <param name="count">The maximum number of characters to read.</param>
        /// <returns>The number of characters that have been read.</returns>
        int Write(UnsafeBuffer<char> buffer, int start, int count);

        /// <summary>
        /// Discards completed tokens from the internal buffer.
        /// </summary>
        void DiscardCompleted();
    }
}
