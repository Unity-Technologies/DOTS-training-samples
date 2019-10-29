using Unity.Entities;

namespace Unity.Rendering
{
    /// <summary>
    /// Used by skinned mesh entities to retrieve the related skinned entities.
    /// </summary>
    public struct SkinnedEntityReference : IComponentData
    {
        public Entity Value;
    }
}
