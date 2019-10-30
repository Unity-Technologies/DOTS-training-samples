using UnityEditor.Build.Content;

namespace UnityEditor.Build.Pipeline.Interfaces
{
    /// <summary>
    /// Base interface for the generating deterministic identifiers for different parts of the build pipeline.
    /// </summary>
    public interface IDeterministicIdentifiers : IContextObject
    {
        /// <summary>
        /// Generates a deterministic internal file name from the passed in name.
        /// </summary>
        /// <param name="name">Name identifier for internal file name generation</param>
        /// <returns>Deterministic file name.</returns>
        string GenerateInternalFileName(string name);

        /// <summary>
        /// Generates a deterministic id for a given object in the build.
        /// </summary>
        /// <param name="objectID">Object identifier to for id generation.</param>
        /// <returns><c>long</c> representing the id of the objectID.</returns>
        long SerializationIndexFromObjectIdentifier(ObjectIdentifier objectID);
    }
}