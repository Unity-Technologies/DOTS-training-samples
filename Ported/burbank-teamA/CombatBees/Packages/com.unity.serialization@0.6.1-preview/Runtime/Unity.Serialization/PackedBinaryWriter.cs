using System;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace Unity.Serialization
{
    class PackedBinaryWriter : IDisposable
    {
        struct WriteTokensJobOutput
        {
            public int InputTokenNextIndex;
            public int InputTokenParentIndex;
            public int BinaryTokenParentIndex;
            public int BinaryTokenNextIndex;
            public int BinaryBufferPosition;
        }

        [BurstCompile(CompileSynchronously = true)]
        unsafe struct WriteTokensJob : IJob
        {
            [NativeDisableUnsafePtrRestriction] public WriteTokensJobOutput* Output;

            public int InputBufferLength;

            [NativeDisableUnsafePtrRestriction] public Token* InputTokens;
            public int InputTokensCapacity;
            public int InputTokensLength;
            public int InputTokenNextIndex;
            public int InputTokenParentIndex;

            [NativeDisableUnsafePtrRestriction] public BinaryToken* BinaryTokens;
            public int BinaryTokenNextIndex;
            public int BinaryTokenParentIndex;

            [NativeDisableUnsafePtrRestriction] public HandleData* Handles;
            public int BinaryBufferPosition;

            public void Execute()
            {
                for (; InputTokenNextIndex < InputTokensLength; InputTokenNextIndex++, BinaryTokenNextIndex++)
                {
                    var inputToken = InputTokens[InputTokenNextIndex];

                    while (InputTokenParentIndex != -1 && inputToken.Parent < InputTokenParentIndex)
                    {
                        var inputTokenParent = InputTokens[InputTokenParentIndex];

                        InputTokenParentIndex = inputTokenParent.Parent;

                        if (BinaryTokenParentIndex == -1)
                        {
                            continue;
                        }

                        BinaryTokens[BinaryTokenParentIndex].Length = inputTokenParent.Start == -1 ? -1 : BinaryTokenNextIndex - BinaryTokenParentIndex;
                        BinaryTokenParentIndex = BinaryTokens[BinaryTokenParentIndex].Parent;
                    }

                    var binaryToken = BinaryTokens[BinaryTokenNextIndex];
                    binaryToken.Type = inputToken.Type;
                    binaryToken.Position = BinaryBufferPosition;
                    binaryToken.Parent = BinaryTokenParentIndex;
                    binaryToken.Length = inputToken.Start == -1 ? -1 : 1;
                    BinaryTokens[BinaryTokenNextIndex] = binaryToken;
                    Handles[binaryToken.HandleIndex].DataVersion++;

                    switch (inputToken.Type)
                    {
                        case TokenType.Array:
                        case TokenType.Object:
                        {
                            InputTokenParentIndex = InputTokenNextIndex;
                            BinaryTokenParentIndex = BinaryTokenNextIndex;
                        }
                        break;

                        case TokenType.Primitive:
                        case TokenType.String:
                        {
                            if (inputToken.Parent == -1 || InputTokens[inputToken.Parent].Type == TokenType.Object || inputToken.End == -1)
                            {
                                InputTokenParentIndex = InputTokenNextIndex;
                                BinaryTokenParentIndex = BinaryTokenNextIndex;
                            }

                            if (inputToken.Start != -1)
                            {
                                BinaryBufferPosition += sizeof(int);
                            }

                            var start = inputToken.Start != -1 ? inputToken.Start : 0;
                            var end = inputToken.End != -1 ? inputToken.End : InputBufferLength;
                            BinaryBufferPosition += (end - start) * sizeof(ushort);
                        }
                        break;
                    }
                }

                // Patch up the lengths
                for (int inputTokenIndex = InputTokenNextIndex - 1, binaryTokenIndex = BinaryTokenNextIndex - 1; inputTokenIndex >= 0 && binaryTokenIndex >= 0;)
                {
                    var inputToken = InputTokens[inputTokenIndex];
                    var binaryToken = BinaryTokens[binaryTokenIndex];

                    BinaryTokens[binaryTokenIndex].Length = inputToken.Start == -1 ? -1 : BinaryTokenNextIndex - binaryTokenIndex;

                    inputTokenIndex = inputToken.Parent;
                    binaryTokenIndex = binaryToken.Parent;
                }

                if (InputTokenNextIndex >= InputTokensCapacity)
                {
                    if (!IsObjectKey(InputTokens, InputTokenNextIndex - 1))
                    {
                        // Close the stack
                        while (InputTokenParentIndex >= 0)
                        {
                            var index = InputTokenParentIndex;
                            var token = InputTokens[index];

                            if (token.End == -1)
                            {
                                break;
                            }

                            InputTokenParentIndex = token.Parent;

                            if (BinaryTokenParentIndex == -1)
                            {
                                continue;
                            }

                            BinaryTokenParentIndex = BinaryTokens[BinaryTokenParentIndex].Parent;
                        }
                    }
                }

                Output->InputTokenNextIndex = InputTokenNextIndex;
                Output->InputTokenParentIndex = InputTokenParentIndex;
                Output->BinaryTokenNextIndex = BinaryTokenNextIndex;
                Output->BinaryTokenParentIndex = BinaryTokenParentIndex;
                Output->BinaryBufferPosition = BinaryBufferPosition;
            }

            /// <summary>
            /// Returns true if the given token is an object key.
            /// </summary>
            static bool IsObjectKey(Token* tokens, int index)
            {
                var token = tokens[index];

                if (token.Type != TokenType.String && token.Type != TokenType.Primitive)
                {
                    return false;
                }

                if (token.Parent == -1)
                {
                    return false;
                }

                var parent = tokens[token.Parent];

                return parent.Type == TokenType.Object;
            }
        }

        [BurstCompile(CompileSynchronously = true)]
        unsafe struct WriteCharactersJob : IJobParallelFor
        {
            [NativeDisableUnsafePtrRestriction] public ushort* InputBuffer;
            public int InputBufferLength;

            [NativeDisableUnsafePtrRestriction] public Token* InputTokens;
            public int InputTokenStart;

            [NativeDisableUnsafePtrRestriction] public BinaryToken* BinaryTokens;
            public int BinaryTokenStart;

            [NativeDisableUnsafePtrRestriction] public byte* BinaryBuffer;

            public void Execute(int index)
            {
                var inputTokenIndex = index + InputTokenStart;
                var inputToken = InputTokens[inputTokenIndex];

                var binaryTokenIndex = index + BinaryTokenStart;
                var binaryToken = BinaryTokens[binaryTokenIndex];

                var position = binaryToken.Position;

                switch (inputToken.Type)
                {
                    case TokenType.Primitive:
                    case TokenType.String:
                    {
                        if (inputToken.Start != -1)
                        {
                            position += sizeof(int);
                        }

                        var start = inputToken.Start != -1 ? inputToken.Start : 0;
                        var end = inputToken.End != -1 ? inputToken.End : InputBufferLength;

                        for (var i = start; i < end; i++)
                        {
                            Write(ref position, InputBuffer[i]);
                        }

                        if (inputToken.End != -1)
                        {
                            var startTokenIndex = inputTokenIndex;

                            for (;;)
                            {
                                if (inputToken.Start != -1 || inputToken.Parent == -1)
                                {
                                    break;
                                }

                                startTokenIndex = inputToken.Parent;
                                inputToken = InputTokens[startTokenIndex];
                            }

                            var offset = startTokenIndex - inputTokenIndex;
                            var startPosition = BinaryTokens[binaryTokenIndex + offset].Position;
                            var byteLength = position - startPosition;
                            byteLength -= sizeof(int);
                            Write(ref startPosition, byteLength / sizeof(ushort));
                        }
                    }
                    break;
                }
            }

            void Write<T>(ref int position, T value) where T : unmanaged
            {
                *(T*) (BinaryBuffer + position) = value;
                position += sizeof(T);
            }
        }

        PackedBinaryStream m_Stream;
        readonly ITokenizer m_Tokenizer;

        int m_InputTokenNextIndex;
        int m_InputTokenParentIndex;

        public int TokenNextIndex => m_InputTokenNextIndex;
        public int TokenParentIndex => m_InputTokenParentIndex;

        public PackedBinaryWriter(PackedBinaryStream stream, ITokenizer tokenizer)
        {
            m_Stream = stream;
            m_Tokenizer = tokenizer;
            Seek(0, -1);
        }

        /// <summary>
        /// Seeks the writer to the given token/parent combination.
        /// </summary>
        public void Seek(int index, int parent)
        {
            m_InputTokenNextIndex = index;
            m_InputTokenParentIndex = parent;
        }

        internal unsafe SerializedValueView GetView(int index)
        {
            var data = m_Stream.GetUnsafeData();
            if ((uint) index >= (uint) data->TokenNextIndex)
            {
                throw new IndexOutOfRangeException();
            }

            var token = data->Tokens[index];
            var handle = data->Handles[token.HandleIndex];
            return new SerializedValueView(m_Stream, new Handle { Index = token.HandleIndex, Version = handle.DataVersion });
        }

        /// <summary>
        /// Writes tokens and characters to the internal binary stream.
        /// </summary>
        /// <param name="buffer">A character array containing the input data that was tokenized.</param>
        /// <param name="count">The number of tokens to write.</param>
        /// <returns>The number of tokens written.</returns>
        public unsafe int Write(UnsafeBuffer<char> buffer, int count)
        {
            var data = m_Stream.GetUnsafeData();

            var length = Math.Min(m_InputTokenNextIndex + count, m_Tokenizer.TokenNextIndex);
            count = length - m_InputTokenNextIndex;

            m_Stream.EnsureTokenCapacity(data->TokenNextIndex + count);

            var output = new WriteTokensJobOutput();

            new WriteTokensJob
            {
                Output = &output,
                InputBufferLength = buffer.Length,
                InputTokens = (Token*) m_Tokenizer.Tokens.GetUnsafePtr(),
                InputTokensCapacity = m_Tokenizer.TokenNextIndex,
                InputTokensLength = length,
                InputTokenNextIndex = m_InputTokenNextIndex,
                InputTokenParentIndex = m_InputTokenParentIndex,
                BinaryTokens = data->Tokens,
                BinaryTokenNextIndex = data->TokenNextIndex,
                BinaryTokenParentIndex = data->TokenParentIndex,
                Handles = data->Handles,
                BinaryBufferPosition = data->BufferPosition,
            }.Run();

            m_Stream.EnsureBufferCapacity(output.BinaryBufferPosition);

            new WriteCharactersJob
            {
                InputBuffer = (ushort*) buffer.Buffer,
                InputBufferLength = buffer.Length,
                InputTokens = (Token*) m_Tokenizer.Tokens.GetUnsafePtr(),
                InputTokenStart = m_InputTokenNextIndex,
                BinaryTokens = data->Tokens,
                BinaryTokenStart = data->TokenNextIndex,
                BinaryBuffer = data->Buffer
            }.Run(count);

            m_InputTokenNextIndex = output.InputTokenNextIndex;
            m_InputTokenParentIndex = output.InputTokenParentIndex;

            data->TokenNextIndex = output.BinaryTokenNextIndex;
            data->TokenParentIndex = output.BinaryTokenParentIndex;
            data->BufferPosition = output.BinaryBufferPosition;

            return count;
        }

        public void Dispose()
        {
        }
    }
}
