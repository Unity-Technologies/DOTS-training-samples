using Authoring;
using Unity.Entities;

namespace Aspects
{
    readonly partial struct LaneAspect : IAspect
    {
        public readonly Entity Self;

        private readonly RefRO<Lane> lane;

        public int LaneNumber
        {
            get => lane.ValueRO.LaneNumber;
        }

        public float LaneLength
        {
            get => lane.ValueRO.LaneLength;
        }
    }
}
