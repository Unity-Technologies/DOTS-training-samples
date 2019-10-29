using System;

namespace Unity.Build
{
    /// <summary>
    /// Attribute used to make a <see cref="UnityEditor.GUID"/> or <see cref="UnityEditor.GlobalObjectId"/> be displayed as an asset. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class AssetGuidAttribute : UnityEngine.PropertyAttribute
    {
        /// <summary>
        /// The <see cref="UnityEngine.Object"/> derived asset type.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// Constructs a new <see cref="AssetGuidAttribute"/> with the provided <see cref="UnityEngine.Object"/> derived asset type.
        /// </summary>
        /// <param name="type">A <see cref="UnityEngine.Object"/> derived asset type.</param>
        public AssetGuidAttribute(Type type)
        {
            Type = type;
        }
    }
}
