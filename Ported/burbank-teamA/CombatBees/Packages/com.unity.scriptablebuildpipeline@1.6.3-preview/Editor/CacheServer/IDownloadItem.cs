using System.IO;

namespace UnityEditor.Build.CacheServer
{
    /// <summary>
    /// Represents a single file download request from a Cache Server.
    /// </summary>
    public interface IDownloadItem
    {
        /// <summary>
        /// the FileId (guid/hash pair) of the file to download
        /// </summary>
        FileId Id { get; }
        
        /// <summary>
        /// the FileType for the given FileId to download
        /// </summary>
        FileType Type { get; }
        
        /// <summary>
        /// Provides a writable stream for saving downloaded file bytes
        /// </summary>
        /// <param name="size">Size of file to download</param>
        /// <returns>A writable stream</returns>
        Stream GetWriteStream(long size);
        
        /// <summary>
        /// Method called when a download is finished. Used to finalize and cleanup a single file download. e.g. to move
        /// a temporary file into place.
        /// </summary>
        void Finish();
    }
}