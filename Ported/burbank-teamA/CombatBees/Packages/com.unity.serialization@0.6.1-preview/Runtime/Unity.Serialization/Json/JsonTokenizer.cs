using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Serialization.Json
{
    /// <summary>
    /// The validation type to use.
    /// </summary>
    public enum JsonValidationType
    {
        /// <summary>
        /// No validation is performed.
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Validation is performed against the standard json spec.
        /// </summary>
        Standard = 1,
        
        /// <summary>
        /// Only structural validation is performed.
        /// </summary>
        Simple = 2
    }

    /// <summary>
    /// The tokenizer is the lowest level API for json deserialization.
    ///
    /// It's only job is to parse characters into an array of <see cref="Token"/> simple structs.
    ///
    /// e.g. {"foo": 10} becomes
    ///
    ///  [0] Type=[JsonType.Object]    Range=[0..11] Parent=[-1]
    ///  [1] Type=[JsonType.String]    Range=[2..5]  Parent=[0]
    ///  [2] Type=[JsonType.Primitive] Range=[8..10] Parent=[1]
    ///
    /// @NOTE No conversion or copying of data takes place here.
    ///
    /// Implementation based off of https://github.com/zserge/jsmn
    /// </summary>
    class JsonTokenizer : ITokenizer
    {
        /// <summary>
        /// Special start value to signify this is a partial token continuation.
        /// </summary>
        const int k_PartialTokenStart = -1;

        /// <summary>
        /// Special end value to signify there is another part to follow.
        /// </summary>
        const int k_PartialTokenEnd = -1;

        /// <summary>
        /// All input characters were consumed and all tokens were generated.
        /// </summary>
        const int k_ResultSuccess = 0;

        /// <summary>
        /// The token buffer could not fit all tokens.
        /// </summary>
        const int k_ResultTokenBufferOverflow = -1;

        /// <summary>
        /// The input data was invalid in some way.
        /// </summary>
        const int k_ResultInvalidInput = -2;

        /// <summary>
        /// All input characters were consumed and the writer is expecting more
        /// </summary>
        const int k_ResultEndOfStream = -3;

        /// <summary>
        /// The maximum depth limit has been exceeded.
        /// </summary>
        const int k_ResultStackOverflow = -4;

        /// <summary>
        /// Maximum depth limit for discarding completed tokens.
        /// </summary>
        const int k_DefaultDepthLimit = 128;

        struct TokenizeJobOutput
        {
            public int Result;
            public int BufferPosition;
            public int TokenNextIndex;
            public int TokenParentIndex;
            public ushort PrevChar;
        }

        /// <summary>
        /// Transforms raw input characters to <see cref="Token"/> objects.
        ///
        /// Only structural validation is performed.
        /// </summary>
        [BurstCompile(CompileSynchronously = true)]
        unsafe struct TokenizeJob : IJob
        {
            [NativeDisableUnsafePtrRestriction] public TokenizeJobOutput* Output;

            [NativeDisableUnsafePtrRestriction] public ushort* CharBuffer;
            public int CharBufferLength;
            public int CharBufferPosition;
            public ushort PrevChar;

            [NativeDisableUnsafePtrRestriction] public Token* Tokens;
            public int TokensLength;
            public int TokensNextIndex;
            public int TokenParentIndex;

            void Break(int result)
            {
                Output->Result = result;
                Output->BufferPosition = CharBufferPosition;
                Output->TokenNextIndex = TokensNextIndex;
                Output->TokenParentIndex = TokenParentIndex;
                Output->PrevChar = PrevChar;
            }

            public void Execute()
            {
                if (TokensNextIndex >= TokensLength)
                {
                    Break(k_ResultTokenBufferOverflow);
                    return;
                }

                // Handle re-entry with a open token on the `stack`
                if (TokensNextIndex - 1 >= 0 && Tokens[TokensNextIndex - 1].End == -1)
                {
                    var token = Tokens[TokensNextIndex - 1];

                    switch (token.Type)
                    {
                        case TokenType.String:
                        case TokenType.Primitive:
                        {
                            // This is the continuation of a primitive or string
                            // Use -1 as the `Start` position to signify we are part of a partial token stream.
                            var result = token.Type == TokenType.String
                                ? ParseString(TokensNextIndex - 1, k_PartialTokenStart)
                                : ParsePrimitive(TokensNextIndex - 1, k_PartialTokenStart);

                            if (result != k_ResultSuccess)
                            {
                                if (result == k_ResultTokenBufferOverflow)
                                {
                                    CharBufferPosition = 0;
                                }

                                Break(result);
                                return;
                            }

                            CharBufferPosition++;
                        }
                            break;
                    }
                }

                for (; CharBufferPosition < CharBufferLength; CharBufferPosition++)
                {
                    var c = CharBuffer[CharBufferPosition];

                    switch (c)
                    {
                        case '{':
                        case '[':
                        {
                            if (TokensNextIndex >= TokensLength)
                            {
                                Break(k_ResultTokenBufferOverflow);
                                return;
                            }

                            Tokens[TokensNextIndex++] = new Token
                            {
                                Type = c == '{' ? TokenType.Object : TokenType.Array,
                                Parent = TokenParentIndex,
                                Start = CharBufferPosition,
                                End = -1
                            };

                            TokenParentIndex = TokensNextIndex - 1;
                        }
                            break;

                        case '}':
                        case ']':
                        {
                            var type = c == '}' ? TokenType.Object : TokenType.Array;

                            if (TokensNextIndex < 1)
                            {
                                Break(k_ResultInvalidInput);
                                return;
                            }

                            var index = TokensNextIndex - 1;

                            for (;;)
                            {
                                var token = Tokens[index];

                                if (token.Start != k_PartialTokenStart && token.End == k_PartialTokenEnd && token.Type != TokenType.String && token.Type != TokenType.Primitive)
                                {
                                    if (token.Type != type)
                                    {
                                        Break(k_ResultInvalidInput);
                                        return;
                                    }

                                    TokenParentIndex = token.Parent;
                                    token.End = CharBufferPosition + 1;
                                    Tokens[index] = token;
                                    break;
                                }

                                if (token.Parent == -1)
                                {
                                    if (token.Type != type || TokenParentIndex == -1)
                                    {
                                        Break(k_ResultInvalidInput);
                                        return;
                                    }

                                    break;
                                }

                                index = token.Parent;
                            }

                            var parent = TokenParentIndex != -1 ? Tokens[TokenParentIndex] : default;

                            if (TokenParentIndex != -1 &&
                                parent.Type != TokenType.Object &&
                                parent.Type != TokenType.Array)
                            {
                                TokenParentIndex = Tokens[TokenParentIndex].Parent;
                            }
                        }
                            break;

                        case '\t':
                        case '\r':
                        case ' ':
                        case '\n':
                        case '\0':
                        case ':':
                        case '=':
                        case ',':
                        {
                        }
                            break;

                        default:
                        {
                            int result;

                            if (c == '"')
                            {
                                CharBufferPosition++;

                                var start = CharBufferPosition;

                                result = ParseString(TokenParentIndex, start);

                                if (result == k_ResultTokenBufferOverflow)
                                {
                                    CharBufferPosition = start - 1;
                                    Break(result);
                                    return;
                                }
                            }
                            else
                            {
                                var start = CharBufferPosition;

                                result = ParsePrimitive(TokenParentIndex, start);

                                if (result == k_ResultTokenBufferOverflow)
                                {
                                    CharBufferPosition = start - 1;
                                    Break(result);
                                    return;
                                }
                            }

                            if (TokenParentIndex == -1 || Tokens[TokenParentIndex].Type == TokenType.Object)
                            {
                                TokenParentIndex = TokensNextIndex - 1;
                            }
                            else if (TokenParentIndex != -1 &&
                                     Tokens[TokenParentIndex].Type != TokenType.Object &&
                                     Tokens[TokenParentIndex].Type != TokenType.Array)
                            {
                                TokenParentIndex = Tokens[TokenParentIndex].Parent;
                            }

                            if (result == k_ResultEndOfStream)
                            {
                                Break(result);
                                return;
                            }
                        }
                        break;
                    }
                }

                Break(k_ResultSuccess);
            }

            int ParseString(int parent, int start)
            {
                PrevChar = 0;

                for (; CharBufferPosition < CharBufferLength; CharBufferPosition++)
                {
                    var c = CharBuffer[CharBufferPosition];

                    if (c == '"' && PrevChar != '\\')
                    {
                        if (TokensNextIndex >= TokensLength)
                        {
                            CharBufferPosition = start - 1;
                            return k_ResultTokenBufferOverflow;
                        }

                        Tokens[TokensNextIndex++] = new Token
                        {
                            Type = TokenType.String,
                            Parent = parent,
                            Start = start,
                            End = CharBufferPosition
                        };

                        break;
                    }

                    PrevChar = c;
                }

                if (CharBufferPosition >= CharBufferLength)
                {
                    if (TokensNextIndex >= TokensLength)
                    {
                        CharBufferPosition = start - 1;
                        return k_ResultTokenBufferOverflow;
                    }

                    Tokens[TokensNextIndex++] = new Token
                    {
                        Type = TokenType.String,
                        Parent = parent,
                        Start = start,
                        End = -1
                    };

                    return k_ResultEndOfStream;
                }

                return k_ResultSuccess;
            }

            int ParsePrimitive(int parent, int start)
            {
                for (; CharBufferPosition < CharBufferLength; CharBufferPosition++)
                {
                    var c = CharBuffer[CharBufferPosition];

                    if (c == ' ' ||
                        c == '\t' ||
                        c == '\r' ||
                        c == '\n' ||
                        c == '\0' ||
                        c == ',' ||
                        c == ']' ||
                        c == '}' ||
                        c == ':')
                    {
                        if (TokensNextIndex >= TokensLength)
                        {
                            CharBufferPosition = start;
                            return k_ResultTokenBufferOverflow;
                        }

                        Tokens[TokensNextIndex++] = new Token
                        {
                            Type = TokenType.Primitive,
                            Parent = parent,
                            Start = start,
                            End = CharBufferPosition
                        };

                        CharBufferPosition--;
                        break;
                    }

                    if (c < 32 || c >= 127)
                    {
                        return k_ResultInvalidInput;
                    }
                }

                if (CharBufferPosition >= CharBufferLength)
                {
                    if (TokensNextIndex >= TokensLength)
                    {
                        CharBufferPosition = start;
                        return k_ResultTokenBufferOverflow;
                    }

                    Tokens[TokensNextIndex++] = new Token
                    {
                        Type = TokenType.Primitive,
                        Parent = parent,
                        Start = start,
                        End = -1
                    };

                    return k_ResultEndOfStream;
                }

                return k_ResultSuccess;
            }
        }

        struct DiscardCompletedJobOutput
        {
            public int Result;
            public int ParentTokenIndex;
            public int NextTokenIndex;
        }

        /// <summary>
        /// Trims all completed sibling tokens.
        /// </summary>
        [BurstCompile(CompileSynchronously = true)]
        unsafe struct DiscardCompletedJob : IJob
        {
            [NativeDisableUnsafePtrRestriction] public DiscardCompletedJobOutput* Output;

            [NativeDisableUnsafePtrRestriction] public Token* JsonTokens;
            [NativeDisableUnsafePtrRestriction] public int* Remap;
            public int JsonTokenParentIndex;
            public int JsonTokenNextIndex;
            public int StackSize;

            public void Execute()
            {
                var stack = stackalloc int[StackSize];
                var sp = -1;

                var index = JsonTokenNextIndex - 1;

                for (;;)
                {
                    if (index == -1)
                    {
                        break;
                    }

                    var token = JsonTokens[index];

                    if (token.Start != k_PartialTokenStart)
                    {
                        // Support partial tokens
                        if (token.End == k_PartialTokenEnd && (token.Type == TokenType.Primitive || token.Type == TokenType.String))
                        {
                            var partToken = token;
                            var partIndex = index;
                            var partCount = 1;

                            while (partToken.End == -1 && partIndex < JsonTokenNextIndex - 1)
                            {
                                partCount++;
                                partToken = JsonTokens[++partIndex];
                            }

                            if (sp + partCount >= StackSize)
                            {
                                Output->Result = k_ResultTokenBufferOverflow;
                                return;
                            }

                            for (var i = partCount - 1; i >= 0; i--)
                            {
                                stack[++sp] = index + i;
                            }
                        }
                        else
                        {
                            if (sp + 1>= StackSize)
                            {
                                Output->Result = k_ResultTokenBufferOverflow;
                                return;
                            }

                            stack[++sp] = index;
                        }
                    }

                    index = token.Parent;
                }

                JsonTokenNextIndex = sp + 1;

                for (var i = 0; sp >= 0; i++, sp--)
                {
                    index = stack[sp];

                    if (JsonTokenParentIndex == index)
                    {
                        JsonTokenParentIndex = i;
                    }

                    var token = JsonTokens[index];

                    var parentIndex = i - 1;

                    if (token.Start != k_PartialTokenStart)
                    {
                        while (parentIndex != -1 && JsonTokens[parentIndex].Start == k_PartialTokenStart)
                        {
                            parentIndex--;
                        }
                    }

                    token.Parent = parentIndex;
                    JsonTokens[i] = token;
                    Remap[index] = i;
                }

                Output->NextTokenIndex = JsonTokenNextIndex;
                Output->ParentTokenIndex = JsonTokenParentIndex;
            }
        }

        const int k_DefaultBufferSize = 1024;

        readonly Allocator m_Label;
        NativeArray<Token> m_JsonTokens;
        NativeArray<int> m_DiscardRemap;
        int m_TokenNextIndex;
        int m_TokenParentIndex;
        ushort m_PrevChar;
        readonly JsonValidationType m_ValidationType;
        readonly IJsonValidator m_Validator;

        /// <inheritdoc />
        public NativeArray<Token> Tokens => m_JsonTokens;

        internal NativeArray<int> DiscardRemap => m_DiscardRemap;

        /// <inheritdoc />
        public int TokenNextIndex => m_TokenNextIndex;

        /// <inheritdoc />
        public int TokenParentIndex => m_TokenParentIndex;

        /// <summary>
        /// If true the token buffer will double in size when the capacity is exceeded; otherwise <see cref="BufferOverflowException"/> is thrown.
        /// </summary>
        public bool AllowTokenBufferResize { get; set; } = true;

        public JsonTokenizer(Allocator label)
            : this (k_DefaultBufferSize, JsonValidationType.None, label)
        {
        }

        public JsonTokenizer(JsonValidationType validation, Allocator label = SerializationConfiguration.DefaultAllocatorLabel)
            : this (k_DefaultBufferSize, validation, label)
        {
        }

        public JsonTokenizer(int bufferSize = k_DefaultBufferSize, JsonValidationType validation = JsonValidationType.None, Allocator label = SerializationConfiguration.DefaultAllocatorLabel)
        {
            m_ValidationType = validation;
            m_Label = label;

            if (bufferSize <= 0)
            {
                throw new ArgumentException($"Token buffer size {bufferSize} <= 0");
            }

            m_JsonTokens = new NativeArray<Token>(bufferSize, m_Label, NativeArrayOptions.UninitializedMemory);
            m_DiscardRemap = new NativeArray<int>(bufferSize, m_Label, NativeArrayOptions.UninitializedMemory);

            switch (validation)
            {
                case JsonValidationType.None:
                    m_Validator = new JsonNoneValidator();
                    break;
                case JsonValidationType.Standard:
                    m_Validator = new JsonStandardValidator(label);
                    break;
                case JsonValidationType.Simple:
                    m_Validator = new JsonSimpleValidator(label);
                    break;
            }

            Initialize();
        }

        /// <summary>
        /// Initializes the tokenizer for re-use.
        /// </summary>
        public void Initialize()
        {
            m_TokenNextIndex = 0;
            m_TokenParentIndex = -1;
            m_PrevChar = 0;
            m_Validator.Initialize();
        }

        /// <inheritdoc />
        /// <summary>
        /// Writes <see cref="T:Unity.Serialization.Token" /> objects to the internal buffer.
        /// </summary>s
        /// <param name="buffer">A character array containing the input json data to tokenize.</param>
        /// <param name="start">The index of ptr at which to begin reading.</param>
        /// <param name="count">The maximum number of characters to read.</param>
        /// <returns>The number of characters that have been read.</returns>
        public unsafe int Write(UnsafeBuffer<char> buffer, int start, int count)
        {
            if (start + count > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            var position = start;
            var validation = m_Validator.ValidateAsync(buffer, position, count);

            for (;;)
            {
                var output = new TokenizeJobOutput();

                var job = new TokenizeJob
                {
                    Output = &output,
                    CharBuffer = (ushort*) buffer.Buffer,
                    CharBufferLength = start + count,
                    CharBufferPosition = position,
                    PrevChar = m_PrevChar,
                    Tokens = (Token*) m_JsonTokens.GetUnsafePtr(),
                    TokensLength = m_JsonTokens.Length,
                    TokensNextIndex = m_TokenNextIndex,
                    TokenParentIndex = m_TokenParentIndex
                };

                if (m_ValidationType == JsonValidationType.None)
                {
                    job.Run();
                }
                else
                {
                    job.Schedule().Complete();
                }

                position = output.BufferPosition;

                m_TokenNextIndex = output.TokenNextIndex;
                m_TokenParentIndex = output.TokenParentIndex;
                m_PrevChar = output.PrevChar;

                if (output.Result == k_ResultTokenBufferOverflow)
                {
                    if (!AllowTokenBufferResize)
                    {
                        throw new BufferOverflowException($"Token buffer overflow TokenNextIndex=[{output.TokenNextIndex}]. Use a larger buffer or set AllowTokenBufferResize=[True]");
                    }

                    m_JsonTokens = NativeArrayUtility.Resize(m_JsonTokens, m_JsonTokens.Length * 2, m_Label, NativeArrayOptions.UninitializedMemory);
                    m_DiscardRemap = NativeArrayUtility.Resize(m_DiscardRemap, m_DiscardRemap.Length * 2, m_Label, NativeArrayOptions.UninitializedMemory);
                    continue;
                }

                validation.Complete();

                var result = m_Validator.GetResult();

                if (!result.IsValid() && result.ActualType != JsonType.EOF || output.Result == k_ResultInvalidInput)
                {
                    if (m_ValidationType == JsonValidationType.None)
                    {
                        // No validation pass was performed.
                        // The tokenizer has failed with something that was structurally invalid.
                        throw new InvalidJsonException($"Input json was structurally invalid. Try with {nameof(JsonValidationType)}=[Standard or Simple]")
                        {
                            Line = -1,
                            Character = -1
                        };
                    }

                    throw new InvalidJsonException(result.ToString())
                    {
                        Line = result.LineCount,
                        Character = result.CharCount
                    };
                }

                return position - start;
            }
        }

        /// <inheritdoc />
        public void DiscardCompleted()
        {
            DiscardCompleted(k_DefaultDepthLimit);
        }

        public unsafe void DiscardCompleted(int depth)
        {
            var output = new DiscardCompletedJobOutput();

            new DiscardCompletedJob
            {
                Output = &output,
                JsonTokens = (Token*) m_JsonTokens.GetUnsafePtr(),
                Remap = (int*) m_DiscardRemap.GetUnsafePtr(),
                JsonTokenParentIndex = m_TokenParentIndex,
                JsonTokenNextIndex = m_TokenNextIndex,
                StackSize = depth
            }.Run();

            if (output.Result == k_ResultStackOverflow)
            {
                throw new StackOverflowException($"Tokenization depth limit of {depth} exceeded.");
            }

            m_TokenNextIndex = output.NextTokenIndex;
            m_TokenParentIndex = output.ParentTokenIndex;
        }

        public void Dispose()
        {
            m_JsonTokens.Dispose();
            m_DiscardRemap.Dispose();
            m_Validator.Dispose();
        }
    }
}
