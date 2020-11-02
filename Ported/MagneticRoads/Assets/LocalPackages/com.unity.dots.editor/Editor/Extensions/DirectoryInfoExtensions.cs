using System.IO;
using System.Linq;

namespace Unity.Entities.Editor
{
    static class DirectoryInfoExtensions
    {
        public static DirectoryInfo Combine(this DirectoryInfo directoryInfo, params string[] paths)
        {
            return new DirectoryInfo(Path.Combine(paths.Prepend(directoryInfo.FullName).ToArray()));
        }

        public static FileInfo GetFile(this DirectoryInfo directoryInfo, string fileName)
        {
            return new FileInfo(Path.Combine(directoryInfo.FullName, fileName));
        }

        public static FileInfo GetFile(this DirectoryInfo directoryInfo, FileInfo file)
        {
            return new FileInfo(Path.Combine(directoryInfo.FullName, file.Name));
        }

        public static string ToHyperLink(this DirectoryInfo directoryInfo, string hyperlinkKey)
        {
            return directoryInfo.FullName.ToHyperLink(hyperlinkKey);
        }

        public static void CopyTo(this DirectoryInfo directoryInfo, DirectoryInfo destination, bool recursive)
        {
            if (!Directory.Exists(directoryInfo.FullName))
            {
                throw new DirectoryNotFoundException($"Directory '{directoryInfo.FullName}' not found.");
            }

            // Make sure destination exist
            destination.Create();

            // Copy files
            foreach (var file in directoryInfo.GetFiles())
            {
                file.CopyTo(Path.Combine(destination.FullName, file.Name), true);
            }

            // Copy subdirs
            if (recursive)
            {
                foreach (var subdir in directoryInfo.GetDirectories())
                {
                    CopyTo(subdir, destination.Combine(subdir.Name), recursive);
                }
            }
        }
    }
}
