using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Build.CacheServer
{
    /// <summary>
    /// The CacheServerUploader is responsible for uploading assets to a given Cache Server.
    /// </summary>
    public static class CacheServerUploader
    {
        private struct Transaction
        {
            public struct FileInfo
            {
                public readonly FileType type;
                public readonly string path;

                public FileInfo(FileType type, string path)
                {
                    this.type = type;
                    this.path = path;
                }
            }

            public readonly FileId fileId;
            public readonly FileInfo[] files;

            private Transaction(FileId fileId, FileInfo[] files)
            {
                this.fileId = fileId;
                this.files = files;
            }

            public static Transaction CreateForAssetPath(string assetPath)
            {
                var projectRoot = Directory.GetParent(Application.dataPath).FullName;

                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                var hash = AssetDatabase.GetAssetDependencyHash(assetPath);

                var libPath =
                    new[] {projectRoot, "Library", "metadata", guid.Substring(0, 2), guid}
                        .Aggregate(string.Empty, Path.Combine);

                if (!File.Exists(libPath))
                {
                    throw new Exception("Cannot find Library representation for GUID " + guid);
                }

                var files = new List<FileInfo>
                {
                    new FileInfo(FileType.Asset, libPath)
                };

                var infoLibPath = libPath + ".info";
                if (File.Exists(infoLibPath))
                {
                    files.Add(new FileInfo(FileType.Info, infoLibPath));
                }

                var resLibPath = libPath + ".resource";
                if (File.Exists(resLibPath))
                {
                    files.Add(new FileInfo(FileType.Resource, resLibPath));
                }

                return new Transaction(FileId.From(guid, hash.ToString()), files.ToArray());
            }
        }

        /// <summary>
        /// Synchronize project library with the configured Cache Server.
        /// </summary>
        public static void UploadAllFilesToCacheServer()
        {
            string host;
            int port;
            Util.ParseCacheServerIpAddress(Util.ConfigCacheServerAddress, out host, out port);
            UploadAllFilesToCacheServer(host, port);
        }

        /// <summary>
        /// Synchronize project library folder with a remote Cache Server.
        /// </summary>
        /// <param name="host">Host name or IP or remote Cache Server</param>
        /// <param name="port">Port number for remote Cache Server</param>
        public static void UploadAllFilesToCacheServer(string host, int port)
        {
            var client = new Client(host, port);
            client.Connect();

            var assetPaths = AssetDatabase.GetAllAssetPaths();
            var len = assetPaths.Length;
            
            for (var i = 0; i < len; i++)
            {
                var path = assetPaths[i];
                if (!File.Exists(path))
                    continue;
                
                var progress = (float) (i + 1) / (len + 1);

                if (EditorUtility.DisplayCancelableProgressBar("Uploading to Cache Server", path, progress)) break;
                
                try
                {
                    var trx = Transaction.CreateForAssetPath(path);
                    client.BeginTransaction(trx.fileId);

                    foreach (var file in trx.files)
                        using (var stream = new FileStream(file.path, FileMode.Open, FileAccess.Read))
                            client.Upload(file.type, stream);

                    client.EndTransaction();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    break;
                }
            }

            EditorUtility.ClearProgressBar();
            client.Close();
        }
    }
}