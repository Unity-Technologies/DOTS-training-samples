using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace UnityEditor.Build.Pipeline.Utilities
{
    // Since C# only has GZipStream until .NET 4.0, we are forced to implement our own packing system
    // for artifact files. We compact all artifacts into a single GZip Stream with the header before
    // the file name and contents. The header contains the file name length and file length (in bytes).
    public class FileCompressor
    {
        public static bool Compress(string directoryPath, string archiveFilePath)
        {
            if (!directoryPath.EndsWith("/") && !directoryPath.EndsWith("\\"))
                directoryPath += Path.DirectorySeparatorChar.ToString();

            var directory = new DirectoryInfo(directoryPath);
            if (!directory.Exists)
                return false;

            var files = directory.GetFiles("*", SearchOption.AllDirectories);
            files = files.Where(x => (File.GetAttributes(x.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden && x.Extension != ".sbpGz").ToArray();
            if (files.Length == 0)
                return false;

            using (var archiveStream = new FileStream(archiveFilePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var gZipStream = new GZipStream(archiveStream, CompressionMode.Compress))
                {
                    foreach (var file in files)
                    {
                        var relativePath = file.FullName.Substring(directory.FullName.Length);
                        relativePath = relativePath.Replace("\\", "/");

                        using (var fileStream = file.OpenRead())
                        {
                            byte[] filePathBytes = Encoding.ASCII.GetBytes(relativePath);

                            // Write chunk header
                            gZipStream.Write(BitConverter.GetBytes(filePathBytes.Length), 0, sizeof(int));
                            gZipStream.Write(BitConverter.GetBytes(fileStream.Length), 0, sizeof(long));

                            // Write chunk body
                            // Write file path
                            gZipStream.Write(filePathBytes, 0, filePathBytes.Length);

                            // Write file contents
                            byte[] readBuffer = new byte[4096];
                            int readSize = readBuffer.Length;
                            while (readSize == readBuffer.Length)
                            {
                                readSize = fileStream.Read(readBuffer, 0, readBuffer.Length);
                                gZipStream.Write(readBuffer, 0, readSize);
                            }
                        }
                    }
                }
            }
            return true;
        }

        public static bool Decompress(string archiveFilePath, string directoryPath)
        {
            var archiveFile = new FileInfo(archiveFilePath);
            if (!archiveFile.Exists)
                return false;

            var directory = new DirectoryInfo(directoryPath);
            if (!directory.Exists)
                directory.Create();

            using (var archiveStream = archiveFile.OpenRead())
            {
                using (var gZipStream = new GZipStream(archiveStream, CompressionMode.Decompress))
                {
                    while (true)
                    {
                        // Read chunk header
                        byte[] header = new byte[sizeof(int) + sizeof(long)];
                        int readSize = gZipStream.Read(header, 0, header.Length);
                        if (readSize != header.Length)
                            break;

                        int filePathLength = BitConverter.ToInt32(header, 0);
                        long fileLenth = BitConverter.ToInt64(header, sizeof(int));

                        // Read chunk body
                        // Read file path
                        byte[] filePathBytes = new byte[filePathLength];
                        gZipStream.Read(filePathBytes, 0, filePathLength);
                        string filePath = Encoding.ASCII.GetString(filePathBytes);

                        var pathSeperator = filePath.LastIndexOf("/");
                        if (pathSeperator > -1)
                            Directory.CreateDirectory(string.Format("{0}/{1}", directoryPath, filePath.Substring(0, pathSeperator)));

                        // Read file contents
                        using (var fileStream = new FileStream(directoryPath + "/" + filePath, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            byte[] readBuffer = new byte[4096];
                            long readRemaining = fileLenth;
                            while (readRemaining > 0)
                            {
                                readSize = readBuffer.Length < readRemaining ? readBuffer.Length : (int)readRemaining;
                                gZipStream.Read(readBuffer, 0, readSize);
                                fileStream.Write(readBuffer, 0, readSize);
                                readRemaining -= readSize;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}
