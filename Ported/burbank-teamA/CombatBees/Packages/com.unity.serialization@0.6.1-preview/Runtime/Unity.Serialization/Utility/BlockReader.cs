using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Unity.Serialization
{
    /// <summary>
    /// A block used to store a chunk of characters read from disc.
    /// </summary>
    class Block
    {
        public char[] Buffer;
        public int Length;
    }

    interface IBlockReader
    {
        /// <summary>
        /// Returns the next block in the stream.
        /// </summary>
        Block GetNextBlock();
    }

    /// <summary>
    /// An asynchronous block reader that tries to read one block ahead in the stream.
    ///
    /// * The first call to `GetNextBlock` reads synchronously and queues the second block.
    /// * Subsequent calls wait on the running task and start the next task before returning.
    ///
    /// </summary>
    class AsyncBlockReader : IBlockReader
    {
        readonly TextReader m_Reader;
        readonly IList<Block> m_Blocks = new List<Block>();
        int m_NextBufferIndex;
        Task<int> m_ReadBlockTask;

        public AsyncBlockReader(TextReader reader, int bufferSize)
        {
            m_Reader = reader;
            m_Blocks.Add(new Block { Buffer = new char[bufferSize] });
            m_Blocks.Add(new Block { Buffer = new char[bufferSize] });
        }

        public Block GetNextBlock()
        {
            var block = m_Blocks[m_NextBufferIndex++];

            if (m_NextBufferIndex >= m_Blocks.Count)
            {
                m_NextBufferIndex = 0;
            }

            int count;

            if (null == m_ReadBlockTask)
            {
                // If we have no task running read the block synchronously
                count = m_Reader.ReadBlock(block.Buffer, index: 0, count: block.Buffer.Length);
            }
            else
            {
                // Wait on the current queued task
                if (!m_ReadBlockTask.IsCompleted)
                {
                    m_ReadBlockTask.Wait();
                }

                count = m_ReadBlockTask.Result;
            }

            block.Length = count;

            if (count == block.Buffer.Length)
            {
                // Queue up the next block if we still have some bytes to read
                var next = m_Blocks[m_NextBufferIndex];
                m_ReadBlockTask = m_Reader.ReadBlockAsync(next.Buffer, index: 0, count: next.Buffer.Length);
            }
            else
            {
                m_ReadBlockTask = null;
            }

            return block;
        }
    }

    /// <summary>
    /// A synchronous block reader. This is a simple wrapper over `StreamReader.ReadBlock`.
    /// </summary>
    class SyncBlockReader : IBlockReader
    {
        readonly TextReader m_Reader;
        readonly Block m_Block;

        public SyncBlockReader(TextReader reader, int bufferSize)
        {
            m_Reader = reader;
            m_Block = new Block {Buffer = new char[bufferSize]};
        }

        public Block GetNextBlock()
        {
            m_Block.Length = m_Reader.ReadBlock(m_Block.Buffer, 0, m_Block.Buffer.Length);
            return m_Block;
        }
    }
}
