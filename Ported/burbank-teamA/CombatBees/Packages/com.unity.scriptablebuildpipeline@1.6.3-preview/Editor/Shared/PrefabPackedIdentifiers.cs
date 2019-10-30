using System;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;

namespace UnityEditor.Build.Pipeline
{
    /// <summary>
    /// Generates a deterministic identifier using a MD5 hash algorithm and does not require object ordering to be deterministic.
    /// This algorithm ensures objects coming from the same asset are packed closer together and can improve loading performance under certain situations.
    /// </summary>
    public class PrefabPackedIdentifiers : IDeterministicIdentifiers
    {
        /// <inheritdoc />
        public virtual string GenerateInternalFileName(string name)
        {
            var hash = HashingMethods.Calculate(name).ToString();
            return string.Format("CAB-{0}", hash);
        }

        /// <inheritdoc />
        public virtual long SerializationIndexFromObjectIdentifier(ObjectIdentifier objectID)
        {
            byte[] assetHash = HashingMethods.Calculate(objectID.guid, objectID.filePath).ToBytes();
            byte[] objectHash = HashingMethods.Calculate(objectID).ToBytes();

            var assetVal = BitConverter.ToUInt64(assetHash, 0);
            var objectVal = BitConverter.ToUInt64(objectHash, 0);
            return (long)((0xFFFFFFFF00000000 & assetVal) | (0x00000000FFFFFFFF & (objectVal ^ assetVal)));
        }
    }
}
