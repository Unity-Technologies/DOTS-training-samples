using Unity.Entities;

namespace Unity.Transforms
{
    /// <summary>
    /// Holds the index offset at which the skin matrices of this entity are stored in the buffer containing all skin matrices.
    /// </summary>
    public struct BoneIndexOffset : IComponentData
    {
        public float Value;
    }
}
