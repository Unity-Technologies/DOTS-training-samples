using System.IO;

namespace Unity.Build
{
    internal static class DirectoryInfoExtensions
    {
        public static string GetRelativePath(this DirectoryInfo directoryInfo)
        {
            var path = directoryInfo.FullName.ToForwardSlash();
            var relativePath = new DirectoryInfo(".").FullName.ToForwardSlash();
            return path.StartsWith(relativePath) ? path.Substring(relativePath.Length).TrimStart('/') : path;
        }
    }
}
