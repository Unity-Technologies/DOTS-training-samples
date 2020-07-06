using Unity.Entities;

namespace HighwayRacer
{
    public struct RoadInit : IComponentData
    {
        public float Length;
        public int NumCars;
    }
}