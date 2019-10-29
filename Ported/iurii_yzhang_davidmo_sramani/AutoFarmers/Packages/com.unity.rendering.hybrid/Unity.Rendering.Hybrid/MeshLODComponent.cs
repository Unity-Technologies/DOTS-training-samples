using Unity.Entities;
using Unity.Mathematics;

namespace Unity.Rendering
{
    public struct ActiveLODGroupMask : IComponentData
    {
        public int LODMask;
    }

    public struct MeshLODGroupComponent : IComponentData
    {
        public Entity    ParentGroup;
        public int       ParentMask;
        
        public float4    LODDistances0;
        public float4    LODDistances1;

        public float3    LocalReferencePoint;
    }
    

    public struct HLODComponent : IComponentData
    {
    }
    
    public struct MeshLODComponent : IComponentData
    {
        public Entity   Group;
        public int      LODMask;
    }
}
