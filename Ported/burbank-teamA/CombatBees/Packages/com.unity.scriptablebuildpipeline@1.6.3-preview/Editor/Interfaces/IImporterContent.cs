using System;
using System.Collections.Generic;

namespace UnityEditor.Build.Pipeline.Interfaces
{
    [Serializable]
    public struct ImporterContentInfo : IEquatable<ImporterContentInfo>
    {
        public GUID Asset { get; set; }
        public Type Importer { get; set; }
        //public consumer

        public bool Equals(ImporterContentInfo other)
        {
            return Asset == other.Asset && Importer == other.Importer;
        }
    }


    public interface IImporterContent : IContextObject
    {
        List<ImporterContentInfo> ImportedContent { get; }
    }
}

// To do this at a generic level, there are 3 pieces of information that we need
// 1. Asset
// 2. Importer
// 3. How to consume the results from the Importer
//      - Raw files to add to bundles / place some location?
//      - Generated assets that need further processing (and which bundle to add them to)