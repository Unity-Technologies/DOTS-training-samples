using System;
using System.IO;
using UnityEngine;

namespace UnityEditor.Build.CacheServer
{
    /// <inheritdoc />
    /// <summary>
    /// IDownloadItem implementation for downloading to a file specified by path
    /// </summary>
    public class FileDownloadItem : IDownloadItem
    {
        private Stream m_writeStream;
        private string m_tmpPath;

        /// <summary>
        /// The FileId for the FileDownloadItem.  FileId consists of an assets guid and hash code.
        /// </summary>
        public FileId Id { get; private set; }

        /// <summary>
        /// The type of the FileDownloadItems desired item.
        /// </summary>
        public FileType Type { get; private set; }
        
        /// <summary>
        /// File path where downloaded file data is saved. Data is first written to a temporary file location, and moved
        /// into place when the Finish() method is called by the Cache Server Client.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets the write stream for a given FileDownloadItem.  If one does not exist, it will be created.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public Stream GetWriteStream(long size)
        {
            if (m_writeStream == null)
            {
                m_tmpPath = Path.GetTempFileName();
                m_writeStream = new FileStream(m_tmpPath, FileMode.Create, FileAccess.Write);
            }

            return m_writeStream;
        }

        /// <summary>
        /// Create a new FileDownloadItem
        /// </summary>
        /// <param name="fileId">The FileId of the desired item.</param>
        /// <param name="fileType">The FileType of the desired item.</param>
        /// <param name="path">The path of the desired item.</param>
        public FileDownloadItem(FileId fileId, FileType fileType, string path)
        {
            Id = fileId;
            Type = fileType;
            FilePath = path;
        }
        
        /// <summary>
        /// Dispose the FileDownloadItems write stream and move the data from the temporary path to its final destination.
        /// </summary>
        public void Finish()
        {
            if (m_writeStream == null)
                return;
            
            m_writeStream.Dispose();
            try
            {
                if(File.Exists(FilePath))
                    File.Delete(FilePath);
                
                File.Move(m_tmpPath, FilePath);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}