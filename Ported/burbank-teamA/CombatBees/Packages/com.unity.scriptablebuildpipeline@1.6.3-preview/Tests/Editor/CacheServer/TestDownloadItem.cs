using System.IO;
using UnityEditor.Build.CacheServer;

namespace UnityEditor.CacheServerTests
{
    internal class TestDownloadItem : IDownloadItem
    {
        private ByteArrayStream m_writeStream;
    
        public FileId Id { get; private set; }
        public FileType Type { get; private set; }
    
        public void Finish(){}
        public byte[] Bytes
        {
            get {  return m_writeStream.BackingBuffer; }
        }

        public Stream GetWriteStream(long size)
        {
            return m_writeStream ?? (m_writeStream = new ByteArrayStream(size));
        }

        public TestDownloadItem(FileId fileId, FileType fileType) 
        {
            Id = fileId;
            Type = fileType;
        }
    }
}