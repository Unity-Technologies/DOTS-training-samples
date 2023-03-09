using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct Position : IComponentData
    {
        // use Aspects instead??
        //An aspect is an object-like wrapper that you can use to group
        //together a subset of an entity's components into a single C# struct.
        public float3 value;
    }

    public struct Volume : IComponentData
    {
        public float value;
    }
}
