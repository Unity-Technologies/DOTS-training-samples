using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct Train : IComponentData
    {
        public int TrainID;
        public int TrackID;
        public Entity TrackEntity;
        public int TrackPointIndex;
        public bool Forward;
        public float3 Offset;
        public float Duration;
        public float Speed;
    }
}
