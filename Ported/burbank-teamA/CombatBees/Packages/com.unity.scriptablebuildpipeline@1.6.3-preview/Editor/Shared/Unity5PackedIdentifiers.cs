using System;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;

namespace UnityEditor.Build.Pipeline
{
    /// <summary>
    /// Generates a deterministic identifier using a MD4 hash algorithm and does not require object ordering to be deterministic.
    /// This algorithm generates identical results to what is used internally in <seealso cref="BuildPipeline.BuildAssetbundles"/>.
    /// </summary>
    public class Unity5PackedIdentifiers : IDeterministicIdentifiers
    {
        /// <inheritdoc />
        public virtual string GenerateInternalFileName(string name)
        {
            return "CAB-" + HashingMethods.Calculate<MD4>(name);
        }

        /// <inheritdoc />
        public virtual long SerializationIndexFromObjectIdentifier(ObjectIdentifier objectID)
        {
            RawHash hash;
            if (objectID.fileType == FileType.MetaAssetType || objectID.fileType == FileType.SerializedAssetType)
                hash = HashingMethods.Calculate<MD4>(objectID.guid.ToString(), objectID.fileType, objectID.localIdentifierInFile);
            else
                hash = HashingMethods.Calculate<MD4>(objectID.filePath, objectID.localIdentifierInFile);
            return BitConverter.ToInt64(hash.ToBytes(), 0);
        }
    }
}