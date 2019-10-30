using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Serialization
{
    class NodeParser : IDisposable
    {
        struct ParseJobOutput
        {
            public int TokenNextIndex;
            public int TokenParentIndex;
            public NodeType NodeType;
            public int NodeNextIndex;
        }

        [BurstCompile(CompileSynchronously = true)]
        unsafe struct ParseJob : IJob
        {
            [NativeDisableUnsafePtrRestriction] public ParseJobOutput* Output;

            [NativeDisableUnsafePtrRestriction] public Token* Tokens;
            public int TokensLength;
            public int TokenNextIndex;
            public int TokenParentIndex;

            public NodeType TargetNodeType;
            public int TargetParentIndex;
            public int TargetNodeCount;

            [NativeDisableUnsafePtrRestriction] public int* Nodes;
            int m_NodeNextIndex;

            void Break(NodeType type)
            {
                Output->TokenNextIndex = TokenNextIndex;
                Output->TokenParentIndex = TokenParentIndex;
                Output->NodeType = type;
                Output->NodeNextIndex = m_NodeNextIndex;
            }

            public void Execute()
            {
                for (; TokenNextIndex < TokensLength; TokenNextIndex++)
                {
                    var node = NodeType.None;

                    var token = Tokens[TokenNextIndex];

                    while (Tokens[TokenNextIndex].Parent < TokenParentIndex)
                    {
                        var index = TokenParentIndex;

                        node = PopToken();

                        if (Evaluate(node, index))
                        {
                            if (TokenParentIndex < TargetParentIndex)
                            {
                                TokenParentIndex = index;
                            }

                            Break(node == NodeType.None ? NodeType.Any : node);
                            return;
                        }
                    }

                    var nodeIndex = TokenNextIndex;

                    switch (token.Type)
                    {
                        case TokenType.Array:
                        case TokenType.Object:
                        {
                            node |= token.Type == TokenType.Array ? NodeType.BeginArray : NodeType.BeginObject;
                            TokenParentIndex = TokenNextIndex;
                        }
                        break;

                        case TokenType.Primitive:
                        case TokenType.String:
                        {
                            if (token.End != -1)
                            {
                                node |= token.Type == TokenType.Primitive ? NodeType.Primitive : NodeType.String;

                                while (token.Start == -1)
                                {
                                    nodeIndex = token.Parent;
                                    token = Tokens[nodeIndex];
                                }

                                if (token.Parent == -1 || Tokens[token.Parent].Type == TokenType.Object)
                                {
                                    node |= NodeType.ObjectKey;
                                    TokenParentIndex = TokenNextIndex;
                                }
                            }
                        }
                        break;
                    }

                    if (Evaluate(node, nodeIndex))
                    {
                        TokenNextIndex++;
                        Break(node == NodeType.None ? NodeType.Any : node);
                        return;
                    }
                }

                while (TokenParentIndex >= 0)
                {
                    var index = TokenParentIndex;
                    var token = Tokens[index];

                    if (token.End == -1)
                    {
                        Break(NodeType.None);
                        return;
                    }

                    var node = PopToken();

                    if (Evaluate(node, index))
                    {
                        if (TokenParentIndex < TargetParentIndex)
                        {
                            TokenParentIndex = index;
                        }

                        Break(node == NodeType.None ? NodeType.Any : node);
                        return;
                    }
                }

                Break(NodeType.None);
            }

            /// <summary>
            /// Evaluate user instruction to determine if we should break the parsing.
            ///
            /// @TODO Cleanup; far too many checks happening here
            /// </summary>
            bool Evaluate(NodeType node, int index)
            {
                if (TokenParentIndex <= TargetParentIndex)
                {
                    if (node == NodeType.None || (node & TargetNodeType) == node && TokenParentIndex == TargetParentIndex)
                    {
                        Nodes[m_NodeNextIndex++] = index;

                        if (m_NodeNextIndex < TargetNodeCount)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                if (node == NodeType.None)
                {
                    return false;
                }

                if ((node & TargetNodeType) != NodeType.None && (TargetParentIndex == k_IgnoreParent || TargetParentIndex >= TokenParentIndex))
                {
                    Nodes[m_NodeNextIndex++] = index;

                    if (m_NodeNextIndex >= TargetNodeCount)
                    {
                        return true;
                    }
                }

                return false;
            }

            NodeType PopToken()
            {
                var node = NodeType.None;
                var token = Tokens[TokenParentIndex];

                switch (token.Type)
                {
                    case TokenType.Array:
                        node = NodeType.EndArray;
                        break;
                    case TokenType.Object:
                        node = NodeType.EndObject;
                        break;
                }

                var parentIndex = token.Parent;

                while (parentIndex >= 0)
                {
                    var parent = Tokens[parentIndex];

                    if (parent.Type != TokenType.Primitive && parent.Type != TokenType.String)
                    {
                        break;
                    }

                    if (parent.Start != -1 || parent.Parent == -1)
                    {
                        break;
                    }

                    parentIndex = parent.Parent;
                }

                TokenParentIndex = parentIndex;
                return node;
            }
        }

        const int k_DefaultBatchSize = 64;

        /// <summary>
        /// One less than the minimum parent (i.e. -1)
        /// </summary>
        public const int k_IgnoreParent = -2;

        readonly Allocator m_Label;
        readonly ITokenizer m_Tokenizer;
        NativeArray<int> m_Nodes;

        int m_TokenNextIndex;
        int m_TokenParentIndex;

        int m_NodeNextIndex;

        public NativeArray<int> Nodes => m_Nodes;
        public int NodeNextIndex => m_NodeNextIndex;

        public NodeType NodeType { get; private set; }
        public int Node => m_NodeNextIndex <= 0 ? -1 : m_Nodes[m_NodeNextIndex - 1];

        /// <summary>
        /// Number of tokens processed.
        /// </summary>
        public int TokenNextIndex => m_TokenNextIndex;

        public int TokenParentIndex => m_TokenParentIndex;

        public NodeParser(ITokenizer tokenizer, Allocator label =  SerializationConfiguration.DefaultAllocatorLabel) : this(tokenizer, k_DefaultBatchSize, label)
        {
        }

        public NodeParser(ITokenizer tokenizer, int batchSize, Allocator label =  SerializationConfiguration.DefaultAllocatorLabel)
        {
            m_Label = label;
            m_Tokenizer = tokenizer;
            NodeType = NodeType.None;

            if (batchSize < 1)
            {
                throw new ArgumentException("batchSize < 1");
            }

            m_Nodes = new NativeArray<int>(batchSize, m_Label, NativeArrayOptions.UninitializedMemory);
            Seek(0, -1);
        }

        /// <summary>
        /// Seeks the parser to the given token/parent combination.
        /// </summary>
        public void Seek(int index, int parent)
        {
            m_TokenNextIndex = index;
            m_TokenParentIndex = parent;
        }

        /// <summary>
        /// Reads the next node from the input stream and advances the position by one.
        /// </summary>
        public NodeType Step()
        {
            Step(NodeType.Any);
            return NodeType;
        }

        /// <summary>
        /// Reads until the given node type and advances the position.
        /// <param name="type">The node type to break at.</param>
        /// <param name="parent">The minimum parent to break at.</param>
        /// </summary>
        public void Step(NodeType type, int parent = k_IgnoreParent)
        {
            StepBatch(1, type, parent);
        }

        /// <summary>
        /// Reads until the given number of matching nodes have been read.
        /// </summary>
        /// <param name="count">The maximum number of elements of the given type/parent to read.</param>
        /// <param name="type">The node type to break at.</param>
        /// <param name="parent">The minimum parent to break at.</param>
        /// <returns>The number of batch elements that have been read.</returns>
        public unsafe int StepBatch(int count, NodeType type, int parent = k_IgnoreParent)
        {
            if (m_Nodes.Length < count)
            {
                m_Nodes.Dispose();
                m_Nodes = new NativeArray<int>(count, m_Label, NativeArrayOptions.UninitializedMemory);
            }

            var output = new ParseJobOutput();

            new ParseJob
            {
                Output = &output,
                Tokens = (Token*) m_Tokenizer.Tokens.GetUnsafeReadOnlyPtr(),
                TokensLength = m_Tokenizer.TokenNextIndex,
                TokenNextIndex = m_TokenNextIndex,
                TokenParentIndex = m_TokenParentIndex,
                TargetNodeType = type,
                TargetParentIndex = parent,
                TargetNodeCount = count,
                Nodes = (int*) m_Nodes.GetUnsafePtr()
            }
            .Run();

            m_TokenNextIndex = output.TokenNextIndex;
            m_TokenParentIndex = output.TokenParentIndex;
            m_NodeNextIndex = output.NodeNextIndex;

            NodeType = output.NodeType;

            return output.NodeNextIndex;
        }

        public void Dispose()
        {
            m_Nodes.Dispose();
        }
    }
}
