using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct Bar : IComponentData
    {
        public float3 oldDirection;
        public float thickness;


        //temp
        public int indexPoint;
    }
}