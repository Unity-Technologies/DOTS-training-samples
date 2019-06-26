using Unity.Entities;
using Unity.Mathematics;

namespace Unity.Rendering
{
    public struct RenderBounds : IComponentData
    {
        public AABB Value;
    }

    public struct WorldRenderBounds : IComponentData
    {
        public AABB Value;
    }
    
    public struct ChunkWorldRenderBounds : IComponentData
    {
        public AABB Value;
    }
}