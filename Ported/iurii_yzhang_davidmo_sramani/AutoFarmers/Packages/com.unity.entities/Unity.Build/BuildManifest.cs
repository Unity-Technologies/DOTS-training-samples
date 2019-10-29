using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Unity.Build
{
    /// <summary>
    /// Holds information about exported assets throughout a <see cref="BuildPipeline"/> execution.
    /// All exported assets listed in the build manifest will also be installed in the build data directory.
    /// </summary>
    public sealed class BuildManifest
    {
        readonly Dictionary<Guid, string> m_Assets = new Dictionary<Guid, string>();
        readonly List<FileInfo> m_ExportedFiles = new List<FileInfo>();

        /// <summary>
        /// A dictionary of all assets exported during the <see cref="BuildPipeline"/> execution.
        /// </summary>
        public IReadOnlyDictionary<Guid, string> Assets => m_Assets;

        /// <summary>
        /// The list of exported files during the <see cref="BuildPipeline"/> execution.
        /// </summary>
        public IEnumerable<FileInfo> ExportedFiles => m_ExportedFiles;

        /// <summary>
        /// Add an asset and its exported files to the <see cref="BuildManifest"/>.
        /// </summary>
        /// <param name="assetGuid">The asset <see cref="Guid"/>.</param>
        /// <param name="assetPath">The asset path.</param>
        /// <param name="exportedFiles">The files that were exported by the asset exporter for this asset.</param>
        public void Add(Guid assetGuid, string assetPath, IEnumerable<FileInfo> exportedFiles)
        {
            if (exportedFiles == null || exportedFiles.Count() == 0)
            {
                return;
            }

            m_Assets.Add(assetGuid, assetPath);
            m_ExportedFiles.AddRange(exportedFiles);
        }
    }
}
