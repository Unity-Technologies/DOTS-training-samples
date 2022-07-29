using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Components
{
    public struct Car : IComponentData
    {
        public float Speed;
        public float SafeDistance;
        
        public Entity RoadSegment;
        public int LaneNumber;
        public float T;
        public Entity NextIntersection;
    }
}
