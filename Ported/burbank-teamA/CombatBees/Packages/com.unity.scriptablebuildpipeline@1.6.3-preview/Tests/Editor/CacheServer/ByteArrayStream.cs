using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace UnityEditor.CacheServerTests
{
    internal class ByteArrayStream : Stream
    {
        private int m_pos;
    
        public override bool CanRead
        {
            get { return true; }
        }
    
        public override bool CanSeek
        {
            get { return true; }
        }
    
        public override bool CanWrite
        {
            get { return true; }
        }
    
        public override long Length
        {
            get { return BackingBuffer.Length; }
        }
    
        public override long Position
        {
            get { return m_pos; }
            set
            {
                m_pos = Math.Min((int) value, BackingBuffer.Length - 1);
                Debug.Assert(m_pos >= 0);
            }
        }

        public byte[] BackingBuffer { get; private set; }

        public ByteArrayStream(long size)
        {
            BackingBuffer = new byte[size];
            RandomNumberGenerator.Create().GetBytes(BackingBuffer);
        }
    
        public override void SetLength(long value){}
        public override void Flush(){}

        public override void Write(byte[] buffer, int offset, int count)
        {
            Debug.Assert(count <= BackingBuffer.Length - m_pos); // can't write past out buffer length
            count = Math.Min(count, buffer.Length - offset); // Don't read past the input buffer length
            Buffer.BlockCopy(buffer, offset, BackingBuffer, m_pos, count);
            m_pos += count;
        }
    
        public override int Read(byte[] buffer, int offset, int count)
        {
            count = Math.Min(count, BackingBuffer.Length - m_pos); // Don't copy more bytes than we have
            count = Math.Min(count, buffer.Length - offset); // Don't overrun the destination buffer
            Buffer.BlockCopy(BackingBuffer, m_pos, buffer, offset, count);
            m_pos += count;
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = (int) offset;
                    break;
                
                case SeekOrigin.Current:
                    Position += (int) offset;
                    break;
                case SeekOrigin.End:
                    Position = BackingBuffer.Length - (int) offset - 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("origin", origin, null);
            }
        
            return Position;
        }
    }
}