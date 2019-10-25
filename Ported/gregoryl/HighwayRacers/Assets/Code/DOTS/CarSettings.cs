using Unity.Entities;

namespace HighwayRacers
{
    struct CarSettings : IComponentData
    {
        public float DefaultSpeed;
        public float OvertakePercent;
        public float LeftMergeDistance;
        public float MergeSpace;
        public float OvertakeEagerness;
    }
}
