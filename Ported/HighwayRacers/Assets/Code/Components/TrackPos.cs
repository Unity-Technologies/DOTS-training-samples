using Unity.Entities;

namespace HighwayRacer
{
    public struct TrackPos : IComponentData
    {
        public float Val;    // position along track of the car's rear bumper in meters; start of track is 0.0
    }
}