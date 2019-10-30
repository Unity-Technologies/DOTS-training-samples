using System.Collections.Generic;
using UnityEditor.Build.Content;

namespace UnityEditor.Build.Pipeline.Interfaces
{
    /// <summary>
    /// Optional interface used for overriding the location where specific objects will be serialized
    /// </summary>
    public interface IBundleExplictObjectLayout : IContextObject
    {
        /// <summary>
        /// Map listing object identifiers and their new bundle location
        /// </summary>
        Dictionary<ObjectIdentifier, string> ExplicitObjectLocation { get; }
    }
}
