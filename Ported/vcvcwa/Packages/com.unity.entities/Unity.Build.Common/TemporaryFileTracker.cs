using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Unity.Build.Common
{
    public class TemporaryFileTracker : IDisposable
    {
        readonly List<string> m_CreatedDirectories = new List<string>();
        readonly List<string> m_TrackedFiles = new List<string>();

        public void CreateDirectory(string path)
        {
            var parent = path;
            while (!Directory.Exists(parent))
            {
                m_CreatedDirectories.Add(parent);
                parent = Path.GetDirectoryName(parent);
            }

            Directory.CreateDirectory(path);
        }

        public string TrackFile(string path, bool ensureDoesntExist = true)
        {
            CreateDirectory(Path.GetDirectoryName(path));
            m_TrackedFiles.Add(path);

            if (ensureDoesntExist)
                EnsureFileDoesntExist(path);

            return path;
        }

        public void EnsureFileDoesntExist(string path)
        {
            if (!File.Exists(path))
                return;

            FileUtil.DeleteFileOrDirectory(path);
            FileUtil.DeleteFileOrDirectory(path + ".meta");
        }

        public void Clear()
        {
            m_CreatedDirectories.Clear();
            m_TrackedFiles.Clear();
        }

        public void Dispose()
        {
            foreach (var file in m_TrackedFiles)
            {
                FileUtil.DeleteFileOrDirectory(file);
                FileUtil.DeleteFileOrDirectory(file + ".meta");
            }

            foreach (var directory in m_CreatedDirectories)
            {
                FileUtil.DeleteFileOrDirectory(directory);
                FileUtil.DeleteFileOrDirectory(directory + ".meta");
            }

            Clear();
        }
    }
}